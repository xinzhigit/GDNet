using Net.Share;
using Net.System;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Net.Plugins
{
    class Cmd
    {
        /// <summary>
        /// 网络帧
        /// </summary>
        internal const byte Frame = 2;
        /// <summary>
        /// 网络帧确认
        /// </summary>
        internal const byte Ack = 4;
        /// <summary>
        /// 冗余帧
        /// </summary>
        internal const byte DAck = 6;
    }

    class DataFrame
    {
        private int hashCode;
        internal byte[] buffer;
        internal int tick;
        public DataFrame()
        {
        }
        public DataFrame(int hashCode, byte[] buffer)
        {
            this.hashCode = hashCode;
            this.buffer = buffer;
        }
        public override int GetHashCode()
        {
            return hashCode;
        }
    }

    class DataPackage 
    {
        internal Segment revderBuffer;
        internal int revderFrameEnd;
        internal byte[] revderHash;
        internal int revderHashCount;
        internal bool finish;
    }

    /// <summary>
    /// Gcp可靠协议核心类
    /// </summary>
    public class GcpKernel : IDisposable
    {
        private uint pushPackage;
        private uint senderPackage;
        private uint revderPackage;
        private readonly Queue<byte[]> senderQueue = new Queue<byte[]>();
        private readonly Queue<Segment> revderQueue = new Queue<Segment>();
        public Action<byte[]> OnSender;
        public Action<RTProgress> OnSendProgress;
        public Action<RTProgress> OnRevdProgress;
        private readonly MyDictionary<uint, MyDictionary<int, DataFrame>> senderDict = new MyDictionary<uint, MyDictionary<int, DataFrame>>();
        private readonly MyDictionary<uint, DataPackage> revderDict = new MyDictionary<uint, DataPackage>();
        public EndPoint RemotePoint { get; set; }
        private readonly object SyncRoot = new object();
        private int tick;
        private int flowTick;
        private int currFlow;
        private int progressTick;
        public ushort MTU { get; set; } = 1300;
        public int RTO = 1000;
        private int ackNumber = 1;
        private int currAck;
        public int MTPS { get; set; } = 1024 * 1024;
        public int ProgressDataLen { get; set; } = ushort.MaxValue;//要求数据大于多少才会调用发送，接收进度值
        public FlowControlMode FlowControl { get; set; } = FlowControlMode.Normal;

        public void Update()
        {
            lock (SyncRoot)
            {
                CheckNextSend();
                tick = Environment.TickCount;
                if (tick >= flowTick)
                {
                    flowTick = tick + 1000;
                    currFlow = 0;
                    currAck = 0;
                    ackNumber = FlowControl == FlowControlMode.Normal ? 1 : 100;
                }
                if (senderDict.Count <= 0)
                    return;
                var length = senderPackage + senderDict.Count;
                for (uint i = senderPackage; i < length; i++)
                {
                    if (!FastRetransmit(i))
                        if(i == senderPackage)
                            senderPackage++;
                    if (currFlow >= MTPS | currAck >= ackNumber)
                        break;
                }
            }
        }
        public int Receive(out Segment segment)
        {
            lock (SyncRoot)
            {
                if (revderQueue.Count <= 0)
                {
                    segment = null;
                    return 0;
                }
                segment = revderQueue.Dequeue();
                return segment.Count;
            }
        }
        public void Send(byte[] buffer)
        {
            lock (SyncRoot)
            {
                senderQueue.Enqueue(buffer);
            }
        }
        private void CheckNextSend()
        {
            if (senderQueue.Count > 0)
            {
                var current = senderQueue.Dequeue();
                var count = current.Length;
                var frameEnd = (int)Math.Ceiling(count / (float)MTU);
                using (var segment = BufferPool.Take())
                {
                    var dic = new MyDictionary<int, DataFrame>(frameEnd);
                    senderDict.Add(pushPackage, dic);
                    for (int serialNo = 0; serialNo < frameEnd; serialNo++)
                    {
                        segment.SetPositionLength(0);
                        segment.WriteByte(Cmd.Frame);
                        segment.Write(pushPackage);
                        segment.Write(serialNo);
                        segment.Write(count);
                        var offset = serialNo * MTU;
                        segment.Write(current, offset, offset + MTU >= count ? count - offset : MTU);
                        var dataFrame = new DataFrame(serialNo, segment.ToArray());
                        dic.Add(serialNo, dataFrame);
                    }
                    pushPackage++;
                }
            }
        }
        public void Input(byte[] buffer)
        {
            lock (SyncRoot)
            {
                var segment = new Segment(buffer, false);
                var flags = segment.ReadByte();
                if (flags == Cmd.Frame)
                {
                    var package = segment.ReadUInt32();
                    var serialNo = segment.ReadInt32();
                    var dataLen = segment.ReadInt32();
                    if (package < revderPackage)
                        goto J;
                    if (!revderDict.TryGetValue(package, out var dp))
                        revderDict.Add(package, dp = new DataPackage());
                    if (dp.revderBuffer == null)
                    {
                        dp.revderBuffer = BufferPool.Take(dataLen);
                        dp.revderBuffer.Count = dataLen;
                        dp.revderFrameEnd = (int)Math.Ceiling(dataLen / (float)MTU);
                        dp.revderHash = new byte[dp.revderFrameEnd];
                    }
                    if (dp.revderHash[serialNo] == 0)
                    {
                        dp.revderHashCount++;
                        dp.revderHash[serialNo] = 1;
                        Buffer.BlockCopy(segment, segment.Position, dp.revderBuffer, (int)(serialNo * MTU), segment.Count - segment.Position);
                        if (dp.revderHashCount >= dp.revderFrameEnd)
                            dp.finish = true;
                    }
                    while (revderDict.TryGetValue(revderPackage, out var dp1))
                    {
                        if (tick >= progressTick)
                        {
                            progressTick = tick + 1000;
                            if (dp.revderFrameEnd * MTU >= ProgressDataLen)
                            {
                                var progress = (dp.revderHashCount / (float)dp.revderFrameEnd) * 100f;
                                OnRevdProgress?.Invoke(new RTProgress(progress, (RTState)(progress >= 100f ? 3 : 1)));
                            }
                        }
                        if (!dp1.finish)
                            break;
                        revderQueue.Enqueue(dp1.revderBuffer);
                        revderDict.Remove(revderPackage);
                        revderPackage++;
                        dp1.revderBuffer = null;
                        dp1.revderHash = null;
                        dp1.revderHashCount = 0;
                    }
                J: segment.SetPositionLength(0);
                    segment.WriteByte(Cmd.Ack);
                    segment.Write(package);
                    segment.Write(serialNo);
                    segment.Write(dataLen);
                    var bytes = segment.ToArray(true);
                    OnSender(bytes);
                }
                else if (flags == Cmd.Ack)
                {
                    var package = segment.ReadUInt32();
                    var serialNo = segment.ReadInt32();
                    var dataLen = segment.ReadInt32();
                    if (senderDict.TryGetValue(package, out var dic))
                    {
                        if (dic.Remove(serialNo))
                            ackNumber++;
                        if (dic.Count <= 0)
                            senderDict.Remove(package);
                        if (tick >= progressTick)
                        {
                            progressTick = tick + 1000;
                            if (dataLen >= ProgressDataLen)
                            {
                                var progress = ((dic.Count * MTU) / (float)dataLen) * 100f;
                                OnSendProgress?.Invoke(new RTProgress(progress, (RTState)(progress >= 100f ? 3 : 1)));
                            }
                        }
                    }
                    FastRetransmit(senderPackage);
                }
                else if (flags == Cmd.DAck)
                {
                    var package = segment.ReadUInt32();
                    FastRetransmit(package);
                }
            }
        }
        private bool FastRetransmit(uint package)
        {
            if (senderDict.TryGetValue(package, out var dict))
            {
                foreach (var sender in dict.Values)
                {
                    if (tick >= sender.tick & currFlow < MTPS & currAck < ackNumber)
                    {
                        sender.tick = tick + RTO;
                        var count = sender.buffer.Length;
                        currFlow += count;
                        currAck++;
                        OnSender(sender.buffer);
                    }
                }
                return true;
            }
            return false;
        }
        public bool HasSend()
        {
            return false;
        }
        public void Dispose()
        {
            lock (SyncRoot)
            {
                pushPackage = 0;
                senderPackage = 0;
                revderPackage = 0;
                senderQueue.Clear();
                revderQueue.Clear();
                senderDict.Clear();
                revderDict.Clear();
                RemotePoint = null;
            }
        }
        public override string ToString()
        {
            return $"{RemotePoint}";
        }
    }
}