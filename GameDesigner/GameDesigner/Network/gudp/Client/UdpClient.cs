namespace Net.Client
{
    using Net.Share;
    using global::System;
    using global::System.Collections.Generic;
    using global::System.IO;
    using global::System.Net.Sockets;
    using global::System.Reflection;
    using global::System.Threading;
    using global::System.Threading.Tasks;
    using Net.System;
    using Net.Helper;
    using Net.Plugins;
    using global::System.Net;
    using Net.Event;

    /// <summary>
    /// Udp网络客户端
    /// 在安卓端必须设置可以后台运行, 如果不设置,当你按下home键后,app的所有线程将会被暂停,这会影响网络心跳检测线程,导致网络中断
    /// 解决方法 : 在android项目AndroidManifest.xml文件中的activity中添加如下内容：
    /// android:configChanges="fontScale|keyboard|keyboardHidden|locale|mnc|mcc|navigation|orientation|screenLayout|screenSize|smallestScreenSize|uiMode|touchscreen" 
    /// 详情请看此博文:https://www.cnblogs.com/nanwei/p/9125316.html
    /// 或这个博文: http://www.voidcn.com/article/p-yakpcmce-bpk.html
    /// </summary>
    [Serializable]
    public class UdpClient : ClientBase
    {
        public override int MTU { get => Gcp.MTU; set => Gcp.MTU = (ushort)value; }
        public override int RTO { get => Gcp.RTO; set => Gcp.RTO = value; }
        public override int MTPS { get => Gcp.MTPS; set => Gcp.MTPS = value; }
        public override FlowControlMode FlowControl { get => Gcp.FlowControl; set => Gcp.FlowControl = value; }
        public override Action<RTProgress> OnRevdRTProgress { get => Gcp.OnRevdProgress; set => Gcp.OnRevdProgress = value; }
        public override Action<RTProgress> OnSendRTProgress { get => Gcp.OnSendProgress; set => Gcp.OnSendProgress = value; }
        /// <summary>
        /// 构造udp可靠客户端
        /// </summary>
        public UdpClient() 
        {
            Gcp = new GcpKernel();
            Gcp.OnSender += (bytes) => {
                Send(NetCmd.ReliableTransport, bytes);
            };
        }

        /// <summary>
        /// 构造udp可靠客户端
        /// </summary>
        /// <param name="useUnityThread">使用unity多线程?</param>
        public UdpClient(bool useUnityThread) : this()
        {
            UseUnityThread = useUnityThread;
        }

        /// <summary>
        /// 获取p2p IP和端口, 通过client.OnP2PCallback事件回调
        /// </summary>
        /// <param name="uid"></param>
        public void GetP2P(int uid)
        {
            SendRT(NetCmd.P2P, BitConverter.GetBytes(uid));
        }

#if UDPTEST
        protected override void StartupThread()
        {
            base.StartupThread();
            Gcp.RemotePoint = Client.LocalEndPoint;
        }
        protected override void ReceiveHandle()
        {
        }
        internal void ReceiveTest(byte[] buffer)//本机测试
        {
            var segment = new Segment(buffer, false);
            receiveCount += segment.Count;
            receiveAmount++;
            heart = 0;
            ResolveBuffer(ref segment, false);
            revdLoopNum++;
        }
        internal void DataHandleTest(byte[] buffer)//本机测试
        {
            DataHandle(buffer);
        }
        protected override void SendByteData(byte[] buffer, bool reliable)
        {
            sendCount += buffer.Length;
            sendAmount++;
            if (buffer.Length <= 65507)
                (Net.Server.UdpServer.Instance as Net.Server.UdpServer).ReceiveTest(buffer, Client.LocalEndPoint); //Client.Send(buffer, 0, buffer.Length, SocketFlags.None);
            else
                NDebug.LogError("数据过大, 请使用SendRT发送...");
        }
#endif

        /// <summary>
        /// udp压力测试
        /// </summary>
        /// <param name="ip">服务器ip</param>
        /// <param name="port">服务器端口</param>
        /// <param name="clientLen">测试客户端数量</param>
        /// <param name="dataLen">每个客户端数据大小</param>
        public unsafe static CancellationTokenSource Testing(string ip, int port, int clientLen, int dataLen, int millisecondsTimeout, Action<UdpClientTest> onInit = null, Action<List<UdpClientTest>> fpsAct = null, IAdapter adapter = null)
        {
            var cts = new CancellationTokenSource();
            Task.Run(() =>
            {
                var clients = new List<UdpClientTest>();
                for (int i = 0; i < clientLen; i++) 
                {
                    var client = new UdpClientTest();
                    onInit?.Invoke(client);
                    if(adapter!=null)
                        client.AddAdapter(adapter);
                    client.Connect(ip,port);
                    clients.Add(client);
                }
                var buffer = new byte[dataLen];
                Task.Run(()=> 
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
                    Task.Run(()=> 
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

    public class UdpClientTest : UdpClient
    {
        public int fps;
        public int revdSize { get { return receiveCount; } }
        public int sendSize { get { return sendCount; } }
        public int sendNum { get { return sendAmount; } }
        public int revdNum { get { return receiveAmount; } }
        public int resolveNum { get { return receiveAmount; } }
        private byte[] addressBuffer;
        public UdpClientTest()
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
        protected unsafe override void SendByteData(byte[] buffer, bool reliable)
        {
            sendCount += buffer.Length;
            sendAmount++;
#if WINDOWS
            fixed (byte* ptr = buffer)
                Win32KernelAPI.sendto(Client.Handle, ptr, buffer.Length, SocketFlags.None, addressBuffer, 16);
#else
            Client.Send(buffer, 0, buffer.Length, SocketFlags.None);
#endif
        }
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
