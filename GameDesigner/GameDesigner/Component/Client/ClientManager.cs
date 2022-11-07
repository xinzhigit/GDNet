#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA || UNITY_WEBGL
namespace Net.Component
{
    using Net.Client;
    using Net.Event;
    using Net.Share;
    using global::System;
    using global::System.Threading;
    using UnityEngine;
    using global::System.Net;
    using global::System.Threading.Tasks;

    public enum TransportProtocol
    {
        Tcp, Gcp, Udx, Kcp, Web
    }

    public enum LogMode 
    {
        None,
        Default,
        LogAll,
        LogAndWarning
    }

    [DefaultExecutionOrder(1)]//在NetworkTransform组件之前执行OnDestroy，控制NetworkTransform处于Control模式时退出游戏会同步删除所有网络物体
    public class ClientManager : SingleCase<ClientManager>, ISendHandle
    {
        private bool mainInstance;
        private ClientBase _client;
        public TransportProtocol protocol = TransportProtocol.Tcp;
        public string ip = "127.0.0.1";
        public int port = 6666;
        public LogMode logMode = LogMode.Default;
        public bool debugRpc = true;
        public int frameRate = 60;
        public bool authorize;
        public bool startConnect = true;
        public bool md5CRC;

        public ClientBase client
        {
            get
            {
                if (_client == null)
                {
                    switch (protocol)
                    {
                        case TransportProtocol.Gcp:
                            {
#if UNITY_IOS
                                _client = new UdpClient(true);
#else
                                var type = typeof(ClientBase).Assembly.GetType("Net.Client.UdpClient");//当删减模块后解决报错问题
                                if (type != null)
                                    _client = Activator.CreateInstance(type, new object[] { true }) as ClientBase;
#endif
                            }
                            break;
                        case TransportProtocol.Tcp:
                            {
#if UNITY_IOS
                                _client = new TcpClient(true);
#else
                                var type = typeof(ClientBase).Assembly.GetType("Net.Client.TcpClient");
                                if (type != null)
                                    _client = Activator.CreateInstance(type, new object[] { true }) as ClientBase;
#endif
                            }
                            break;
                        case TransportProtocol.Kcp:
                            {
#if UNITY_IOS
                                _client = new KcpClient(true);
#else
                                var type = typeof(ClientBase).Assembly.GetType("Net.Client.KcpClient");
                                if (type != null)
                                    _client = Activator.CreateInstance(type, new object[] { true }) as ClientBase;
#endif
                            }
                            break;
                        case TransportProtocol.Udx:
                            {
#if UNITY_IOS
                                _client = new UdxClient(true);
#else
                                var type = typeof(ClientBase).Assembly.GetType("Net.Client.UdxClient");
                                if (type != null)
                                    _client = Activator.CreateInstance(type, new object[] { true }) as ClientBase;
#endif
                            }
                            break;
#if UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_WEBGL
                        case TransportProtocol.Web:
                            {
                                _client = new Client.WebClient(true);
                            }
                            break;
#endif
                    }
                    _client.host = ip;
                    _client.port = port;
                    _client.LogRpc = debugRpc;
                    _client.MD5CRC = md5CRC;
                }
                return _client;
            }
            set { _client = value; }
        }

        /// <summary>
        /// 客户端唯一标识
        /// </summary>
        public static string Identify { get { return Instance.client.Identify; } }
        /// <summary>
        /// 客户端唯一标识
        /// </summary>
        public static int UID { get { return Instance.client.UID; } }

        void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            mainInstance = true;
            DontDestroyOnLoad(gameObject);
            Application.targetFrameRate = frameRate;
            Application.runInBackground = true;
        }

        // Use this for initialization
        void Start()
        {
            switch (logMode)
            {
                case LogMode.Default:
                    NDebug.BindLogAll(Debug.Log, Debug.LogWarning, Debug.LogError);
                    break;
                case LogMode.LogAll:
                    NDebug.BindLogAll(Debug.Log);
                    break;
                case LogMode.LogAndWarning:
                    NDebug.BindLogAll(Debug.Log, Debug.Log, Debug.LogError);
                    break;
            }
            if (startConnect)
                Connect();
        }

        public Task<bool> Connect()
        {
            _client = client;
            var ips = Dns.GetHostAddresses(ip);
            if (ips.Length > 0)
                _client.host = ips[UnityEngine.Random.Range(0, ips.Length)].ToString();
            else
                _client.host = ip;
            _client.port = port;
            _client.AddRpcHandle(this);
            return _client.Connect(result =>
            {
                if (result)
                {
                    _client.Send(new byte[1]);//发送一个字节:调用服务器的OnUnClientRequest方法, 如果不需要账号登录, 则会直接允许进入服务器
                }
            });
        }

        // Update is called once per frame
        void Update()
        {
            _client.NetworkEventUpdate();
        }

        void OnDestroy()
        {
            if (mainInstance)
                _client.Close();
            switch (logMode)
            {
                case LogMode.Default:
                    NDebug.RemoveLogAll(Debug.Log, Debug.LogWarning, Debug.LogError);
                    break;
                case LogMode.LogAll:
                    NDebug.RemoveLogAll(Debug.Log);
                    break;
                case LogMode.LogAndWarning:
                    NDebug.RemoveLogAll(Debug.Log, Debug.Log, Debug.LogError);
                    break;
            }
        }

