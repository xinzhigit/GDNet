namespace Net.Client
{
    using Net.Event;
    using Net.Share;
    using global::System;
    using global::System.Collections.Generic;
    using global::System.IO;
    using global::System.Net;
    using global::System.Net.Sockets;
    using global::System.Threading;
    using global::System.Threading.Tasks;
    using Net.System;
    using global::System.Security.Cryptography;
    using Net.Helper;

    /// <summary>
    /// TCP客户端类型 
    /// 第三版本 2020.9.14
    /// </summary>
    [Serializable]
    public class TcpClient : ClientBase
    {
        /// <summary>
        /// tcp数据长度(4) + 1CRC协议 = 5
        /// </summary>
        protected override int frame { get; set; } = 5;
        public override bool MD5CRC
        {
            get => md5crc;
            set
            {
                md5crc = value;
                if (value)
                    frame = 5 + 16;
                else
                    frame = 5;
            }
        }
        public override int HeartInterval { get; set; } = 1000;
        public override byte HeartLimit { get; set; } = 5;

        /// <summary>
        /// 构造可靠传输客户端
        /// </summary>
        public TcpClient()
        {
        }

        /// <summary>
        /// 构造不可靠传输客户端
        /// </summary>
        /// <param name="useUnityThread">使用unity多线程?</param>
        public TcpClient(bool useUnityThread) : this()
        {
            UseUnityThread = useUnityThread;
        }

        ~TcpClient()
        {
#if !UNITY_EDITOR
            Close();
#endif
        }

        protected override Task<bool> ConnectResult(string host, int port, int localPort, Action<bool> result)
        {
            return Task.Run(() =>
            {
                try
                {
                    Client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    this.localPort = localPort;
                    Client.NoDelay = true;
                    if (localPort != -1)
                        Client.Bind(new IPEndPoint(IPAddress.Any, localPort));
                    Client.Connect(host, port);
                    var tick = Environment.TickCount + 8000;
                    while (UID == 0)
                    {
                        Receive(true);
                        if (Environment.TickCount >= tick)
                            throw new Exception("uid赋值失败!连接超时处理");
                        if (!openClient)
                            throw new Exception("客户端调用Close!");
                    }
                    StackStream = new MemoryStream(Net.Config.Config.BaseCapacity);//BufferStreamShare.Take();
                    StartupThread();
                    InvokeContext(() => {
                        networkState = !openClient ? NetworkState.ConnectClosed : NetworkState.Connected;
                        result(true);
                    });
                    return true;
                }
                catch(Exception ex)
                {
                    NDebug.LogError("连接错误:" + ex);
                    Connected = false;
                    Client?.Close();
                    Client = null;
                    InvokeContext(() => {
                        networkState = !openClient ? NetworkState.ConnectClosed : NetworkState.ConnectFailed;
                        result(false);
                    });
                    return false;
                }
            });
        }

        protected override bool HeartHandler()
        {
            try
            {
                heart++;
                if (heart <= HeartLimit)
                    return true;
                if (!Connected)
                    Reconnection();
                else
                    Send(NetCmd.SendHeartbeat, new byte[0]);
            }
            catch
            {
            }
            return openClient & CurrReconnect < ReconnectCount;
        }

        protected override void StartupThread()
        {
            AbortedThread();//断线重连处理
            Connected = true;
            StartThread("ReceiveHandle", ReceiveHandle);
            //checkRpcHandleID = ThreadManager.Invoke("CheckRpcHandle", CheckRpcHandle);
            networkFlowHandlerID = ThreadManager.Invoke("NetworkFlowHandler", 1f, NetworkFlowHandler);
            heartHandlerID = ThreadManager.Invoke("HeartHandler", HeartInterval, HeartHandler);
            syncVarHandlerID = ThreadManager.Invoke("SyncVarHandler", SyncVarHandler);
            sendHandlerID = ThreadManager.Invoke("SendHandler", SendInterval, SendDataHandler);
            if (!UseUnityThread)
                updateHandlerID = ThreadManager.Invoke("UpdateHandle", UpdateHandler);
        }

        protected override void SendRTDataHandle()
        {
            SendDataHandle(rtRPCModels, true);
        }

        protected override byte[] PackData(Segment stream)
        {
            stream.Flush();
            if (MD5CRC)
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                var head = frame;
                byte[] retVal = md5.ComputeHash(stream, head, stream.Count - head);
                EncryptHelper.ToEncrypt(Password, retVal);
                int len = stream.Count - head;
                var lenBytes = BitConverter.GetBytes(len);
                byte crc = CRCHelper.CRC8(lenBytes, 0, lenBytes.Length);
                stream.Position = 0;
                stream.Write(lenBytes, 0, 4);
                stream.WriteByte(crc);
                stream.Write(retVal, 0, retVal.Length);
                stream.Position = len + head;
            }
            else 
            {
                int len = stream.Count - frame;
                var lenBytes = BitConverter.GetBytes(len);
                byte retVal = CRCHelper.CRC8(lenBytes, 0, lenBytes.Length);
                stream.Position = 0;
                stream.Write(lenBytes, 0, 4);
                stream.WriteByte(retVal);
                stream.Position = len + frame;
            }
            return stream.ToArray();
        }
#if TEST1
        ListSafe<byte> list = new ListSafe<byte>();
#endif
        protected override void SendByteData(byte[] buffer, bool reliable)
        {
            sendCount += buffer.Length;
            sendAmount++;
            if (Client.Poll(1, SelectMode.SelectWrite))
            {
#if TEST1
                list.AddRange(buffer);
                do
                {
                    var buffer1 = list.GetRemoveRange(0, RandomHelper.Range(0, buffer.Length));
                    Net.Server.TcpServer.Instance.ReceiveTest(buffer1);
                }
                while (rtRPCModels.Count == 0 & list.Count > 0);
#else
                int count = Client.Send(buffer, 0, buffer.Length, SocketFlags.None);
                if (count <= 0)
                    OnSendErrorHandle?.Invoke(buffer, reliable);
                else if (count != buffer.Length)
                    NDebug.Log($"发送了{buffer.Length - count}个字节失败!");
#endif
            }
            else
            {
                NDebug.Log("缓冲区已满,等待接收中!");
            }
        }

        protected override void ResolveBuffer(ref Segment buffer, bool isTcp)
        {
#if TEST1
            if (StackStream == null)
                StackStream = BufferStreamShare.Take();
#endif
            heart = 0;
            //if (stack > StackNumberMax)//不能一直叠包
            //{
            //    stack = 0;
            //    NDebug.LogError($"请设置StackNumberMax属性, 叠包次数过高, 叠包数量达到{StackNumberMax}次以上...");
            //    return;
            //}
            if (stack > 0)
            {
                stack++;
                StackStream.Seek(stackIndex, SeekOrigin.Begin);
                int size = buffer.Count - buffer.Position;
                stackIndex += size;
                StackStream.Write(buffer, buffer.Position, size);
                if (stackIndex < stackCount)
                {
                    InvokeRevdRTProgress(stackIndex, stackCount);
                    return;
                }
                var count = (int)StackStream.Position;//.Length; //错误问题,不能用length, 这是文件总长度, 之前可能已经有很大一波数据
                BufferPool.Push(buffer);//要回收掉, 否则会提示内存泄露
                buffer = BufferPool.Take(count);//ref 才不会导致提示内存泄露
                StackStream.Seek(0, SeekOrigin.Begin);
                StackStream.Read(buffer, 0, count);
                buffer.Count = count;
            }
            while (buffer.Position < buffer.Count)
            {
                if (buffer.Position + 5 > buffer.Count)//流数据偶尔小于frame头部字节
                {
                    var position = buffer.Position;
                    var count = buffer.Count - position;
                    stackIndex = count;
                    stackCount = 0;
                    StackStream.Seek(0, SeekOrigin.Begin);
                    StackStream.Write(buffer, position, count);
                    stack++;
                    break;
                }
                var lenBytes = buffer.Read(4);
                byte crcCode = buffer.ReadByte();//CRC检验索引
                byte retVal = CRCHelper.CRC8(lenBytes, 0, 4);
                if (crcCode != retVal)
                {
                    stack = 0;
                    NDebug.LogError($"[{UID}]CRC校验失败!");
                    return;
                }
                int size = BitConverter.ToInt32(lenBytes, 0);
                if (size < 0 | size > PackageSize)//如果出现解析的数据包大小有问题，则不处理
                {
                    stack = 0;
                    NDebug.LogError($"[{UID}]数据被拦截修改或数据量太大: size:{size}，如果想传输大数据，请设置PackageSize属性");
                    return;
                }
                int value = MD5CRC ? 16 : 0;
                if (buffer.Position + size + value <= buffer.Count)
                {
                    stack = 0;
                    var count = buffer.Count;//此长度可能会有连续的数据(粘包)
                    buffer.Count = buffer.Position + value + size;//需要指定一个完整的数据长度给内部解析
                    base.ResolveBuffer(ref buffer, true);
                    buffer.Count = count;//解析完成后再赋值原来的总长
                }
                else
                {
                    var position = buffer.Position - 5;
                    var count = buffer.Count - position;
                    stackIndex = count;
                    stackCount = size;
                    StackStream.Seek(0, SeekOrigin.Begin);
                    StackStream.Write(buffer, position, count);
                    stack++;
                    break;
                }
            }
        }

        public override void Close(bool await = true, int millisecondsTimeout = 1000)
        {
            Connected = false;
            openClient = false;
            NetworkState = networkState = NetworkState.ConnectClosed;
            if (await) Thread.Sleep(millisecondsTimeout);//给update线程一秒的时间处理关闭事件
            AbortedThread();
            Client?.Close();
            Client = null;
            StackStream?.Close();
            StackStream = null;
            stack = 0;
            stackIndex = 0;
            stackCount = 0;
            UID = 0;
            CurrReconnect = 0;
            if (Instance == this) Instance = null;
            if (Gcp != null) Gcp.Dispose();
            NDebug.Log("客户端已关闭！");
        }

        /// <summary>
        /// tcp压力测试
        /// </summary>
        /// <param name="ip">服务器ip</param>
        /// <param name="port">服务器端口</param>
        /// <param name="clientLen">测试客户端数量</param>
        /// <param name="dataLen">每个客户端数据大小</param>
        public static CancellationTokenSource Testing(string ip, int port, int clientLen, int dataLen, int millisecondsTimeout, Action<TcpClientTest> onInit = null, Action<List<TcpClientTest>> fpsAct = null, IAdapter adapter = null)
        {
            var cts = new CancellationTokenSource();
            Task.Run(() =>
            {
                var clients = new List<TcpClientTest>();
                for (int i = 0; i < clientLen; i++)
                {
                    var client = new TcpClientTest();
                    onInit?.Invoke(client);
                    if(adapter != null)
                        client.AddAdapter(adapter);
                    try { 
                        client.Connect(ip, port);
                    } catch (Exception ex) {
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
                        var timer = new TimerTick();
                        while (!cts.IsCancellationRequested)
                        {
                            bool canSend = false;
                            var tick = (uint)Environment.TickCount;
                            if (timer.CheckTimeout(tick, (uint)millisecondsTimeout, true))
                            {
                                canSend = true;
                            }
                            for (int ii = index; ii < end; ii++)
                            {
                                try
                                {
                                    var client = clients[ii];
                                    if (client.Client == null)
                                        continue;
                                    if (!client.Client.Connected)
                                        continue;
                                    if (client.Client.Poll(0, SelectMode.SelectRead))
                                    {
                                        var buffer1 = BufferPool.Take(65535);
                                        buffer1.Count = client.Client.Receive(buffer1, 0, ushort.MaxValue, SocketFlags.None, out var errorCode);
                                        if(errorCode == SocketError.Success)
                                            client.ResolveBuffer(ref buffer1, false);
                                        BufferPool.Push(buffer1);
                                    }
                                    if (canSend)
                                    {
                                        client.SendRT(NetCmd.Local, buffer);
                                        //client.AddOperation(new Operation(66, buffer));
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

    public class TcpClientTest : TcpClient
    {
        public int fps;
        public int revdSize { get { return receiveCount; } }
        public int sendSize { get { return sendCount; } }
        public int sendNum { get { return sendAmount; } }
        public int revdNum { get { return receiveAmount; } }
        public int resolveNum { get { return receiveAmount; } }

        public TcpClientTest()
        {
            OnRevdBufferHandle += (model) => { fps++; };
            OnOperationSync += (list) => { fps++; };
        }
        protected override Task<bool> ConnectResult(string host, int port, int localPort, Action<bool> result)
        {
            Client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.localPort = localPort;
            Client.Connect(host, port);
            Client.Blocking = false;
            Client.NoDelay = true;
            rPCModels.Enqueue(new RPCModel(NetCmd.Connect, new byte[0]));
            SendDirect();
            Connected = true;
            StackStream = new MemoryStream(Net.Config.Config.BaseCapacity);//BufferStreamShare.Take();
            return Task.FromResult(Connected);
        }
        protected override void StartupThread() { }

        protected override void OnConnected(bool result) { }

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
                Win32KernelAPI.send(Client.Handle, ptr, buffer.Length, SocketFlags.None);
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
        public override string ToString()
        {
            return $"uid:{Identify} conv:{Connected}";
        }
    }
}