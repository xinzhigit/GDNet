namespace Net.Client
{
    using Net.Event;
    using Net.Share;
    using global::System;
    using global::System.Collections.Concurrent;
    using global::System.Collections.Generic;
    using global::System.IO;
    using global::System.Net.Sockets;
    using global::System.Runtime.InteropServices;
    using global::System.Threading;
    using global::System.Threading.Tasks;
    using Udx;
    using Net.System;

    /// <summary>
    /// udx客户端类型 -> 只能300人以下连接, 如果想要300个客户端以上, 请进入udx网址:www.goodudx.com 联系作者下载专业版FastUdxApi.dll, 然后更换下框架内的FastUdxApi.dll即可
    /// 第三版本 2020.9.14
    /// </summary>
    [Serializable]
    public class UdxClient : ClientBase, IUDX
    {
        protected IntPtr udxObj;
        public IntPtr ClientPtr { get; private set; }
        protected UDXPRC uDXPRC;

        /// <summary>
        /// 构造可靠传输客户端
        /// </summary>
        public UdxClient()
        {
        }

        /// <summary>
        /// 构造可靠传输客户端
        /// </summary>
        /// <param name="useUnityThread">使用unity多线程?</param>
        public UdxClient(bool useUnityThread) : this()
        {
            UseUnityThread = useUnityThread;
        }

        ~UdxClient()
        {
#if !UNITY_EDITOR
            Close();
#endif
        }

        public override Task<bool> Connect(string host, int port, int localPort, Action<bool> result)
        {
            if (!UdxLib.INIT)
            {
                UdxLib.INIT = true;
#if !UNITY_EDITOR && !UNITY_STANDALONE && !UNITY_ANDROID && !UNITY_IOS
                string path = AppDomain.CurrentDomain.BaseDirectory;
                if (!File.Exists(path + "\\FastUdxApi.dll"))
                    throw new FileNotFoundException($"FastUdxApi.dll没有在程序根目录中! 请从GameDesigner文件夹下找到 FastUdxApi.dll复制到{path}目录下.");
#endif
                UdxLib.UInit(1);
                UdxLib.UEnableLog(false);
            }
            UdxLib.UDXS.Add(this);
            return base.Connect(host, port, localPort, result);
        }

        protected override Task<bool> ConnectResult(string host, int port, int localPort, Action<bool> result)
        {
            try
            {
                udxObj = UdxLib.UCreateFUObj();
                UdxLib.UBind(udxObj, null, 0);
                uDXPRC = new UDXPRC(ProcessReceive);
                UdxLib.USetFUCB(udxObj, uDXPRC);
                GC.KeepAlive(uDXPRC);
                string host1 = host;
                if (host == "127.0.0.1")
                    host1 = Server.NetPort.GetIP();
                UdxLib.UConnect(udxObj, host1, port, 0, false, 0);
                return Task.Run(() =>
                {
                    var timeout = DateTime.Now.AddSeconds(5);
                    while (!Connected & DateTime.Now < timeout) { Thread.Sleep(1); }
                    if (Connected)
                        StartupThread();
                    timeout = DateTime.Now.AddSeconds(3);
                    while (Connected & UID == 0 & DateTime.Now < timeout)
                    {
                        Send(NetCmd.Identify);
                        Thread.Sleep(200);
                        if (!openClient)
                            throw new Exception("客户端调用Close!");
                    }
                    if (UID == 0)
                        throw new Exception("uid赋值失败!");
                    InvokeContext(() => {
                        networkState = Connected ? NetworkState.Connected : NetworkState.ConnectFailed;
                        result(Connected); 
                    });
                    return Connected;
                });
            }
            catch (Exception ex)
            {
                NDebug.Log("连接错误: " + ex.ToString());
                networkState = NetworkState.ConnectFailed;
                result(false);
                return Task.FromResult(false);
            }
        }

        protected override void StartupThread()
        {
            AbortedThread();//断线重连处理
            Connected = true;
            //checkRpcHandleID = ThreadManager.Invoke("CheckRpcHandle", CheckRpcHandle);
            networkFlowHandlerID = ThreadManager.Invoke("NetworkFlowHandler", 1f, NetworkFlowHandler);
            heartHandlerID = ThreadManager.Invoke("HeartHandler", HeartInterval, HeartHandler);
            syncVarHandlerID = ThreadManager.Invoke("SyncVarHandler", SyncVarHandler);
            sendHandlerID = ThreadManager.Invoke("SendHandler", SendInterval, SendDataHandler);
            if (!UseUnityThread)
                updateHandlerID = ThreadManager.Invoke("UpdateHandle", UpdateHandler);
        }

        protected void ProcessReceive(UDXEVENT_TYPE type, int erro, IntPtr cli, IntPtr pData, int len)//cb回调
        {
            try
            {
                heart = 0;
                switch (type)
                {
                    case UDXEVENT_TYPE.E_CONNECT:
                        if (erro != 0)
                            return;
                        ClientPtr = cli;
                        UdxLib.UDump(cli);
                        Connected = true;
                        UdxLib.USetGameMode(cli, true);
                        break;
                    case UDXEVENT_TYPE.E_LINKBROKEN:
                        Connected = false;
                        NetworkState = networkState = NetworkState.ConnectLost;
                        rtRPCModels = new QueueSafe<RPCModel>();
                        rPCModels = new QueueSafe<RPCModel>();
                        NDebug.Log("断开连接！");
                        break;
                    case UDXEVENT_TYPE.E_DATAREAD:
                        var buffer = BufferPool.Take(len);
                        buffer.Count = len;
                        Marshal.Copy(pData, buffer, 0, len);
                        receiveCount += len;
                        receiveAmount++;
                        ResolveBuffer(ref buffer, false);
                        BufferPool.Push(buffer);
                        break;
                }
            }
            catch (Exception ex)
            {
                NDebug.LogError("处理异常:" + ex);
            }
        }

        protected override bool HeartHandler()
        {
            try
            {
                heart++;
                if (heart <= HeartLimit)
                    return true;
                if (Connected & heart < HeartLimit + 5)
                    Send(NetCmd.SendHeartbeat, new byte[0]);
                else if (!Connected)//尝试连接执行
                    Reconnection();
            }
            catch { }
            return openClient & CurrReconnect < 10;
        }

        protected override void SendRTDataHandle()
        {
            SendDataHandle(rtRPCModels, true);
        }

        protected unsafe override void SendByteData(byte[] buffer, bool reliable)
        {
            if (ClientPtr == IntPtr.Zero)
                return;
            sendCount += buffer.Length;
            sendAmount++;
            fixed (byte* ptr = buffer) 
            {
                int count = UdxLib.USend(ClientPtr, ptr, buffer.Length);
                if (count <= 0)
                    OnSendErrorHandle?.Invoke(buffer, reliable);
            }
        }

        public override void Close(bool await = true, int millisecondsTimeout = 1000)
        {
            Connected = false;
            openClient = false;
            NetworkState = networkState = NetworkState.ConnectClosed;
            if (await) Thread.Sleep(millisecondsTimeout);//给update线程一秒的时间处理关闭事件
            AbortedThread();
            StackStream?.Close();
            StackStream = null;
            stack = 0;
            UID = 0;
            CurrReconnect = 0;
            if (Instance == this) Instance = null;
            if (Gcp != null) Gcp.Dispose();
            if (ClientPtr != IntPtr.Zero)
            {
                UdxLib.UClose(ClientPtr);
                UdxLib.UUndump(ClientPtr);
                ClientPtr = IntPtr.Zero;
            }
            if (udxObj != IntPtr.Zero)
            {
                UdxLib.UDestroyFUObj(udxObj);
                udxObj = IntPtr.Zero;
            }
            UdxLib.UDXS.Remove(this);
            if (UdxLib.UDXS.Count == 0 & UdxLib.INIT)
            {
                UdxLib.UUnInit();
                UdxLib.INIT = false;
            }
            NDebug.Log("客户端已关闭！");
        }

        public static CancellationTokenSource Testing(string ip, int port, int clientLen, int dataLen, int millisecondsTimeout, Action<UdxClientTest> onInit = null, Action<List<UdxClientTest>> fpsAct = null, IAdapter adapter = null)
        {
            if (!UdxLib.INIT)
            {
                UdxLib.INIT = true;
#if !UNITY_EDITOR && !UNITY_STANDALONE && !UNITY_ANDROID && !UNITY_IOS
                string path = AppDomain.CurrentDomain.BaseDirectory;
                if (!File.Exists(path + "\\FastUdxApi.dll"))
                    throw new FileNotFoundException($"FastUdxApi.dll没有在程序根目录中! 请从GameDesigner文件夹下找到 FastUdxApi.dll复制到{path}目录下.");
#endif
                UdxLib.UInit(8);
                UdxLib.UEnableLog(false);
            }
            var cts = new CancellationTokenSource();
            Task.Run(() =>
            {
                var clients = new List<UdxClientTest>();
                for (int i = 0; i < clientLen; i++)
                {
                    var client = new UdxClientTest();
                    onInit?.Invoke(client);
                    if (adapter != null)
                        client.AddAdapter(adapter);
                    try
                    {
                        client.Connect(ip, port);
                    }
                    catch (Exception ex)
                    {
                        NDebug.LogError(ex);
                        return;
                    }
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
                                        client.AddOperation(new Operation(66, buffer));
                                        client.SendDirect();
                                    }
                                    client.NetworkEventUpdate();
                                }
                                catch (Exception ex)
                                {
                                    NDebug.LogError(ex);
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

    public class UdxClientTest : UdxClient
    {
        public int fps;
        public int revdSize { get { return receiveCount; } }
        public int sendSize { get { return sendCount; } }
        public int sendNum { get { return sendAmount; } }
        public int revdNum { get { return receiveAmount; } }
        public int resolveNum { get { return receiveAmount; } }

        public UdxClientTest()
        {
            OnRevdBufferHandle += (model) => { fps++; };
            OnOperationSync += (list) => { fps++; };
        }
        protected override Task<bool> ConnectResult(string host, int port, int localPort, Action<bool> result)
        {
            udxObj = UdxLib.UCreateFUObj();
            UdxLib.UBind(udxObj, null, 0);
            uDXPRC = new UDXPRC(ProcessReceive);
            UdxLib.USetFUCB(udxObj, uDXPRC);
            GC.KeepAlive(uDXPRC);
            string host1 = host;
            if (host == "127.0.0.1")
                host1 = Server.NetPort.GetIP();
            UdxLib.UConnect(udxObj, host1, port, 0, false, 0);
            return Task.FromResult(true);
        }
        protected override void StartupThread() { }

        protected override void OnConnected(bool result) { }

        protected unsafe override void SendByteData(byte[] buffer, bool reliable)
        {
            base.SendByteData(buffer, reliable);
        }
        public override string ToString()
        {
            return $"uid:{Identify} conv:{Connected}";
        }
    }
}