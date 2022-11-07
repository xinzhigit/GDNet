#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA || UNITY_WEBGL
using Net.Client;
using Net.Event;
using Net.Share;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Threading.Tasks;
using System;

namespace Net.Component
{
    [Serializable]
    public class ClientGourp 
    {
        public string name;
        public ClientBase _client;
        public TransportProtocol protocol = TransportProtocol.Gcp;
        public string ip = "127.0.0.1";
        public int port = 6666;
        public bool localTest;//本机测试
        public bool debugRpc = true;
        public bool authorize;
        public bool startConnect = true;
        public bool md5CRC;
        [Header("序列化适配器")]
        public SerializeAdapterType type;
        public bool isEncrypt = false;//数据加密?

        public ClientBase Client
        {
            get
            {
                if (_client != null)
                    return _client;
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
                            var type = typeof(ClientBase).Assembly.GetType("Net.Client.WebClient");
                            if (type != null)
                                _client = Activator.CreateInstance(type, new object[] { true }) as ClientBase;
                        }
                        break;
#endif
                }
                _client.host = ip;
                _client.port = port;
                _client.LogRpc = debugRpc;
                _client.MD5CRC = md5CRC;
                return _client;
            }
            set { _client = value; }
        }

        public Task<bool> Connect()
        {
            _client = Client;
            if (!localTest)
            {
                var ips = Dns.GetHostAddresses(ip);
                if (ips.Length > 0)
                    _client.host = ips[RandomHelper.Range(0, ips.Length)].ToString();
                else
                    _client.host = ip;
            }
            else _client.host = "127.0.0.1";
            _client.port = port;
            switch (type)
            {
                case SerializeAdapterType.Default:
                    break;
                case SerializeAdapterType.PB_JSON_FAST:
                    _client.AddAdapter(new Adapter.SerializeFastAdapter() { isEncrypt = isEncrypt });
                    break;
                case SerializeAdapterType.Binary:
                    _client.AddAdapter(new Adapter.SerializeAdapter() { isEncrypt = isEncrypt });
                    break;
                case SerializeAdapterType.Binary2:
                    _client.AddAdapter(new Adapter.SerializeAdapter2() { isEncrypt = isEncrypt });
                    break;
                case SerializeAdapterType.Binary3:
                    _client.AddAdapter(new Adapter.SerializeAdapter3() { isEncrypt = isEncrypt });
                    break;
            }
            return _client.Connect(result =>
            {
                if (result)
                {
                    _client.Send(new byte[1]);//发送一个字节:调用服务器的OnUnClientRequest方法, 如果不需要账号登录, 则会直接允许进入服务器
                }
            });
        }

        public Task<bool> Connect(string ip, int port)
        {
            this.ip = ip;
            this.port = port;
            return Connect();
        }
    }

    public class NetworkManager : SingleCase<NetworkManager>
    {
        public LogMode logMode = LogMode.Default;
#if UNITY_2020_1_OR_NEWER
        [NonReorderable]
#endif
        public List<ClientGourp> clients = new List<ClientGourp>();

        public ClientBase this[int index]
        {
            get { return clients[index].Client; }
            set { clients[index].Client = value; }
        }

        void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
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
            foreach (var client in clients)
            {
                if (client.startConnect)
                    client.Connect();
            }
        }

        // Update is called once per frame
        void Update()
        {
            for (int i = 0; i < clients.Count; i++)
            {
                if (clients[i]._client == null)
                    continue;
                clients[i]._client.NetworkEventUpdate();
            }
        }

        void OnDestroy()
        {
            for (int i = 0; i < clients.Count; i++)
            {
                if (clients[i]._client == null)
                    continue;
                clients[i]._client.Close();
            }
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

        public static void BindNetworkAllHandle(INetworkHandle handle)
        {
            foreach (var item in I.clients)
            {
                item.Client.BindNetworkHandle(handle);
            }
        }

        public static void AddRpcHandle(object target)
        {
            foreach (var item in I.clients)
            {
                item.Client.AddRpcHandle(target);
            }
        }

        public static void RemoveRpc(object target)
        {
            foreach (var item in I.clients)
            {
                item.Client.RemoveRpc(target);
            }
        }

        public static void Close(bool v1, int v2)
        {
            foreach (var item in I.clients)
            {
                item.Client.Close(v1, v2);
            }
        }

        public static void CallUnity(Action ptr)
        {
            I.clients[0].Client.WorkerQueue.Enqueue(ptr);
        }

        public static void DispatcherRpc(ushort hash, params object[] parms)
        {
            I.clients[1].Client.DispatchRpc(hash, parms);
        }
    }
}
#endif