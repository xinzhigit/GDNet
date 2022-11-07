namespace Net.Client
{
    using Net.Share;
    using global::System;
    using global::System.IO;
    using global::System.Net.Sockets;
    using global::System.Runtime.InteropServices;
    using global::System.Threading;
    using global::System.Threading.Tasks;
    using Kcp;
    using static Kcp.KcpLib;
    using global::System.Net;
    using global::System.Reflection;
    using Net.System;
    using AOT;
    using global::System.Collections.Generic;

    /// <summary>
    /// kcp客户端
    /// </summary>
    [Serializable]
    public unsafe class KcpClient : ClientBase
    {
        private readonly IntPtr kcp;
        private readonly outputCallback output;
        private static readonly Dictionary<IntPtr, KcpClient> KcpDict = new Dictionary<IntPtr, KcpClient>();

        public KcpClient() : base()
        {
            kcp = ikcp_create(1400, (IntPtr)1);
            output = new outputCallback(Output);
            IntPtr outputPtr = Marshal.GetFunctionPointerForDelegate(output);
            ikcp_setoutput(kcp, outputPtr);
            ikcp_wndsize(kcp, ushort.MaxValue, ushort.MaxValue);
            ikcp_nodelay(kcp, 1, 10, 2, 1);
            KcpDict[kcp] = this;
        }

        public KcpClient(bool useUnityThread) : this()
        {
            UseUnityThread = useUnityThread;
        }

        ~KcpClient()
        {
            KcpDict.Remove(kcp);
        }

        private byte[] addressBuffer;
        internal byte[] RemoteAddressBuffer()
        {
            if (addressBuffer != null)
                return addressBuffer;
            var socketAddress = Client.RemoteEndPoint.Serialize();
            addressBuffer = (byte[])socketAddress.GetType().GetField("m_Buffer", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(socketAddress);
            return addressBuffer;
        }

        //IL2CPP使用Marshal.GetFunctionPointerForDelegate需要设置委托方法为静态方法，并且要添加上特性MonoPInvokeCallback
        [MonoPInvokeCallback(typeof(outputCallback))]
        public static unsafe int Output(IntPtr buf, int len, IntPtr kcp, IntPtr user)
        {
            var client = KcpDict[kcp];
            client.sendCount += len;
            client.sendAmount++;
#if WINDOWS
            return Win32KernelAPI.sendto(client.Client.Handle, (byte*)buf, len, SocketFlags.None, client.RemoteAddressBuffer(), 16);
#else
            byte[] buff = new byte[len];
            Marshal.Copy(buf, buff, 0, len);
            return client.Client.Send(buff, 0, len, SocketFlags.None);
#endif
        }

        public override void Receive(bool isSleep)
        {
            if (Client.Poll(1, SelectMode.SelectRead))
            {
                var segment = BufferPool.Take(65507);
                segment.Count = Client.Receive(segment, 0, segment.Length, SocketFlags.None, out SocketError error);
                if (error != SocketError.Success)
                {
                    BufferPool.Push(segment);
                    return;
                }
                if (segment.Count == 0)
                {
                    BufferPool.Push(segment);
                    return;
                }
                receiveCount += segment.Count;
                receiveAmount++;
                heart = 0;
                fixed (byte* p = &segment.Buffer[0])
                    ikcp_input(kcp, p, segment.Count);
                ikcp_update(kcp, (uint)Environment.TickCount);
                int len;
                while ((len = ikcp_peeksize(kcp)) > 0)
                {
                    var segment1 = BufferPool.Take(len);
                    fixed (byte* p1 = &segment1.Buffer[0])
                    {
                        segment1.Count = ikcp_recv(kcp, p1, len);
                        ResolveBuffer(ref segment1, false);
                        BufferPool.Push(segment1);
                    }
                    revdLoopNum++;
                }
                BufferPool.Push(segment);
            }
            else
            {
                Thread.Sleep(1);
            }
        }

        protected override void SendByteData(byte[] buffer, bool reliable)
        {
            fixed (byte* p = &buffer[0])
            {
                int count = ikcp_send(kcp, p, buffer.Length);
                ikcp_update(kcp, (uint)Environment.TickCount);
                if (count < 0)
                    OnSendErrorHandle?.Invoke(buffer, reliable);
            }
        }

        protected override void SendRTDataHandle()
        {
            SendDataHandle(rtRPCModels, true);
        }

        public override void Close(bool await = true, int millisecondsTimeout = 1000)
        {
            base.Close(await);
            addressBuffer = null;
        }

        private class KcpCallback
        {
            public Socket Client;

            public KcpCallback(Socket Client)
            {
                this.Client = Client;
            }

            public unsafe int Output(IntPtr buf, int len, IntPtr kcp, IntPtr user)
            {
                byte[] buff = new byte[len];
                Marshal.Copy(buf, buff, 0, len);
                Client.Send(buff, 0, len, SocketFlags.None);
                return 0;
            }
        }

        /// <summary>
        /// udp压力测试
        /// </summary>
        /// <param name="ip">服务器ip</param>
        /// <param name="port">服务器端口</param>
        /// <param name="clientLen">测试客户端数量</param>
        /// <param name="dataLen">每个客户端数据大小</param>
        public static CancellationTokenSource Testing(string ip, int port, int clientLen, int dataLen, int millisecondsTimeout, Action<KcpClientTest> onInit = null, Action<List<KcpClientTest>> fpsAct = null, IAdapter adapter = null)
        {
            var cts = new CancellationTokenSource();
            Task.Run(() =>
            {
                var clients = new List<KcpClientTest>();
                for (int i = 0; i < clientLen; i++)
                {
                    var client = new KcpClientTest();
                    onInit?.Invoke(client);
                    if (adapter != null)
                        client.AddAdapter(adapter);
                    client.Connect(ip, port);
                    clients.Add(client);
                }
                var buffer = new byte[dataLen];
                Task.Run(() =>
                {
                    while (!cts.IsCancellationRequested)
                    {
                        Thread.Sleep(1000);
                        fpsAct?.Invoke(clients);
                        for (int i = 0; i < clients.Count; i++)
                        {
                            clients[i].NetworkFlowHandler();
                            clients[i].fps = 0;
                        }
                    }
                });
                int threadNum = (clientLen / 1000) + 1;
                for (int i = 0; i < threadNum; i++)
                {
                    int index = i * 1000;
                    int end = index + 1000;
                    if (index >= clientLen)
                        break;
                    Task.Run(() =>
                    {
                        if (end > clientLen)
                            end = clientLen;
                        var tick = Environment.TickCount + millisecondsTimeout;
                        while (!cts.IsCancellationRequested)
                        {
                            bool canSend = false;
                            if (Environment.TickCount >= tick)
                            {
                                tick = Environment.TickCount + millisecondsTimeout;
                                canSend = true;
                            }
                            for (int ii = index; ii < end; ii++)
                            {
                                try
                                {
                                    var client = clients[ii];
                                    if (canSend)
                                    {
                                        //client.SendRT(NetCmd.Local, buffer);
                                        client.AddOperation(new Operation(NetCmd.Local, buffer));
                                    }
                                    client.Update();
                                }
                                catch (Exception ex)
                                {
                                    Event.NDebug.LogError(ex);
                                }
                            }
                        }
                    });
                }
                while (!cts.IsCancellationRequested)
                    Thread.Sleep(30);
                Thread.Sleep(100);
                for (int i = 0; i < clients.Count; i++)
                    clients[i].Close(false);
            }, cts.Token);
            return cts;
        }
    }

    public class KcpClientTest : KcpClient
    {
        public int fps;
        public int revdSize { get { return receiveCount; } }
        public int sendSize { get { return sendCount; } }
        public int sendNum { get { return sendAmount; } }
        public int revdNum { get { return receiveAmount; } }
        public int resolveNum { get { return receiveAmount; } }
        private byte[] addressBuffer;
        public KcpClientTest() : base()
        {
            OnRevdBufferHandle += (model) => { fps++; };
            OnOperationSync += (list) => { fps++; };
        }
        protected override Task<bool> ConnectResult(string host, int port, int localPort, Action<bool> result)
        {
            Client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            this.localPort = localPort;
            Client.Connect(host, port);
            Client.Blocking = false;
#if WINDOWS
            var socketAddress = Client.RemoteEndPoint.Serialize();
            addressBuffer = (byte[])socketAddress.GetType().GetField("m_Buffer", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(socketAddress);
#endif
            rPCModels.Enqueue(new RPCModel(NetCmd.Connect, new byte[0]));
            SendDirect();
            Connected = true;
            result(true);
            return Task.FromResult(Connected);
        }
        protected override void StartupThread() { }

        protected override void OnConnected(bool result) { NetworkState = NetworkState.Connected; }

        //protected override void ResolveBuffer(ref Segment buffer, bool isTcp)
        //{
        //    base.ResolveBuffer(ref buffer, isTcp);
        //}
//        protected unsafe override void SendByteData(byte[] buffer, bool reliable)
//        {
//            sendCount += buffer.Length;
//            sendAmount++;
//#if WINDOWS
//            fixed (byte* ptr = buffer)
//                Win32KernelAPI.sendto(Client.Handle, ptr, buffer.Length, SocketFlags.None, addressBuffer, 16);
//#else
//            Client.Send(buffer, 0, buffer.Length, SocketFlags.None);
//#endif
//        }
        //protected internal override byte[] OnSerializeOptInternal(OperationList list)
        //{
        //    return new byte[0];
        //}
        //protected internal override OperationList OnDeserializeOptInternal(byte[] buffer, int index, int count)
        //{
        //    return default;
        //}
        /// <summary>
        /// 单线程更新，需要开发者自动调用更新
        /// </summary>
        public void Update()
        {
            if (!Connected)
                return;
            Receive(false);
            SendDirect();
            NetworkEventUpdate();
        }
        public override string ToString()
        {
            return $"uid:{Identify} conv:{Connected}";
        }
    }
}
