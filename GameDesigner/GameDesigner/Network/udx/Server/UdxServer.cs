namespace Net.Server
{
    using Net.Share;
    using global::System;
    using global::System.Collections.Concurrent;
    using global::System.Collections.Generic;
    using global::System.IO;
    using global::System.Net;
    using global::System.Runtime.InteropServices;
    using global::System.Text;
    using global::System.Threading;
    using Udx;
    using Debug = Event.NDebug;
    using Net.System;

    /// <summary>
    /// udx服务器类型  只能300人以下连接, 如果想要300个客户端以上, 请进入udx网址:www.goodudx.com 联系作者下载专业版FastUdxApi.dll, 然后更换下框架内的FastUdxApi.dll即可
    /// 第三版本 2020.9.14
    /// <para>Player:当有客户端连接服务器就会创建一个Player对象出来, Player对象和XXXClient是对等端, 每当有数据处理都会通知Player对象. </para>
    /// <para>Scene:你可以定义自己的场景类型, 比如帧同步场景处理, mmorpg场景什么处理, 可以重写Scene的Update等等方法实现每个场景的更新和处理. </para>
    /// </summary>
    public class UdxServer<Player, Scene> : ServerBase<Player, Scene>, IUDX where Player : UdxPlayer, new() where Scene : NetScene<Player>, new()
    {
        /// <summary>
        /// udx服务器对象
        /// </summary>
        public new IntPtr Server;
        private UDXPRC uDXPRC;

        public override void Start(ushort port = 6666)
        {
            if (Server != IntPtr.Zero)//如果服务器套接字已创建
                throw new Exception("服务器已经运行，不可重新启动，请先关闭后在重启服务器");
            base.Start(port);
        }

        protected override void CreateServerSocket(ushort port)
        {
#if !UNITY_EDITOR && !UNITY_STANDALONE && !UNITY_ANDROID && !UNITY_IOS
            string path = AppDomain.CurrentDomain.BaseDirectory;
            if (!File.Exists(path + "\\FastUdxApi.dll"))
                throw new FileNotFoundException($"FastUdxApi.dll没有在程序根目录中! 请从GameDesigner文件夹下找到FastUdxApi.dll复制到{path}目录下.");
#endif
            if (!UdxLib.INIT)
            {
                UdxLib.INIT = true;
                UdxLib.UInit(MaxThread);
                UdxLib.UEnableLog(false);
            }
            UdxLib.UDXS.Add(this);
            Server = UdxLib.UCreateFUObj();
            UdxLib.UBind(Server, null, port);
            uDXPRC = new UDXPRC(ProcessReceive);
            UdxLib.USetFUCB(Server, uDXPRC);
            GC.KeepAlive(uDXPRC);
        }

        protected override void ReceiveProcessed(EndPoint remotePoint, ref bool isSleep)
        {
        }

        protected void ProcessReceive(UDXEVENT_TYPE eventtype, int erro, IntPtr cli, IntPtr pData, int len)
        {
            try
            {
                Player client = null;
                switch (eventtype)
                {
                    case UDXEVENT_TYPE.E_CONNECT:
                        byte[] ipbytes = new byte[128];
                        int port = 0;
                        int ntype = 0;
                        UdxLib.USetGameMode(cli, true);
                        UdxLib.UGetRemoteAddr(cli, ipbytes, ref port, ref ntype);
                        port = UdxLib.UGetDesStreamID(cli);
                        string ip = Encoding.ASCII.GetString(ipbytes, 0, 128);
                        ip = ip.Replace("\0", "");
                        var remotePoint = new IPEndPoint(IPAddress.Parse(ip), port);
                        client = AcceptHander(null, remotePoint);
                        client.Udx = cli;
                        var handle = GCHandle.Alloc(client);
                        var ptr = GCHandle.ToIntPtr(handle);
                        UdxLib.USetUserData(cli, ptr);
                        client.Handle = handle;
                        break;
                    case UDXEVENT_TYPE.E_LINKBROKEN:
                        var ptr1 = UdxLib.UGetUserData(cli);
                        client = GCHandle.FromIntPtr(ptr1).Target as Player;
                        if (client == null)
                            return;
                        RemoveClient(client);
                        break;
                    case UDXEVENT_TYPE.E_DATAREAD:
                        var ptr2 = UdxLib.UGetUserData(cli);
                        client = GCHandle.FromIntPtr(ptr2).Target as Player;
                        if (client == null)
                            return;
                        client.heart = 0;
                        var buffer = BufferPool.Take(len);
                        buffer.Count = len;
                        Marshal.Copy(pData, buffer, 0, len);
                        receiveCount += len;
                        receiveAmount++;
                        client.RevdQueue.Enqueue(buffer);
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        }

        protected override bool IsInternalCommand(Player client, RPCModel model)
        {
            if (model.cmd == NetCmd.Connect | model.cmd == NetCmd.Broadcast)
                return true;
            return false;
        }

        protected override void SendRTDataHandle(Player client, QueueSafe<RPCModel> rtRPCModels)
        {
            SendDataHandle(client, rtRPCModels, true);
        }

        protected unsafe override void SendByteData(Player client, byte[] buffer, bool reliable)
        {
            if (client.Udx == IntPtr.Zero)
                return;
            if (buffer.Length == frame)//解决长度==6的问题(没有数据)
                return;
            sendAmount++;
            sendCount += buffer.Length;
            fixed (byte* ptr = buffer)
            {
                int count = UdxLib.USend(client.Udx, ptr, buffer.Length);
                if (count <= 0)
                    OnSendErrorHandle?.Invoke(client, buffer, reliable);
            }
        }

        public override void RemoveClient(Player client)
        {
            base.RemoveClient(client);
        }

        public override void Close()
        {
            base.Close();
            if (Server != IntPtr.Zero)
            {
                UdxLib.UDestroyFUObj(Server);
                Server = IntPtr.Zero;
            }
            UdxLib.UDXS.Remove(this);
            if (UdxLib.UDXS.Count == 0 & UdxLib.INIT)
            {
                UdxLib.UUnInit();
                UdxLib.INIT = false;
            }
        }

        ~UdxServer()
        {
            Close();
        }
    }

    /// <summary>
    /// 默认udx服务器，当不需要处理Player对象和Scene对象时可使用
    /// </summary>
    public class UdxServer : UdxServer<UdxPlayer, NetScene<UdxPlayer>>
    {
    }
}