        /// <summary>
        /// 发起场景同步操作, 在同一个场景的所有客户端都会收到该操作参数operation
        /// </summary>
        /// <param name="operation"></param>
        public static void AddOperation(Operation operation)
        {
            Instance.client.AddOperation(operation);
        }

        public static void AddRpcHandler(object target)
        {
            I.client.AddRpcHandle(target);
        }

        /// <summary>
        /// 判断name是否是本地唯一id(本机玩家标识)
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal static bool IsLocal(string name)
        {
            if (Instance == null)
                return false;
            return instance._client.Identify == name;
        }

        /// <summary>
        /// 判断uid是否是本地唯一id(本机玩家标识)
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        internal static bool IsLocal(int uid)
        {
            if (Instance == null)
                return false;
            return instance._client.UID == uid;
        }

        public static void CallUnity(Action ptr)
        {
            I.client.WorkerQueue.Enqueue(ptr);
        }

#region 发送接口实现
        public void Send(byte[] buffer)
        {
            ((ISendHandle)_client).Send(buffer);
        }

        public void Send(byte cmd, byte[] buffer)
        {
            ((ISendHandle)_client).Send(cmd, buffer);
        }

        public void Send(string func, params object[] pars)
        {
            ((ISendHandle)_client).Send(func, pars);
        }

        public void Send(byte cmd, string func, params object[] pars)
        {
            ((ISendHandle)_client).Send(cmd, func, pars);
        }

        public void CallRpc(string func, params object[] pars)
        {
            ((ISendHandle)_client).CallRpc(func, pars);
        }

        public void CallRpc(byte cmd, string func, params object[] pars)
        {
            ((ISendHandle)_client).CallRpc(cmd, func, pars);
        }

        public void Request(string func, params object[] pars)
        {
            ((ISendHandle)_client).Request(func, pars);
        }

        public void Request(byte cmd, string func, params object[] pars)
        {
            ((ISendHandle)_client).Request(cmd, func, pars);
        }

        public void SendRT(string func, params object[] pars)
        {
            ((ISendHandle)_client).SendRT(func, pars);
        }

        public void SendRT(byte cmd, string func, params object[] pars)
        {
            ((ISendHandle)_client).SendRT(cmd, func, pars);
        }

        public void SendRT(byte[] buffer)
        {
            ((ISendHandle)_client).SendRT(buffer);
        }

        public void SendRT(byte cmd, byte[] buffer)
        {
            ((ISendHandle)_client).SendRT(cmd, buffer);
        }

        public void Send(string func, string funcCB, Delegate callback, params object[] pars)
        {
            ((ISendHandle)_client).Send(func, funcCB, callback, pars);
        }

        public void Send(string func, string funcCB, Delegate callback, int millisecondsDelay, params object[] pars)
        {
            ((ISendHandle)_client).Send(func, funcCB, callback, millisecondsDelay, pars);
        }

        public void Send(string func, string funcCB, Delegate callback, int millisecondsDelay, Action outTimeAct, params object[] pars)
        {
            ((ISendHandle)_client).Send(func, funcCB, callback, millisecondsDelay, outTimeAct, pars);
        }

        public void Send(byte cmd, string func, string funcCB, Delegate callback, int millisecondsDelay, Action outTimeAct, params object[] pars)
        {
            ((ISendHandle)_client).Send(cmd, func, funcCB, callback, millisecondsDelay, outTimeAct, pars);
        }

        public void SendRT(string func, string funcCB, Delegate callback, params object[] pars)
        {
            ((ISendHandle)_client).SendRT(func, funcCB, callback, pars);
        }

        public void SendRT(string func, string funcCB, Delegate callback, int millisecondsDelay, params object[] pars)
        {
            ((ISendHandle)_client).SendRT(func, funcCB, callback, millisecondsDelay, pars);
        }

        public void SendRT(string func, string funcCB, Delegate callback, int millisecondsDelay, Action outTimeAct, params object[] pars)
        {
            ((ISendHandle)_client).SendRT(func, funcCB, callback, millisecondsDelay, outTimeAct, pars);
        }

        public void SendRT(byte cmd, string func, string funcCB, Delegate callback, int millisecondsDelay, Action outTimeAct, params object[] pars)
        {
            ((ISendHandle)_client).SendRT(cmd, func, funcCB, callback, millisecondsDelay, outTimeAct, pars);
        }

        public void Send(byte cmd, string func, string funcCB, Delegate callback, int millisecondsDelay, Action outTimeAct, SynchronizationContext context, params object[] pars)
        {
            ((ISendHandle)_client).Send(cmd, func, funcCB, callback, millisecondsDelay, outTimeAct, context, pars);
        }

        public void SendRT(byte cmd, string func, string funcCB, Delegate callback, int millisecondsDelay, Action outTimeAct, SynchronizationContext context, params object[] pars)
        {
            ((ISendHandle)_client).SendRT(cmd, func, funcCB, callback, millisecondsDelay, outTimeAct, context, pars);
        }
#endregion
    }
}
#endif