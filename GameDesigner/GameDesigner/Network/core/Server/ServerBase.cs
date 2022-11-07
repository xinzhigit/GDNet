/*版权所有（C）GDNet框架
*
*该软件按“原样”提供，不提供任何形式的明示或暗示担保，
*无论是由于软件，使用或其他方式产生的，侵权或其他形式的任何索赔，损害或其他责任，作者或版权所有者概不负责。
*
*允许任何人出于任何目的使用本框架，
*包括商业应用程序，并对其进行修改和重新发布自由
*
*受以下限制：
*
*  1. 不得歪曲本软件的来源；您不得
*声称是你写的原始软件。如果你用这个框架
*在产品中，产品文档中要确认感谢。
*  2. 更改的源版本必须清楚地标记来源于GDNet框架，并且不能
*被误传为原始软件。
*  3. 本通知不得从任何来源分发中删除或更改。
*/
namespace Net.Server
{
    using Net.Share;
    using global::System;
    using global::System.Collections.Concurrent;
    using global::System.Collections.Generic;
    using global::System.IO;
    using global::System.Linq;
    using global::System.Net;
    using global::System.Net.Sockets;
    using global::System.Reflection;
    using global::System.Text;
    using global::System.Threading;
    using global::System.Threading.Tasks;
    using Debug = Event.NDebug;
    using Net.System;
    using Net.Serialize;
    using Net.Helper;
    using global::System.Security.Cryptography;
    using Net.Plugins;
    using Microsoft.Win32;
    using Net.Event;

    /// <summary>
    /// 网络服务器核心基类 2019.11.22
    /// <para>Player:当有客户端连接服务器就会创建一个Player对象出来, Player对象和XXXClient是对等端, 每当有数据处理都会通知Player对象. </para>
    /// <para>Scene:你可以定义自己的场景类型, 比如帧同步场景处理, mmorpg场景什么处理, 可以重写Scene的Update等等方法实现每个场景的更新和处理. </para>
    /// </summary>
    public abstract class ServerBase<Player, Scene> : IServerHandle<Player, Scene> where Player : NetPlayer, new() where Scene : NetScene<Player>, new()
    {
        #region 属性
        /// <summary>
        /// (分布式)服务器名称
        /// </summary>
        public string Name { get; set; } = "GDNet";
        /// <summary>
        /// 分布式(集群)服务器区域名称
        /// </summary>
        public string AreaName { get; set; } = "电信1区";
        /// <summary>
        /// 服务器套接字
        /// </summary>
        public Socket Server { get; protected set; }
        /// <summary>
        /// io完成端口对象
        /// </summary>
        public SocketAsyncEventArgs ServerArgs { get; protected set; }
        /// <summary>
        /// 远程方法优化字典
        /// </summary>
        public MyDictionary<string, MyDictionary<object, IRPCMethod>> RpcDic { get; set; } = new MyDictionary<string, MyDictionary<object, IRPCMethod>>();
        /// <summary>
        /// 远程方法哈希字典
        /// </summary>
        public MyDictionary<ushort, MyDictionary<object, IRPCMethod>> RpcHashDic { get; set; } = new MyDictionary<ushort, MyDictionary<object, IRPCMethod>>();
        /// <summary>
        /// 已经收集过的类信息
        /// </summary>
        public MyDictionary<Type, List<MemberData>> MemberInfos { get; set; } = new MyDictionary<Type, List<MemberData>>();
        /// <summary>
        /// 当前收集rpc的对象信息
        /// </summary>
        public MyDictionary<object, MemberDataList> RpcTargetHash { get; set; } = new MyDictionary<object, MemberDataList>();
        /// <summary>
        /// 字段同步信息
        /// </summary>
        public MyDictionary<ushort, SyncVarInfo> SyncVarDic { get; set; } = new MyDictionary<ushort, SyncVarInfo>();
        /// <summary>
        /// 可等待异步的Rpc
        /// </summary>
        public ConcurrentDictionary<string, RPCModelTask> RpcTasks { get; set; } = new ConcurrentDictionary<string, RPCModelTask>();
        /// <summary>
        /// 可等待异步的Rpc
        /// </summary>
        public ConcurrentDictionary<ushort, RPCModelTask> RpcTasks1 { get; set; } = new ConcurrentDictionary<ushort, RPCModelTask>();
        /// <summary>
        /// Rpc任务队列
        /// </summary>
        public QueueSafe<IRPCData> RpcWorkQueue { get; set; } = new QueueSafe<IRPCData>();
        /// <summary>
        /// 登录的客户端 与<see cref="UIDClients"/>为互助字典 所添加的键值为<see cref="NetPlayer.PlayerID"/>
        /// </summary>
        public ConcurrentDictionary<string, Player> Players { get; private set; } = new ConcurrentDictionary<string, Player>();
        /// <summary>
        /// 登录的客户端 与<see cref="Players"/>为互助字典 所添加的键值为<see cref="NetPlayer.UserID"/>
        /// </summary>
        public ConcurrentDictionary<int, Player> UIDClients { get; private set; } = new ConcurrentDictionary<int, Player>();
        /// <summary>
        /// 所有客户端列表
        /// </summary>
        public ConcurrentDictionary<EndPoint, Player> AllClients { get; private set; } = new ConcurrentDictionary<EndPoint, Player>();
        /// <summary>
        /// 所有在线的客户端
        /// </summary>
        public List<Player> Clients
        {
            get
            {
                var unclients = new List<Player>();
                foreach (var client in AllClients.Values)
                    if (client.Login) unclients.Add(client);
                return unclients;
            }
        }
        /// <summary>
        /// 未知客户端连接 或 刚连接服务器还未登录账号的IP
        /// </summary>
        public List<Player> UnClients
        {
            get
            {
                var unclients = new List<Player>();
                foreach (var client in AllClients.Values)
                    if (!client.Login) unclients.Add(client);
                return unclients;
            }
        }
        /// <summary>
        /// 服务器场景，key是场景名或房间名，关卡名。 value是(场景或房间，关卡等)对象
        /// </summary>
        public ConcurrentDictionary<string, Scene> Scenes { get; set; } = new ConcurrentDictionary<string, Scene>();
        /// <summary>
        /// 网络服务器单例
        /// </summary>
        public static ServerBase<Player, Scene> Instance { get; protected set; }
        /// <summary>
        /// 当前玩家在线人数
        /// </summary>
        public int OnlinePlayers { get { return UIDClients.Count; } }
        /// <summary>
        /// 未知客户端人数, 即在线不登录账号的客户端
        /// </summary>
        public int UnClientNumber { get { return AllClients.Count - UIDClients.Count; } }
        /// <summary>
        /// 服务器端口
        /// </summary>
        public ushort Port { get; protected set; }
        /// <summary>
        /// 服务器是否处于运行状态, 如果服务器套接字已经被释放则返回False, 否则返回True. 当调用Close方法后将改变状态
        /// </summary>
        public bool IsRunServer { get; set; }
        /// <summary>
        /// 网络场景同步时间(帧同步间隔), 默认每33毫秒同步一次, 一秒同步30次, 可自己设置
        /// </summary>
        public int SyncSceneTime { get; set; } = 33;
        /// <summary>
        /// 获取或设置最大可排队人数， 如果未知客户端人数超出LineUp值将不处理超出排队的未知客户端数据请求 ， 默认排队5000人
        /// </summary>
        public int LineUp { get; set; } = 5000;
        /// <summary>
        /// 允许玩家在线人数最大值（玩家在线上限）默认2000人同时在线
        /// </summary>
        public int OnlineLimit { get; set; } = 2000;
        /// <summary>
        /// 服务器主场景名称
        /// </summary>
        public string MainSceneName { get; protected set; } = "MainScene";
        /// <summary>
        /// 网络统计发送数据长度/秒
        /// </summary>
        protected int sendCount;
        /// <summary>
        /// 网络统计发送次数/秒
        /// </summary>
        protected int sendAmount;
        /// <summary>
        /// 网络统计解析次数/秒
        /// </summary>
        protected int resolveAmount;
        /// <summary>
        /// 网络统计接收次数/秒
        /// </summary>
        protected int receiveAmount;
        /// <summary>
        /// 网络统计接收长度/秒
        /// </summary>
        protected int receiveCount;
        /// <summary>
        /// 发送线程循环次数 并发数,类似fps
        /// </summary>
        protected int sendLoopNum;
        /// <summary>
        /// 接收线程循环次数(FPS)
        /// </summary>
        protected int revdLoopNum;
        /// <summary>
        /// 从启动到现在总流出的数据流量
        /// </summary>
        protected long outflowTotal;
        /// <summary>
        /// 从启动到现在总流入的数据流量
        /// </summary>
        protected long inflowTotal;
        /// <summary>
        /// 1CRC协议
        /// </summary>
        protected virtual byte frame { get; set; } = 1;
        /// <summary>
        /// 每个客户端接收缓存最大的数据长度 默认可缓存5242880(5M)的数据长度
        /// </summary>
        //public int StackBufferSize { get; set; } = 5242880;
        public int PackageSize { get; set; } = 1024 * 1024 * 5;
        /// <summary>
        /// 允许叠包最大次数，如果数据包太大，接收数据的次数超出StackNumberMax值，则会清除叠包缓存器 默认可叠包50次
        /// </summary>
        //public int StackNumberMax { get; set; } = 50;
        /// <summary>
        /// 心跳时间间隔, 默认每2秒检查一次玩家是否离线, 玩家心跳确认为5次, 如果超出5次 则移除玩家客户端. 确认玩家离线总用时10秒, 
        /// 如果设置的值越小, 确认的速度也会越快. 但发送的数据也会增加. [开发调式时尽量把心跳值设置高点]
        /// </summary>
        public virtual int HeartInterval { get; set; } = 2000;
        /// <summary>
        /// <para>心跳检测次数, 默认为5次检测, 如果5次发送心跳给客户端或服务器, 没有收到回应的心跳包, 则进入断开连接处理</para>
        /// <para>当一直有数据往来时是不会发送心跳数据的, 只有当没有数据往来了, 才会进入发送心跳数据</para>
        /// </summary>
        public virtual byte HeartLimit { get; set; } = 5;
        /// <summary>
        /// 由于随机数失灵导致死循环, 所以用计数来标记用户标识 (从10000开始标记)
        /// </summary>
        public int BeginUserID { get; set; } = 10000;
        /// <summary>
        /// 当前玩家唯一标识计数
        /// </summary>
        public int CurrUserID { get; set; }
        /// <summary>
        /// 玩家唯一标识栈
        /// </summary>
        protected ConcurrentStack<int> UserIDStack = new ConcurrentStack<int>();
        /// <summary>
        /// <para>（Maxium Transmission Unit）最大传输单元, 最大传输单元为1500字节</para>
        /// <para>1.链路层：以太网的数据帧的长度为(64+18)~(1500+18)字节，其中18是数据帧的帧头和帧尾，所以数据帧的内容最大为1500字节（不包括帧头和帧尾），即MUT为1500字节</para>
        /// <para>2.网络层：IP包的首部要占用20字节，所以这里的MTU＝1500－20＝1480字节</para>
        /// <para>3.传输层：UDP包的首部要占有8字节，所以这里的MTU＝1480－8＝1472字节</para>
        /// <see langword="注意:服务器和客户端的MTU属性的值必须保持一致性,否则分包的数据将解析错误!"/> <see cref="Client.ClientBase.MTU"/>
        /// </summary>
        public int MTU { get; set; } = 1300;
        /// <summary>
        /// （Retransmission TimeOut）重传超时时间。 默认为1秒重传一次
        /// </summary>
        public int RTO { get; set; } = 1000;
        /// <summary>
        /// (Maximum traffic per second) 每秒允许传输最大流量, 默认最大每秒可以传输1m大小
        /// </summary>
        public int MTPS { get; set; } = 1024 * 1024;
        /// <summary>
        /// 流量控制模式，只有Gcp协议可用
        /// </summary>
        public FlowControlMode FlowControl { get; set; } = FlowControlMode.Normal;
        /// <summary>
        /// 并发线程数量, 发送线程和接收处理线程数量
        /// </summary>
        public int MaxThread { get; set; } = Environment.ProcessorCount;
        /// <summary>
        /// 组包数量，如果是一些小数据包，最多可以组合多少个？ 默认是组合1000个后发送
        /// </summary>
        public int PackageLength { get; set; } = 1000;
        protected bool md5crc;
        /// <summary>
        /// 采用md5 + 随机种子校验
        /// </summary>
        public virtual bool MD5CRC {
            get => md5crc;
            set 
            {
                md5crc = value;
                if (value)
                    frame = 1 + 16;
                else
                    frame = 1;
            }
        }
        /// <summary>
        /// 随机种子密码
        /// </summary>
        public int Password { get; set; } = 123456789;
        /// <summary>
        /// 限制发送队列长度
        /// </summary>
        public int LimitQueueCount { get; set; } = ushort.MaxValue;
        /// <summary>
        /// 程序根路径, 网络数据缓存读写路径
        /// </summary>
        protected string rootPath;
        /// <summary>
        /// 单线程调用队列
        /// </summary>
        protected ConcurrentQueue<Action> SingleCallQueue { get; set; } = new ConcurrentQueue<Action>();
        protected volatile int threadNum;
        protected List<QueueSafe<RevdDataBuffer>> RcvQueues = new List<QueueSafe<RevdDataBuffer>>();
        /// <summary>
        /// 线程组, 优化多线程资源竞争问题
        /// </summary>
        protected List<ThreadGroup> ThreadGroups = new List<ThreadGroup>();
        /// <summary>
        /// 线程组字典, key是线程唯一id
        /// </summary>
        protected MyDictionary<int, ThreadGroup> ThreadGroupDict = new MyDictionary<int, ThreadGroup>();
        /// <summary>
        /// 排队队列
        /// </summary>
        protected ConcurrentQueue<Player> QueueUp = new ConcurrentQueue<Player>();
        /// <summary>
        /// 同步锁对象
        /// </summary>
        protected readonly object SyncRoot = new object();
        protected int[] taskIDs = new int[4];
        #endregion

        #region 服务器事件处理
        /// <summary>
        /// 开始运行服务器事件
        /// </summary>
        public Action OnStartingHandle { get; set; }
        /// <summary>
        /// 服务器启动成功事件
        /// </summary>
        public Action OnStartupCompletedHandle { get; set; }
        /// <summary>
        /// 当前有客户端连接触发事件
        /// </summary>
        public Action<Player> OnHasConnectHandle { get; set; }
        /// <summary>
        /// 当添加客户端到所有在线的玩家集合中触发的事件
        /// </summary>
        public Action<Player> OnAddClientHandle { get; set; }
        /// <summary>
        /// 当接收到自定义的网络指令时处理事件
        /// </summary>
        public virtual RevdBufferHandle<Player> OnRevdBufferHandle { get; set; }
        /// <summary>
        /// 当移除客户端时触发事件
        /// </summary>
        public Action<Player> OnRemoveClientHandle { get; set; }
        /// <summary>
        /// 当统计网络流量时触发
        /// </summary>
        public NetworkDataTraffic OnNetworkDataTraffic { get; set; }
        /// <summary>
        /// 当客户端在时间帧发送的操作数据， 当使用客户端的<see cref="Client.ClientBase.AddOperation(Operation)"/>方法时调用
        /// </summary>
        public Action<Player, OperationList> OnOperationSyncHandle { get; set; }
        /// <summary>
        /// 当客户端发送的大数据时, 可监听此事件显示进度值
        /// </summary>
        public virtual Action<Player, RTProgress> OnRevdRTProgressHandle { get; set; }
        /// <summary>
        /// 当服务器发送可靠数据时, 可监听此事件显示进度值 (NetworkServer,TcpServer类无效)
        /// </summary>
        public virtual Action<Player, RTProgress> OnSendRTProgressHandle { get; set; }
        /// <summary>
        /// 输出日志, 这里是输出全部日志(提示,警告,错误等信息). 如果想只输出指定的日志, 请使用NDebug类进行监听
        /// </summary>
        public Action<string> Log { get; set; }
        /// <summary>
        /// ping服务器回调 参数double为延迟毫秒单位 当<see cref="RTOMode"/>=<see cref="RTOMode.Variable"/>可变重传时, 内核将会每秒自动ping一次
        /// </summary>
        public Action<Player, uint> OnPingCallback;
        /// <summary>
        /// 当socket发送失败调用.参数1:玩家对象, 参数2:发送的字节数组, 参数3:发送标志(可靠和不可靠)  ->可通过<see cref="SendByteData"/>方法重新发送
        /// </summary>
        public Action<Player, byte[], bool> OnSendErrorHandle;
        /// <summary>
        /// 当添加远程过程调用方法时调用， 参数1：要收集rpc特性的对象，参数2:是否异步收集rpc方法和同步字段与属性？ 参数3：如果服务器的rpc中已经有了这个对象，还可以添加进去？
        /// </summary>
        public Action<object, bool, Action<SyncVarInfo>> OnAddRpcHandle { get; set; }
        /// <summary>
        /// 当移除远程过程调用对象， 参数1：移除此对象的所有rpc方法
        /// </summary>
        public Action<object> OnRemoveRpc { get; set; }
        /// <summary>
        /// 当执行调用远程过程方法时触发
        /// </summary>
        public Action<Player, RPCModel> OnRPCExecute { get; set; }
        /// <summary>
        /// 当序列化远程过程调用方法
        /// </summary>
        public Func<RPCModel, byte[]> OnSerializeRPC { get; set; }
        /// <summary>
        /// 当反序列化远程过程调用方法
        /// </summary>
        public Func<byte[], int, int, FuncData> OnDeserializeRPC { get; set; }
        /// <summary>
        /// 当序列化远程过程调用操作
        /// </summary>
        public Func<OperationList, byte[]> OnSerializeOPT { get; set; }
        /// <summary>
        /// 当反序列化远程过程调用操作
        /// </summary>
        public Func<byte[], int, int, OperationList> OnDeserializeOPT { get; set; }
        /// <summary>
        /// 当开始下载文件时调用, 参数1(Player):下载哪个玩家上传的文件 参数2(string):客户端上传的文件名 返回值(string):开发者指定保存的文件路径(全路径名称)
        /// </summary>
        public Func<Player, string, string> OnDownloadFileHandle { get; set; }
        /// <summary>
        /// 当客户端发送的文件完成, 接收到文件后调用, 返回true:框架内部释放文件流和删除临时文件(默认) false:使用者处理
        /// </summary>
        public Func<Player, FileData, bool> OnReceiveFileHandle { get; set; }
        /// <summary>
        /// 当接收到发送的文件进度
        /// </summary>
        public Action<Player, RTProgress> OnRevdFileProgress { get; set; }
        /// <summary>
        /// 当发送的文件进度
        /// </summary>
        public Action<Player, RTProgress> OnSendFileProgress { get; set; }
        /// <summary>
        /// 可靠传输是排队模式? 排队模式下, 可靠包是一个一个处理. 不排队模式: 可靠传输数据组成多列并发 ---> 默认是无排队模式
        /// </summary>
        public bool Seqencing { get; set; }
        /// <summary>
        /// 服务器线程管理
        /// </summary>
        protected internal Dictionary<string, Thread> threads = new Dictionary<string, Thread>();
        private int sendFileTick, recvFileTick;
        #endregion

        /// <summary>
        /// 构造网络服务器函数
        /// </summary>
        public ServerBase()
        {
        }

        #region 索引
        /// <summary>
        /// 玩家索引
        /// </summary>
        /// <param name="remotePoint"></param>
        /// <returns></returns>
        public Player this[EndPoint remotePoint] => AllClients[remotePoint];

        /// <summary>
        /// uid索引
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public Player this[int uid] => UIDClients[uid];

        /// <summary>
        /// 场景索引
        /// </summary>
        /// <param name="sceneID"></param>
        /// <returns></returns>
        public Scene this[string sceneID] => Scenes[sceneID];

        /// <summary>
        /// 获得所有在线的客户端对象
        /// </summary>
        /// <returns></returns>
        public List<Player> GetClients()
        {
            List<Player> players = new List<Player>();
            foreach (Player p in AllClients.Values)
                if (p.Login)
                    players.Add(p);
            return players;
        }

        /// <summary>
        /// 获得所有服务器场景
        /// </summary>
        /// <returns></returns>
        public List<Scene> GetScenes()
        {
            return new List<Scene>(Scenes.Values);
        }
        #endregion

        #region 重写方法
        /// <summary>
        /// 当未知客户端发送数据请求，返回<see langword="false"/>，不允许<see langword="unClient"/>进入服务器!，如果返回的是<see langword="true"/>，则允许<see langword="unClient"/>客户端进入服务器
        /// 同时会将<see langword="unClient"/>添加到<see cref="Players"/>和<see cref="UIDClients"/>在线字典中.
        /// <code>客户端玩家的入口点，在这里可以控制客户端是否可以进入服务器与其他客户端进行网络交互</code>
        /// 在这里可以用来判断客户端登录和注册等等进站许可 (默认是允许进入服务器)
        /// </summary>
        /// <param name="unClient">尚未登录的客户端对象</param>
        /// <param name="model">数据模型</param>
        /// <returns></returns>
        protected virtual bool OnUnClientRequest(Player unClient, RPCModel model)
        {
            return true;
        }

        /// <summary>
        /// 当开始启动服务器
        /// </summary>
        protected virtual void OnStarting() { Debug.Log("服务器开始运行..."); }

        /// <summary>
        /// 当服务器启动完毕
        /// </summary>
        protected virtual void OnStartupCompleted() { Debug.Log("服务器启动成功!"); }

        /// <summary>
        /// 当添加默认网络场景，服务器初始化后会默认创建一个主场景，供所有玩家刚登陆成功分配的临时场景，默认初始化场景人数为1000人
        /// </summary>
        /// <returns>返回值string：网络玩家所在的场景名称 , 返回值NetScene：网络玩家的场景对象</returns>
        protected virtual Scene OnAddDefaultScene()
        {
            return new Scene { Name = MainSceneName, sceneCapacity = 1000 };
        }

        /// <summary>
        /// 当添加玩家到默认场景， 如果不想添加刚登录游戏成功的玩家进入主场景，可重写此方法让其失效
        /// </summary>
        /// <param name="client"></param>
        protected virtual void OnAddPlayerToScene(Player client)
        {
            if (Scenes.TryGetValue(MainSceneName, out Scene scene))
            {
                scene.AddPlayer(client);//将网络玩家添加到主场景集合中
            }
        }

        /// <summary>
        /// 当有客户端连接
        /// </summary>
        /// <param name="client">客户端套接字</param>
        protected virtual void OnHasConnect(Player client)
        {
            if (client.RemotePoint != null)
                Debug.Log("有客户端连接:" + client.RemotePoint.ToString());
            else if (client.Client != null)
                Debug.Log("有客户端连接:" + client.Client.RemoteEndPoint.ToString());
        }

        /// <summary>
        /// 当服务器判定客户端为断线或连接异常时，移除客户端时调用
        /// </summary>
        /// <param name="client">要移除的客户端</param>
        protected virtual void OnRemoveClient(Player client) { Debug.Log($"[{client.Name}]断开{(client.Redundant ? "冗余" : "")}连接!"); }

        /// <summary>
        /// 当开始调用服务器RPC函数 或 开始调用自定义网络命令时 可设置请求客户端的client为全局字段，方便在服务器RPC函数内引用!!!
        /// 在多线程时有1%不安全，当出现client赋值到其他玩家对象时，可在网络方法加<see langword="[Rpc(NetCmd.SafeCall)]"/>特性
        /// </summary>
        /// <param name="client">发送请求数据的客户端</param>
        [Obsolete("请重写OnRpcExecute方法实现!")]
        protected virtual void OnInvokeRpc(Player client) { }

        /// <summary>
        /// 当接收到客户端自定义数据请求,在这里可以使用你自己的网络命令，系列化方式等进行解析网络数据。（你可以在这里使用ProtoBuf或Json来解析网络数据）
        /// </summary>
        /// <param name="client">当前客户端</param>
        /// <param name="model"></param>
        protected virtual void OnReceiveBuffer(Player client, RPCModel model) { }

        /// <summary>
        /// 当接收到客户端发送的文件
        /// </summary>
        /// <param name="client">当前客户端</param>
        /// <param name="fileData"></param>
        protected virtual bool OnReceiveFile(Player client, FileData fileData) { return true; }

        /// <summary>
        /// 当接收到客户端使用<see cref="Client.ClientBase.AddOperation(Operation)"/>方法发送的请求时调用
        /// </summary>
        /// <param name="client">当前客户端</param>
        /// <param name="list">操作列表</param>
        protected virtual void OnOperationSync(Player client, OperationList list)
        {
            if (client.OnOperationSync(list))
                return;
            var scene = client.Scene as Scene;
            if (scene != null)
                scene.OnOperationSync(client, list);
        }

        /// <summary>
        /// 当客户端发送的大数据时, 可监听此事件显示进度值
        /// </summary>
        /// <param name="client"></param>
        /// <param name="progress"></param>
        protected virtual void OnRevdRTProgress(Player client, RTProgress progress) { }

        /// <summary>
        /// 当服务器发送的大数据时, 可监听此事件显示进度值
        /// </summary>
        /// <param name="client"></param>
        /// <param name="progress"></param>
        protected virtual void OnSendRTProgress(Player client, RTProgress progress) { }

        /// <summary>
        /// 当内核序列化远程函数时调用, 如果想改变内核rpc的序列化方式, 可重写定义序列化协议
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        protected virtual byte[] OnSerializeRpc(RPCModel model) { return OnSerializeRPC(model); }

        protected internal byte[] OnSerializeRpcInternal(RPCModel model) { return NetConvert.Serialize(model); }

        /// <summary>
        /// 当内核解析远程过程函数时调用, 如果想改变内核rpc的序列化方式, 可重写定义解析协议
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        protected virtual FuncData OnDeserializeRpc(byte[] buffer, int index, int count) { return OnDeserializeRPC(buffer, index, count); }

        protected internal FuncData OnDeserializeRpcInternal(byte[] buffer, int index, int count) { return NetConvert.Deserialize(buffer, index, count); }
        #endregion

        /// <summary>
        /// 运行服务器
        /// </summary>
        /// <param name="port">服务器端口号</param>
        public void Run(ushort port = 6666) => Start(port);

        /// <summary>
        /// 启动服务器
        /// </summary>
        /// <param name="port">端口</param>
        public virtual void Start(ushort port = 6666)
        {
            if (Server != null)//如果服务器套接字已创建
                throw new Exception("服务器已经运行，不可重新启动，请先关闭后在重启服务器");
            Port = port;
            RegisterEvent();
            Debug.BindLogAll(Log);
            OnStartingHandle();
            if (Instance == null)
                Instance = this;
            AddRpc(this, true, null);
            CreateServerSocket(port);
            IsRunServer = true;
            StartSocketHandler();
            CreateSenderThread();
            CreateSceneTickThread();
            CreateOtherThread();
            AddLoopEvent();
            SetProcessThreads();
            AddDefaultScene();
            OnStartupCompletedHandle();
#if WINDOWS
            Win32KernelAPI.timeBeginPeriod(1);
#endif
            InitUserID();
        }

        protected virtual void AddDefaultScene()
        {
            var scene = OnAddDefaultScene();
            if (scene != null)
            {
                MainSceneName = scene.Name;
                CreateScene(scene);
            }
        }

        protected virtual void AddLoopEvent()
        {
            int id = 0;
            taskIDs[id++] = ThreadManager.Invoke("DataTrafficHandler", 1f, DataTrafficHandler);
            taskIDs[id++] = ThreadManager.Invoke("SingleHandler", SingleHandler);
            taskIDs[id++] = ThreadManager.Invoke("SyncVarHandler", SyncVarHandler);
        }

        protected virtual void CreateSenderThread()
        {
            var thread = new Thread(SendDataHandle) { IsBackground = true, Name = "SendDataHandle" };
            thread.Start();
            threads.Add("SendDataHandle", thread);
        }

        protected virtual void CreateSceneTickThread()
        {
            var thread = new Thread(SceneUpdateHandle) { IsBackground = true, Name = "SceneTickHandle" };
            thread.Start();
            threads.Add("SceneTickHandle", thread);
        }

        protected virtual void CreateOtherThread() { }

        protected virtual void CreateServerSocket(ushort port)
        {
        }

        protected void SetProcessThreads()
        {
            for (int i = 0; i < MaxThread; i++)
            {
                var group = new ThreadGroup() { Id = i + 1 };
                var receive = new Thread(ProcessReceive) { IsBackground = true, Name = "ProcessReceive" + i };
                receive.Start(group);
                group.Thread = receive;
                ThreadGroups.Add(group);
                ThreadGroupDict[receive.ManagedThreadId] = group;
                threads.Add("ProcessReceive" + i, receive);
            }
        }

        protected void RegisterEvent()
        {
            OnStartingHandle += OnStarting;
            OnStartupCompletedHandle += OnStartupCompleted;
            OnHasConnectHandle += OnHasConnect;
            OnRemoveClientHandle += OnRemoveClient;
            OnOperationSyncHandle += OnOperationSync;
            OnRevdBufferHandle += OnReceiveBuffer;
            OnReceiveFileHandle += OnReceiveFile;
            OnRevdRTProgressHandle += OnRevdRTProgress;
            OnSendRTProgressHandle += OnSendRTProgress;
            if (OnAddRpcHandle == null) OnAddRpcHandle = AddRpcInternal;//在start之前就要添加你的委托
            if (OnRemoveRpc == null) OnRemoveRpc = RemoveRpcInternal;
            if (OnRPCExecute == null) OnRPCExecute = OnRpcExecuteInternal;
            if (OnSerializeRPC == null) OnSerializeRPC = OnSerializeRpcInternal;
            if (OnDeserializeRPC == null) OnDeserializeRPC = OnDeserializeRpcInternal;
            if (OnSerializeOPT == null) OnSerializeOPT = OnSerializeOptInternal;
            if (OnDeserializeOPT == null) OnDeserializeOPT = OnDeserializeOptInternal;
        }

        /// <summary>
        /// 初始化玩家唯一标识栈
        /// </summary>
        protected void InitUserID()
        {
            UserIDStack.Clear();
            CurrUserID = BeginUserID;
        }

        /// <summary>
        /// 网络场景推动玩家同步更新处理线程, 如果想自己处理场景同步, 可重写此方法让同步失效
        /// </summary>
        protected virtual void SceneUpdateHandle()
        {
            var timer = new TimerTick();
            uint tick = (uint)Environment.TickCount;
            while (IsRunServer)
            {
                try
                {
                    tick = (uint)Environment.TickCount;
                    if (timer.CheckTimeout(tick, (uint)SyncSceneTime, true))
                    {
                        var result = Parallel.ForEach(Scenes.Values, scene =>
                        {
                            scene.Update(this, NetCmd.OperationSync);
                        });
                        while (!result.IsCompleted)
                        {
                            Thread.Sleep(1);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError("场景更新异常:" + ex);
                }
            }
        }

        /// <summary>
        /// 调用服务器单线程, 每帧调用
        /// </summary>
        /// <param name="ptr"></param>
        /// <returns>可用于结束事件的id</returns>
        public int Invoke(Func<bool> ptr)
        {
            return ThreadManager.Invoke(0, ptr);
        }

        /// <summary>
        /// 调用服务器单线程
        /// </summary>
        /// <param name="time"></param>
        /// <param name="ptr"></param>
        /// <returns>可用于结束事件的id</returns>
        public int Invoke(float time, Action ptr)
        {
            return ThreadManager.Event.AddEvent(time, ptr);
        }

        /// <summary>
        /// 调用服务器单线程计算器, 如果不返回false, 就会每time秒调用
        /// </summary>
        /// <param name="time"></param>
        /// <param name="ptr"></param>
        /// <returns>可用于结束事件的id</returns>
        public int Invoke(float time, Func<bool> ptr)
        {
            return ThreadManager.Invoke(time, ptr);
        }

        /// <summary>
        /// 流量统计线程
        /// </summary>
        protected virtual bool DataTrafficHandler()
        {
            try
            {
                outflowTotal += (long)sendCount;
                inflowTotal += (long)receiveCount;
                OnNetworkDataTraffic?.Invoke(new Dataflow()
                {
                    sendCount = sendCount,
                    sendNumber = sendAmount,
                    receiveNumber = receiveAmount,
                    receiveCount = receiveCount,
                    resolveNumber = resolveAmount,
                    sendLoopNum = sendLoopNum,
                    revdLoopNum = revdLoopNum,
                    outflowTotal = outflowTotal,
                    inflowTotal = inflowTotal,
                });
            }
            catch (Exception ex)
            {
                Debug.LogError("流量统计异常:" + ex);
            }
            finally
            {
                sendCount = 0;
                sendAmount = 0;
                resolveAmount = 0;
                receiveAmount = 0;
                receiveCount = 0;
                sendLoopNum = 0;
                revdLoopNum = 0;
            }
            return IsRunServer;
        }

        /// <summary>
        /// 单线程处理
        /// </summary>
        /// <returns></returns>
        protected virtual bool SingleHandler() 
        {
            try
            {
                while (SingleCallQueue.TryDequeue(out Action action))
                {
                    action();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("单线程异常:" + ex);
            }
            return IsRunServer;
        }

        /// <summary>
        /// 开始接收数据
        /// </summary>
        protected virtual void StartSocketHandler()
        {
        }

        protected virtual void OnIOCompleted(object sender, SocketAsyncEventArgs args)
        {
            switch (args.LastOperation)
            {
                case SocketAsyncOperation.ReceiveFrom:
                    try
                    {
                        int count = args.BytesTransferred;
                        if (count > 0)
                        {
                            var buffer = BufferPool.Take();
                            Buffer.BlockCopy(args.Buffer, 0, buffer, 0, count);
                            buffer.Count = count;
                            receiveCount += count;
                            receiveAmount++;
                            var remotePoint = args.RemoteEndPoint;
                            ReceiveProcessed(remotePoint, buffer, false);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex.ToString());
                    }
                    finally
                    {
                        if (Server != null & IsRunServer)
                            if (!Server.ReceiveFromAsync(args))
                                OnIOCompleted(null, args);
                    }
                    break;
                case SocketAsyncOperation.SendTo:
                    ObjectPool<SocketAsyncEventArgs>.Push(args);
                    break;
            }
        }

        protected int GetCurrUserID()
        {
            lock (SyncRoot) 
            {
                return CurrUserID++;
            }
        }

        /// <summary>
        /// 业务处理线程组
        /// </summary>
        /// <param name="obj"></param>
        protected virtual void ProcessReceive(object obj)
        {
            var allClients = new Player[0];
            uint tick = (uint)Environment.TickCount;
            uint heartTick = tick + (uint)HeartInterval;
            var group = obj as ThreadGroup;
            EndPoint remotePoint = null;
            if (Server != null)
                remotePoint = Server.LocalEndPoint;
            while (IsRunServer)
            {
                try
                {
                    bool isSleep = true;
                    ReceiveProcessed(remotePoint, ref isSleep);
                    if (allClients.Length != AllClients.Count)
                        allClients = AllClients.Values.ToArray();
                    tick = (uint)Environment.TickCount;
                    bool isCheckHeart = false;
                    if (tick >= heartTick)
                    {
                        heartTick = tick + (uint)HeartInterval;
                        isCheckHeart = true;
                    }
                    for (int i = 0; i < allClients.Length; i++)
                    {
                        var client = allClients[i];
                        if (client.Group != group)
                            continue;
                        if (client.isDispose)
                            continue;
                        if (isCheckHeart)
                            CheckHeart(client);
                        if (client.CloseReceive)
                            goto J;
                        ResolveDataQueue(client, ref isSleep);
                        J: OnClientTick(client);
                    }
                    if(isSleep)
                        Thread.Sleep(1);
                    revdLoopNum++;
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex.ToString());
                }
            }
        }

        protected virtual void OnClientTick(Player client)
        {
        }

        protected virtual void ResolveDataQueue(Player client, ref bool isSleep)
        {
            while (client.RevdQueue.TryDequeue(out var segment))
            {
                DataCRCHandle(client, segment, false);
                BufferPool.Push(segment);
            }
        }

        protected virtual void ReceiveProcessed(EndPoint remotePoint, ref bool isSleep)
        {
            if (Server.Poll(0, SelectMode.SelectRead))
            {
                var buffer = BufferPool.Take();
                buffer.Count = Server.ReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref remotePoint);
                receiveCount += buffer.Count;
                receiveAmount++;
                ReceiveProcessed(remotePoint, buffer, false);
                isSleep = false;
            }
        }

        protected virtual void ReceiveProcessed(EndPoint remotePoint, Segment buffer, bool tcp_udp)
        {
            if (!AllClients.TryGetValue(remotePoint, out Player client))//在线客户端  得到client对象
                client = AcceptHander(null, remotePoint);
            client.heart = 0;//udp在关闭发送和接收后，客户端还是能给服务器发信息，导致服务器一直提示有客户端连接和断开连接，所以这里给他在服务器逗留，但不处理客户端任何数据，直到客户端自己不发送信息为止
            if (client.CloseReceive)
            {
                BufferPool.Push(buffer);
                return;
            }
            client.RevdQueue.Enqueue(buffer);
        }

        protected virtual Player AcceptHander(Socket clientSocket, EndPoint remotePoint)
        {
            var client = new Player();
            client.Client = clientSocket;
            client.RemotePoint = remotePoint;
            if (!UserIDStack.TryPop(out int uid))
                uid = GetCurrUserID();
            client.UserID = uid;
            client.PlayerID = uid.ToString();
            client.Name = uid.ToString();
            client.stackStream = new MemoryStream(Net.Config.Config.BaseCapacity);//BufferStreamShare.Take();
            OnThreadQueueSet(client);
            AcceptHander(client);
            SetClientIdentity(client);
            AllClients.TryAdd(remotePoint, client);//之前放在上面, 由于接收线程并行, 还没赋值revdQueue就已经接收到数据, 导致提示内存池泄露
#if TEST1
            UIDClients.TryAdd(uid, client);//测试
#endif
            OnHasConnectHandle(client);
            if (AllClients.Count >= OnlineLimit + LineUp)
            {
                SendRT(client, NetCmd.ServerFull, new byte[0]);
                SendDirect(client);
                client.CloseSend = true;
                client.CloseReceive = true;
                client.QueueUpNo = int.MaxValue;
            }
            else if (AllClients.Count > OnlineLimit)
            {
                QueueUp.Enqueue(client);
                client.QueueUpNo = QueueUp.Count;
                var segment = BufferPool.Take(50);
                segment.Write(QueueUp.Count);
                segment.Write(client.QueueUpNo);
                SendRT(client, NetCmd.QueueUp, segment.ToArray(true));
            }
            return client;
        }

        protected virtual void OnThreadQueueSet(Player client) 
        {
            var value = threadNum++;
            client.Group = ThreadGroups[value % ThreadGroups.Count];
        }

        protected virtual void AcceptHander(Player client) 
        {
        }

        protected void SetClientIdentity(Player client)
        {
            var segment = BufferPool.Take(50);
            segment.Write(client.UserID);
            segment.Write(client.PlayerID);
            SendRT(client, NetCmd.Identify, segment.ToArray(true));
        }

#if TEST1
        internal void ReceiveTest(byte[] buffer)//本机测试
        {
            var client = UIDClients[10000];
            var segment = new Segment(buffer, false);
            receiveCount += segment.Count;
            receiveAmount++;
            client.revdQueue.Enqueue(new RevdDataBuffer() { client = client, buffer = segment, tcp_udp = true });
        }
#endif

        protected virtual void DataCRCHandle(Player client, Segment buffer, bool isTcp)
        {
            if (MD5CRC)
            {
                var md5Hash = buffer.Read(16);
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(buffer, buffer.Position, buffer.Count - buffer.Position);
                EncryptHelper.ToDecrypt(Password, md5Hash, 0, 16);
                for (int i = 0; i < md5Hash.Length; i++)
                {
                    if (retVal[i] != md5Hash[i])
                    {
                        Debug.LogError($"[{client}]MD5CRC校验失败!");
                        return;
                    }
                }
            }
            else if(!isTcp)
            {
                byte crcCode = buffer.ReadByte();//CRC检验索引
                byte retVal = CRCHelper.CRC8(buffer, buffer.Position, buffer.Count);
                if (crcCode != retVal) 
                {
                    Debug.LogError($"[{client}]CRC校验失败!");
                    return;
                }
            }
            DataHandle(client, buffer);
        }

        protected virtual void DataHandle(Player client, Segment buffer)
        {
            while (buffer.Position < buffer.Count)
            {
                int kernelV = buffer.ReadByte();
                bool kernel = kernelV == 68;
                if (!kernel & kernelV != 74)
                {
                    Debug.LogError($"[{client}][可忽略]协议出错!");
                    break;
                }
                byte cmd1 = buffer.ReadByte();
                int dataCount = buffer.ReadInt32();
                if (buffer.Position + dataCount > buffer.Count)
                    break;
                var position = buffer.Position + dataCount;
                var model = new RPCModel(cmd1, kernel, buffer, buffer.Position, dataCount);
                if (kernel & cmd1 != NetCmd.Scene & cmd1 != NetCmd.SceneRT & cmd1 != NetCmd.Notice & cmd1 != NetCmd.NoticeRT & cmd1 != NetCmd.Local & cmd1 != NetCmd.LocalRT)
                {
                    var func = OnDeserializeRpc(buffer, buffer.Position, dataCount);
                    if (func.error)
                        goto J;
                    model.func = func.name;
                    model.pars = func.pars;
                    model.methodHash = func.hash;
                }
                DataHandle(client, model, buffer);//解析协议完成
                J: buffer.Position = position;
            }
        }

        protected virtual void ResolveBuffer(Player client, ref Segment buffer)
        {
            client.heart = 0;
            //if (client.stack > StackNumberMax)//不能一直叠包
            //{
            //    client.stack = 0;
            //    Debug.LogError($"[{client}]请设置StackNumberMax属性, 叠包次数过高, 叠包数量达到{StackNumberMax}次以上...");
            //    return;
            //}
            if (client.stack > 0)
            {
                client.stack++;
                client.stackStream.Seek(client.stackIndex, SeekOrigin.Begin);
                int size = buffer.Count - buffer.Position;
                client.stackIndex += size;
                client.stackStream.Write(buffer, buffer.Position, size);
                if (client.stackIndex < client.stackCount)
                {
                    InvokeRevdRTProgress(client, client.stackIndex, client.stackCount);
                    return;
                }
                var count = (int)client.stackStream.Position;//.Length; //错误问题,不能用length, 这是文件总长度, 之前可能已经有很大一波数据
                BufferPool.Push(buffer);//要回收掉, 否则会提示内存泄露
                buffer = BufferPool.Take(count);//ref 才不会导致提示内存泄露
                client.stackStream.Seek(0, SeekOrigin.Begin);
                client.stackStream.Read(buffer, 0, count);
                buffer.Count = count;
            }
            while (buffer.Position < buffer.Count)
            {
                if (buffer.Position + 5 > buffer.Count)//流数据偶尔小于frame头部字节
                {
                    var position = buffer.Position;
                    var count = buffer.Count - position;
                    client.stackIndex = count;
                    client.stackCount = 0;
                    client.stackStream.Seek(0, SeekOrigin.Begin);
                    client.stackStream.Write(buffer, position, count);
                    client.stack++;
                    break;
                }
                var lenBytes = buffer.Read(4);
                byte crcCode = buffer.ReadByte();//CRC检验索引
                byte retVal = CRCHelper.CRC8(lenBytes, 0, 4);
                if (crcCode != retVal)
                {
                    client.stack = 0;
                    Debug.LogError($"[{client}]CRC校验失败!");
                    return;
                }
                int size = BitConverter.ToInt32(lenBytes, 0);
                if (size < 0 | size > PackageSize)//如果出现解析的数据包大小有问题，则不处理
                {
                    client.stack = 0;
                    NDebug.LogError($"[{client}]数据被拦截修改或数据量太大: size:{size}，如果想传输大数据，请设置PackageSize属性");
                    return;
                }
                int value = MD5CRC ? 16 : 0;
                if (buffer.Position + size + value <= buffer.Count)
                {
                    client.stack = 0;
                    var count = buffer.Count;//此长度可能会有连续的数据(粘包)
                    buffer.Count = buffer.Position + value + size;//需要指定一个完整的数据长度给内部解析
                    DataCRCHandle(client, buffer, true);
                    buffer.Count = count;//解析完成后再赋值原来的总长
                }
                else
                {
                    var position = buffer.Position - 5;
                    var count = buffer.Count - position;
                    client.stackIndex = count;
                    client.stackCount = size;
                    client.stackStream.Seek(0, SeekOrigin.Begin);
                    client.stackStream.Write(buffer, position, count);
                    client.stack++;
                    break;
                }
            }
        }

        protected virtual bool IsInternalCommand(Player client, RPCModel model)
        {
            if (model.cmd == NetCmd.Connect)
            {
                Send(client, NetCmd.Connect, new byte[0]);
                return true;
            }
            if (model.cmd == NetCmd.Broadcast)
            {
                string hostName = Dns.GetHostName();
                IPHostEntry iPHostEntry = Dns.GetHostEntry(hostName);
                IPAddress ipAddress = IPAddress.Any;
                foreach (IPAddress ipAdd in iPHostEntry.AddressList)
                    if (ipAdd.AddressFamily == AddressFamily.InterNetwork)
                        ipAddress = ipAdd;
                var buffer = Encoding.Unicode.GetBytes(ipAddress.ToString());
                Server.SendTo(buffer, client.RemotePoint);
                return true;
            }
            return false;
        }

        protected virtual void DataHandle(Player client, RPCModel model, Segment segment)
        {
            client.heart = 0;
            if (IsInternalCommand(client, model))
                return;
            if (client.Login)
            {
                CommandHandle(client, model, segment);
                return;
            }
            switch (model.cmd)
            {
                case NetCmd.ReliableTransport:
                    CommandHandle(client, model, segment);
                    return;
                case NetCmd.SendHeartbeat:
                    Send(client, NetCmd.RevdHeartbeat, new byte[0]);
                    return;
                case NetCmd.RevdHeartbeat:
                    return;
                case NetCmd.Disconnect:
                    RemoveClient(client);
                    return;
                case NetCmd.Ping:
                    return;
                case NetCmd.PingCallback:
                    return;
                case NetCmd.Identify:
                    SetClientIdentity(client);
                    break;
                case NetCmd.EntityRpc:
                    if (CheckIsQueueUp(client))
                        return;
                    client.Login = client.OnUnClientRequest(model);
                    break;
                default:
                    if (CheckIsQueueUp(client))
                        return;
                    client.Login = OnUnClientRequest(client, model);
                    break;
            }
            if (client.Login)//当有客户端连接时,如果允许用户添加此客户端
            {
                LoginInternal(client);
            }
        }

        private bool CheckIsQueueUp(Player client) 
        {
            var isQueueUp = client.QueueUpNo > 0;
            if (isQueueUp)
            {
                var segment1 = BufferPool.Take(8);
                segment1.Write(QueueUp.Count);
                segment1.Write(client.QueueUpNo);
                SendRT(client, NetCmd.QueueUp, segment1.ToArray(true, true));
            }
            return isQueueUp;
        }

        /// <summary>
        /// 主动登录服务器, 类似OnUnClientRequest重写方法的返回值为true
        /// </summary>
        /// <param name="client"></param>
        protected void LoginHandle(Player client)
        {
            if (!client.Login)
            {
                client.Login = true;
                LoginInternal(client);
            }
        }

        private void LoginInternal(Player client)
        {
            Players.TryAdd(client.PlayerID, client);
            UIDClients.TryAdd(client.UserID, client);
            client.OnStart();
            OnAddPlayerToScene(client);
            client.AddRpc(client);
            OnAddClientHandle?.Invoke(client);
        }

        protected virtual void CommandHandle(Player client, RPCModel model, Segment segment)
        {
            resolveAmount++;
            switch (model.cmd)
            {
                case NetCmd.EntityRpc:
                    client.OnRpcExecute(model);
                    break;
                case NetCmd.CallRpc:
                    OnRpcExecute(client, model);
                    break;
                case NetCmd.SafeCall:
                    OnRpcExecute(client, model);
                    break;
                case NetCmd.Local:
                    client.udpRPCModels.Enqueue(new RPCModel(model.cmd, model.Buffer, model.kernel, false, model.methodHash));
                    break;
                case NetCmd.LocalRT:
                    client.tcpRPCModels.Enqueue(new RPCModel(model.cmd, model.Buffer, model.kernel, false, model.methodHash));
                    break;
                case NetCmd.Scene:
                    if (!(client.Scene is Scene scene))
                    {
                        client.udpRPCModels.Enqueue(new RPCModel(model.cmd, model.Buffer, model.kernel, false, model.methodHash));
                        return;
                    }
                    Multicast(scene.Clients, false, new RPCModel(model.cmd, model.Buffer, model.kernel, false, model.methodHash));
                    break;
                case NetCmd.SceneRT:
                    if (!(client.Scene is Scene scene1))
                    {
                        client.tcpRPCModels.Enqueue(new RPCModel(model.cmd, model.Buffer, model.kernel, false, model.methodHash));
                        return;
                    }
                    Multicast(scene1.Clients, true, new RPCModel(model.cmd, model.Buffer, model.kernel, false, model.methodHash));
                    break;
                case NetCmd.Notice:
                    Multicast(Players.Values.ToList(), false, new RPCModel(model.cmd, model.Buffer, model.kernel, false, model.methodHash));
                    break;
                case NetCmd.NoticeRT:
                    Multicast(Players.Values.ToList(), true, new RPCModel(model.cmd, model.Buffer, model.kernel, false, model.methodHash));
                    break;
                case NetCmd.SendHeartbeat:
                    Send(client, NetCmd.RevdHeartbeat, new byte[0]);
                    break;
                case NetCmd.RevdHeartbeat:
                    client.heart = 0;
                    break;
                case NetCmd.Disconnect:
                    RemoveClient(client);
                    break;
                case NetCmd.ReliableTransport:
                    client.Gcp.Input(model.Buffer);
                    int count1;
                    Segment buffer1;
                    while ((count1 = client.Gcp.Receive(out buffer1)) > 0)
                    {
                        ReliableTransportComplete(client, buffer1);
                        BufferPool.Push(buffer1);
                    }
                    break;
                case NetCmd.OperationSync:
                    var list = OnDeserializeOpt(model.buffer, model.index, model.count);
                    OnOperationSyncHandle(client, list);
                    break;
                case NetCmd.Ping:
                    client.udpRPCModels.Enqueue(new RPCModel(NetCmd.PingCallback, model.Buffer, model.kernel, false, model.methodHash));
                    break;
                case NetCmd.PingCallback:
                    uint ticks = BitConverter.ToUInt32(model.buffer, model.index);
                    var delayTime = (uint)Environment.TickCount - ticks;
                    OnPingCallback?.Invoke(client, delayTime);
                    break;
                case NetCmd.P2P:
                    int uid = BitConverter.ToInt32(model.buffer, model.index);
                    if (UIDClients.TryGetValue(uid, out Player player))
                    {
                        var endPoint = player.RemotePoint as IPEndPoint;
                        var len = segment.Count;
                        segment.SetPositionLength(0);
                        segment.Write(endPoint.Address.Address);
                        segment.Write(endPoint.Port);
                        SendRT(client, NetCmd.P2P, segment.ToArray(false));
                        segment.Count = len;
                    }
                    break;
                case NetCmd.SyncVarP2P:
                    SyncVarHelper.SyncVarHandler(client.SyncVarDic, model.Buffer);
                    break;
                case NetCmd.SendFile:
                    {
                        var key = segment.ReadInt32();
                        var length = segment.ReadInt64();
                        var fileName = segment.ReadString();
                        var buffer = segment.ReadByteArray();
                        if (!client.ftpDic.TryGetValue(key, out FileData fileData))
                        {
                            fileData = new FileData();
                            string path;
                            if (OnDownloadFileHandle != null)
                            {
                                path = OnDownloadFileHandle(client, fileName);
                                var path1 = Path.GetDirectoryName(path);
                                if (!Directory.Exists(path1))
                                {
                                    Debug.LogError($"[{client}]文件不存在! 或者文件路径字符串编码错误! 提示:可以使用Notepad++查看, 编码是ANSI,不是UTF8");
                                    return;
                                }
                            }
                            else
                            {
                                path = Path.GetTempFileName();
                            }
                            fileData.fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                            fileData.fileName = fileName;
                            client.ftpDic.Add(key, fileData);
                        }
                        fileData.fileStream.Write(buffer, 0, buffer.Length);
                        fileData.Length += buffer.Length;
                        if (fileData.Length >= length)
                        {
                            client.ftpDic.Remove(key);
                            OnRevdFileProgress?.Invoke(client, new RTProgress(fileName, fileData.Length / (float)length * 100f, RTState.Complete));
                            fileData.fileStream.Position = 0;
                            if (OnReceiveFileHandle(client, fileData))
                            {
                                fileData.fileStream.Close();
                                File.Delete(fileData.fileStream.Name);
                            }
                        }
                        else
                        {
                            var len = segment.Count;
                            segment.SetPositionLength(0);
                            segment.Write(key);
                            SendRT(client, NetCmd.Download, segment.ToArray(false));
                            segment.Count = len;
                            if (Environment.TickCount >= recvFileTick)
                            {
                                recvFileTick = Environment.TickCount + 1000;
                                OnRevdFileProgress?.Invoke(client, new RTProgress(fileName, fileData.Length / (float)length * 100f, RTState.Download));
                            }
                        }
                    }
                    break;
                case NetCmd.Download:
                    {
                        var key = segment.ReadInt32();
                        if (client.ftpDic.TryGetValue(key, out FileData fileData))
                            SendFile(client, key, fileData);
                    }
                    break;
                case NetCmd.Identify:
                    SetClientIdentity(client);
                    break;
                default:
                    client.OnRevdBufferHandle(model);
                    OnRevdBufferHandle(client, model);
                    break;
            }
        }

        protected virtual byte[] OnSerializeOpt(OperationList list)
        {
            return OnSerializeOPT(list);
        }

        protected internal byte[] OnSerializeOptInternal(OperationList list)
        {
            return NetConvertFast2.SerializeObject(list).ToArray(true);
        }

        protected virtual OperationList OnDeserializeOpt(byte[] buffer, int index, int count)
        {
            return OnDeserializeOPT(buffer, index, count);
        }

        protected internal OperationList OnDeserializeOptInternal(byte[] buffer, int index, int count)
        {
            var segment = new Segment(buffer, index, count, false);
            return NetConvertFast2.DeserializeObject<OperationList>(segment, false);
        }

        protected virtual void ReliableTransportComplete(Player client, Segment buffer)//为了与NetworkServer协议接轨增加的方法
        {
            DataHandle(client, buffer);
        }

        protected void InvokeRevdRTProgress(Player client, int currValue, int dataCount)
        {
            float bfb = currValue / (float)dataCount * 100f;
            var progress = new RTProgress(bfb, RTState.Sending);
            OnRevdRTProgressHandle(client, progress);
        }

        protected void InvokeSendRTProgress(Player client, int currValue, int dataCount)
        {
            float bfb = currValue / (float)dataCount * 100f;
            var progress = new RTProgress(bfb, RTState.Sending);
            OnSendRTProgressHandle(client, progress);
        }

        protected virtual void SendDataHandle()//发送线程
        {
            var allClients = new Player[0];
            var timer = new TimerTick();
            var tick = (uint)Environment.TickCount;
            while (IsRunServer)
            {
                try
                {
                    if (allClients.Length != AllClients.Count)
                        allClients = AllClients.Values.ToArray();
                    tick = (uint)Environment.TickCount;
                    if (timer.CheckTimeout(tick, (uint)16, true)) 
                    {
                        var result = Parallel.ForEach(allClients, client => SendDirect(client));
                        while (!result.IsCompleted)
                            Thread.Sleep(1);
                        sendLoopNum++;
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning("发送异常:" + ex);
                }
            }
        }

        /// <summary>
        /// 立刻发送, 不需要等待内核时间 (当你要强制把客户端下线时,你还希望客户端先发送完数据后,再强制客户端退出游戏用到)
        /// </summary>
        /// <param name="client"></param>
        public virtual void SendDirect(Player client)
        {
            SendDataHandle(client, client.udpRPCModels, false);//不可靠发送
            SendRTDataHandle(client, client.tcpRPCModels);//可靠发送
        }

        protected virtual void SendRTDataHandle(Player client, QueueSafe<RPCModel> rtRPCModels)
        {
            int count = rtRPCModels.Count;
            if (count <= 0)
                goto J;
            if (client.Gcp.HasSend())
                goto J;
            if (count >= PackageLength)
                count = PackageLength;
            var stream = BufferPool.Take();
            WriteDataBody(client, ref stream, rtRPCModels, count, true);
            client.Gcp.Send(stream.ToArray(true));
        J: client.Gcp.Update();
        }

        protected virtual void WriteDataHead(Segment stream)
        {
            stream.Position = frame;
        }

        protected virtual void WriteDataBody(Player client, ref Segment stream, QueueSafe<RPCModel> rPCModels, int count, bool reliable)
        {
            int index = 0;
            for (int i = 0; i < count; i++)
            {
                if (!rPCModels.TryDequeue(out RPCModel rPCModel))
                    continue;
                if (rPCModel.kernel & rPCModel.serialize)
                {
                    rPCModel.buffer = OnSerializeRpc(rPCModel);
                    if (rPCModel.buffer.Length == 0)
                        continue;
                }
                int len = stream.Position + rPCModel.buffer.Length + frame + 15;
                if (len >= stream.Length)
                {
                    stream.Flush();
                    var stream2 = BufferPool.Take(len);
                    stream2.Write(stream, 0, stream.Count);
                    BufferPool.Push(stream);
                    stream = stream2;
                }
                if (len >= MTU & !reliable)//udp不可靠判断
                {
                    byte[] buffer = PackData(stream);
                    SendByteData(client, buffer, reliable);
                    index = 0;
                    ResetDataHead(stream);
                }
                stream.WriteByte((byte)(rPCModel.kernel ? 68 : 74));
                stream.WriteByte(rPCModel.cmd);
                stream.Write(rPCModel.buffer.Length);
                stream.Write(rPCModel.buffer, 0, rPCModel.buffer.Length);
                if (rPCModel.bigData | ++index >= PackageLength)
                    break;
            }
        }

        /// <summary>
        /// 重置头部数据大小, 在小数据达到<see cref="PackageLength"/>以上时会将这部分的数据先发送, 发送后还有连带的数据, 需要重置头部数据,装入大货车
        /// </summary>
        /// <param name="stream"></param>
        protected virtual void ResetDataHead(Segment stream)
        {
            stream.SetPositionLength(frame);
        }

        protected virtual void SendDataHandle(Player client, QueueSafe<RPCModel> rPCModels, bool reliable)
        {
            int count = rPCModels.Count;//源码中Count执行也不少, 所以优化一下   这里已经取出要处理的长度
            if (count <= 0)
                return;
            var stream = BufferPool.Take();
            WriteDataHead(stream);
            WriteDataBody(client, ref stream, rPCModels, count, reliable);
            byte[] buffer = PackData(stream);
            SendByteData(client, buffer, reliable);
            BufferPool.Push(stream);
        }

        protected virtual byte[] PackData(Segment stream)
        {
            stream.Flush();
            if (MD5CRC)
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(stream, frame, stream.Count - frame);
                EncryptHelper.ToEncrypt(Password, retVal);
                int len = stream.Count;
                stream.Position = 0;
                stream.Write(retVal, 0, retVal.Length);
                stream.Position = len;
            }
            else 
            {
                byte retVal = CRCHelper.CRC8(stream, 1, stream.Count);
                int len = stream.Count;
                stream.Position = 0;
                stream.WriteByte(retVal);
                stream.Position = len;
            }
            return stream.ToArray();
        }

        protected virtual void SendByteData(Player client, byte[] buffer, bool reliable)
        {
            if (buffer.Length == frame)//解决长度==6的问题(没有数据)
                return;
            if (buffer.Length >= 65507)
            {
                Debug.LogError($"[{client}] 数据太大! 请使用SendRT");
                return;
            }
            Server.SendTo(buffer, 0, buffer.Length, SocketFlags.None, client.RemotePoint);
            sendAmount++;
            sendCount += buffer.Length;
        }

        /// <summary>
        /// 当执行Rpc(远程过程调用函数)时调用, 如果想提升服务器Rpc调用性能(默认反射调用), 可以重写此方法, 指定要调用的方法
        /// </summary>
        /// <param name="client">客户端</param>
        /// <param name="model">数据模型</param>
        protected virtual void OnRpcExecute(Player client, RPCModel model)
        {
            OnRPCExecute(client, model);
        }

        protected internal void OnRpcExecuteInternal(Player client, RPCModel model)
        {
            RpcHelper.Invoke(this, model, methods=> 
            {
                foreach (RPCMethod rpc in methods.Values)
                {
                    try
                    {
                        if (rpc.cmd == NetCmd.SafeCall)
                        {
                            object[] pars = new object[model.pars.Length + 1];
                            pars[0] = client;
                            Array.Copy(model.pars, 0, pars, 1, model.pars.Length);
                            rpc.Invoke(pars);
                        }
                        else if (rpc.cmd == NetCmd.SingleCall)
                        {
                            SingleCallQueue.Enqueue(() =>
                            {
                                object[] pars = new object[model.pars.Length + 1];
                                pars[0] = client;
                                Array.Copy(model.pars, 0, pars, 1, model.pars.Length);
                                rpc.Invoke(pars);
                            });
                        }
                        else
                        {
                            rpc.Invoke(model.pars);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"方法:{rpc.method} {model} 详细信息:{ex}");
                    }
                }
            }, log=> {
                switch (log)
                {
                    case 0:
                        Debug.LogWarning($"{client} [mask:{model.methodHash}]的远程方法未被收集!请定义[Rpc(hash = {model.methodHash})] void xx方法和参数, 并使用server.AddRpc方法收集rpc方法!");
                        break;
                    case 1:
                        Debug.LogWarning($"{client} {model.func}的远程方法未被收集!请定义[Rpc]void {model.func}方法和参数, 并使用server.AddRpc方法收集rpc方法!");
                        break;
                    case 2:
                        Debug.LogWarning($"{client} {model}的远程方法未被收集!请定义[Rpc]void xx方法和参数, 并使用server.AddRpc方法收集rpc方法!");
                        break;
                }
            });
        }

        /// <summary>
        /// 检查心跳
        /// </summary>
        /// <param name="client"></param>
        protected virtual void CheckHeart(Player client) 
        {
            client.heart++;
            if (client.heart <= HeartLimit)//有5次确认心跳包
                return;
            if (client.heart < HeartLimit * 2)
            {
                Send(client, NetCmd.SendHeartbeat, new byte[0]);
                return;
            }
            RemoveClient(client);
        }

        /// <summary>
        /// 创建网络场景, 退出当前场景,进入所创建的场景 - 创建场景成功返回场景对象， 创建失败返回null
        /// </summary>
        /// <param name="player">创建网络场景的玩家实体</param>
        /// <param name="name">要创建的场景号或场景名称</param>
        /// <returns></returns>
        public Scene CreateScene(Player player, string name)
        {
            return CreateScene(player, name, new Scene() { Name = name });
        }

        /// <summary>
        /// 创建网络场景, 退出当前场景并加入所创建的场景 - 创建场景成功返回场景对象， 创建失败返回null
        /// </summary>
        /// <param name="player">创建网络场景的玩家实体</param>
        /// <param name="scene">创建场景的实体</param>
        /// <returns></returns>
        public Scene CreateScene(Player player, Scene scene)
        {
            return CreateScene(player, scene.Name, scene);
        }

        /// <summary>
        /// 创建网络场景, 退出当前场景并加入所创建的场景 - 创建场景成功返回场景对象， 创建失败返回null
        /// </summary>
        /// <param name="player">创建网络场景的玩家实体</param>
        /// <param name="name">要创建的场景号或场景名称</param>
        /// <param name="scene">创建场景的实体</param>
        /// <returns></returns>
        public Scene CreateScene(Player player, string name, Scene scene)
        {
            scene.Name = name;
            return CreateScene(player, scene, out _);
        }

        public Scene CreateScene(Player player, Scene scene, out Scene oldScene)
        {
            oldScene = player.Scene as Scene;
            if (string.IsNullOrEmpty(scene.Name))
                return null;
            if (Scenes.TryAdd(scene.Name, scene))
            {
                if (oldScene != null)
                    oldScene.Remove(player);
                OnSceneGroupSet(scene);
                scene.AddPlayer(player);
                scene.onSerializeOptHandle = OnSerializeOpt;
                return scene;
            }
            return null;
        }

        /// <summary>
        /// 创建一个场景, 成功则返回场景对象, 创建失败则返回null
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Scene CreateScene(string name)
        {
            return CreateScene(name, new Scene());
        }

        /// <summary>
        /// 创建一个场景, 成功则返回场景对象, 创建失败则返回null
        /// </summary>
        /// <param name="name"></param>
        /// <param name="scene"></param>
        /// <returns></returns>
        public Scene CreateScene(string name, Scene scene)
        {
            scene.Name = name;
            return CreateScene(scene);
        }

        /// <summary>
        /// 创建一个场景, 成功则返回场景对象, 创建失败则返回null
        /// </summary>
        /// <param name="name"></param>
        /// <param name="scene"></param>
        /// <returns></returns>
        public Scene CreateScene(Scene scene)
        {
            if (string.IsNullOrEmpty(scene.Name))
            {
                Debug.LogError("创建的场景必须给名称,场景名称必须是唯一的!");
                return null;
            }
            if (Scenes.TryAdd(scene.Name, scene))
            {
                OnSceneGroupSet(scene);
                scene.onSerializeOptHandle = OnSerializeOpt;
                return scene;
            }
            return null;
        }

        protected virtual void OnSceneGroupSet(Scene scene) 
        {
        }

        /// <summary>
        /// 退出当前场景,加入指定的场景 - 成功进入返回场景对象，进入失败返回null
        /// </summary>
        /// <param name="player">要进入sceneID场景的玩家实体</param>
        /// <param name="name">场景ID，要切换到的场景号或场景名称</param>
        /// <returns></returns>
        public Scene JoinScene(Player player, string name) => SwitchScene(player, name);

        public Scene JoinScene(Player player, Scene scene) => SwitchScene(player, scene);

        /// <summary>
        /// 进入场景 - 成功进入返回true，进入失败返回false
        /// </summary>
        /// <param name="player">要进入sceneID场景的玩家实体</param>
        /// <param name="name">场景ID，要切换到的场景号或场景名称</param>
        /// <returns></returns>
        public Scene EnterScene(Player player, string name) => SwitchScene(player, name);

        public Scene EnterScene(Player player, Scene scene) => SwitchScene(player, scene);
        
        /// <summary>
        /// 切换场景
        /// </summary>
        /// <param name="player">要操作的玩家</param>
        /// <param name="name">场景名称</param>
        /// <returns>进入的场景,如果查询的场景不存在则为null</returns>
        public Scene SwitchScene(Player player, string name)
        {
            return SwitchScene(player, name, out _);
        }

        /// <summary>
        /// 切换场景
        /// </summary>
        /// <param name="player">要操作的玩家</param>
        /// <param name="name">场景名称</param>
        /// <param name="oldScene">上次所在的场景</param>
        /// <returns>进入的场景,如果查询的场景不存在则为null</returns>
        public Scene SwitchScene(Player player, string name, out Scene oldScene)
        {
            oldScene = player.Scene as Scene;
            if (string.IsNullOrEmpty(name))
                return null;
            if (Scenes.TryGetValue(name, out Scene scene1))
                return SwitchScene(player, scene1, out _);
            return null;
        }

        /// <summary>
        /// 切换场景
        /// </summary>
        /// <param name="player">要操作的玩家</param>
        /// <param name="enterScene">要进入的场景</param>
        /// <returns>进入的场景</returns>
        public Scene SwitchScene(Player player, Scene enterScene)
        {
            return SwitchScene(player, enterScene, out _);
        }

        /// <summary>
        /// 切换场景
        /// </summary>
        /// <param name="player">要操作的玩家</param>
        /// <param name="enterScene">要进入的场景</param>
        /// <param name="oldScene">上次所在的场景</param>
        /// <returns>进入的场景</returns>
        public Scene SwitchScene(Player player, Scene enterScene, out Scene oldScene)
        {
            oldScene = player.Scene as Scene;
            if (oldScene != null)
                oldScene.Remove(player);
            enterScene.AddPlayer(player);
            return enterScene;
        }

        /// <summary>
        /// 退出场景 exitCurrentSceneCall回调时已经不包含player对象
        /// </summary>
        /// <param name="player"></param>
        /// <param name="isEntMain">退出当前场景是否进入主场景: 默认进入主场景</param>
        /// <param name="exitCurrentSceneCall">即将退出当前场景的处理委托函数: 如果你需要对即将退出的场景进行一些事后处理, 则在此委托函数执行! 如:退出当前场景通知当前场景内的其他客户端将你的玩家对象移除等功能</param>
        public void ExitScene(Player player, bool isEntMain = true, Action<Scene> exitCurrentSceneCall = null)
        {
            RemoveScenePlayer(player, isEntMain, exitCurrentSceneCall);
        }

        /// <summary>
        /// 移除服务器场景. 从服务器总场景字典中移除指定的场景: 当你移除指定场景后,如果场景内有其他玩家在内, 则把其他玩家添加到主场景内
        /// </summary>
        /// <param name="name">要移除的场景id</param>
        /// <param name="addToMainScene">允许即将移除的场景内的玩家添加到主场景?</param>
        /// <param name="exitCurrentSceneCall">即将退出当前场景的处理委托函数: 如果你需要对即将退出的场景进行一些事后处理, 则在此委托函数执行! 如:退出当前场景通知当前场景内的其他客户端将你的玩家对象移除等功能</param>
        /// <returns></returns>
        public bool RemoveScene(string name, bool addToMainScene = true, Action<Scene> exitCurrentSceneCall = null)
        {
            if (Scenes.TryRemove(name, out Scene scene))
            {
                exitCurrentSceneCall?.Invoke(scene);
                if (addToMainScene)
                {
                    Scene mainScene = Scenes[MainSceneName];
                    foreach (Player p in scene.Players)
                    {
                        mainScene.AddPlayer(p);
                    }
                }
                else
                {
                    foreach (Player p in scene.Players)
                    {
                        scene.OnRemove(p);
                        p.OnRemove();
                        p.Scene = null;
                        p.SceneName = string.Empty;
                    }
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 将玩家从当前所在的场景移除掉， 移除之后此客户端将会进入默认主场景 call回调时已经不包含player对象
        /// </summary>
        /// <param name="player">要执行的玩家实体</param>
        /// <param name="isEntMain">退出当前场景是否进入主场景: 默认进入主场景</param>
        /// <param name="exitCurrentSceneCall">即将退出当前场景的处理委托函数: 如果你需要对即将退出的场景进行一些事后处理, 则在此委托函数执行! 如:退出当前场景通知当前场景内的其他客户端将你的玩家对象移除等功能</param>
        /// <returns></returns>
        public bool RemoveScenePlayer(Player player, bool isEntMain = true, Action<Scene> exitCurrentSceneCall = null)
        {
            if (string.IsNullOrEmpty(player.SceneName))
                return false;
            if (Scenes.TryGetValue(player.SceneName, out Scene scene))
            {
                scene.Remove(player);
                exitCurrentSceneCall?.Invoke(scene);
                if (isEntMain)
                {
                    Scene mainScene = Scenes[MainSceneName];
                    mainScene.AddPlayer(player);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 从所有在线玩家字典中删除(移除)玩家实体
        /// </summary>
        /// <param name="player"></param>
        public void DeletePlayer(Player player) => RemoveClient(player);

        /// <summary>
        /// 从所有在线玩家字典中移除玩家实体
        /// </summary>
        /// <param name="player"></param>
        public void RemovePlayer(Player player) => RemoveClient(player);

        /// <summary>
        /// 从客户端字典中移除客户端
        /// </summary>
        /// <param name="client"></param>
        public virtual void RemoveClient(Player client)
        {
            if (!AllClients.TryRemove(client.RemotePoint, out _))//防止两次进入
                return;
            Players.TryRemove(client.PlayerID, out _);
            UIDClients.TryRemove(client.UserID, out _);
            OnRemoveClientHandle(client);
            client.OnRemoveClient();
            ExitScene(client, false);
            client.Dispose();
            UserIDStack.Push(client.UserID);
            if (client.IsQueueUp)
                return;
        J: if (QueueUp.TryDequeue(out var client1))
            {
                if (client1.isDispose)
                    goto J;
                if (client1.CloseReceive | client1.CloseSend)
                    goto J;
                client1.QueueUpNo = 0;
                SendRT(client1, NetCmd.QueueCancellation, new byte[0]);
            }
        }

        /// <summary>
        /// 场景是否存在?
        /// </summary>
        /// <param name="sceneID"></param>
        /// <returns></returns>
        public bool IsHasScene(string sceneID)
        {
            return Scenes.ContainsKey(sceneID);
        }

        /// <summary>
        /// 关闭服务器
        /// </summary>
        public virtual void Close()
        {
            IsRunServer = false;
            foreach (Player client in AllClients.Values)
                client.Dispose();
            AllClients.Clear();
            Players.Clear();
            UIDClients.Clear();
            Thread.Sleep(50);//等待线程退出后再关闭套接字, 解决在revd方法出错
            if (Server != null)
            {
                Server.Dispose();
                Server.Close();
                Server = null;
            }
            if (ServerArgs != null)
            {
                ServerArgs.Dispose();
                ServerArgs = null;
                #region 清除iocp完成端口绑定事件池缓存
                var overlappedType = typeof(Overlapped);
                var overlappedDataCacheField = overlappedType.GetField("s_overlappedDataCache", BindingFlags.NonPublic | BindingFlags.Static);
                var s_overlappedDataCache = overlappedDataCacheField.GetValue(null);
                var overlappedDataCacheType = s_overlappedDataCache.GetType();
                var m_FreeListField = overlappedDataCacheType.GetField("m_FreeList", BindingFlags.NonPublic | BindingFlags.Instance);
                var m_FreeList = m_FreeListField.GetValue(s_overlappedDataCache) as ConcurrentStack<object>;
                m_FreeList.Clear();
                var m_NotGen2Field = overlappedDataCacheType.GetField("m_NotGen2", BindingFlags.NonPublic | BindingFlags.Instance);
                var m_NotGen2 = m_NotGen2Field.GetValue(s_overlappedDataCache) as List<object>;
                m_NotGen2.Clear();
                #endregion
            }
            if (this == Instance)//有多个服务器实例, 需要
                Instance = null;
            foreach (var item in threads)
                item.Value.Abort();
            threads.Clear();
            RcvQueues.Clear();
            OnStartingHandle -= OnStarting;
            OnStartupCompletedHandle -= OnStartupCompleted;
            OnHasConnectHandle -= OnHasConnect;
            OnRemoveClientHandle -= OnRemoveClient;
            OnOperationSyncHandle -= OnOperationSync;
            OnRevdBufferHandle -= OnReceiveBuffer;
            OnReceiveFileHandle -= OnReceiveFile;
            OnRevdRTProgressHandle -= OnRevdRTProgress;
            OnSendRTProgressHandle -= OnSendRTProgress;
            Debug.Log("服务器已关闭！");//先打印在移除事件
            Thread.Sleep(100);
            Debug.LogHandle -= Log;
            Debug.LogWarningHandle -= Log;
            Debug.LogErrorHandle -= Log;
        }

        /// <summary>
        /// 发送网络数据
        /// </summary>
        /// <param name="client">发送数据到的客户端</param>
        /// <param name="buffer">数据缓冲区</param>
        public virtual void Send(Player client, byte[] buffer)
        {
            Send(client, NetCmd.OtherCmd, buffer);
        }

        /// <summary>
        /// 发送自定义网络数据
        /// </summary>
        /// <param name="client">发送到客户端</param>
        /// <param name="cmd">网络命令</param>
        /// <param name="buffer">数据缓冲区</param>
        public virtual void Send(Player client, byte cmd, byte[] buffer)
        {
            if (client.CloseSend)
                return;
            if (client.udpRPCModels.Count >= LimitQueueCount)
            {
                Debug.LogError($"[{client}]数据缓存列表超出限制!");
                return;
            }
            if (buffer.Length > 65507)
            {
                Debug.LogError($"[{client}]数据太大，请分块发送!");
                return;
            }
            client.udpRPCModels.Enqueue(new RPCModel(cmd, buffer) { bigData = buffer.Length > short.MaxValue });
        }

        /// <summary>
        /// 发送自定义网络数据
        /// </summary>
        /// <param name="client">发送到客户端</param>
        /// <param name="buffer">数据缓冲区</param>
        public virtual void Send(Player client, bool reliable, byte[] buffer)
        {
            if (reliable)
                SendRT(client, buffer);
            else
                Send(client, buffer);
        }

        /// <summary>
        /// 发送自定义网络数据
        /// </summary>
        /// <param name="client">发送到客户端</param>
        /// <param name="cmd">网络命令</param>
        /// <param name="buffer">数据缓冲区</param>
        public virtual void Send(Player client, byte cmd, bool reliable, byte[] buffer)
        {
            if (reliable)
                SendRT(client, cmd, buffer);
            else
                Send(client, cmd, buffer);
        }

        /// <summary>
        /// 发送网络数据
        /// </summary>
        /// <param name="client">发送数据到的客户端</param>
        /// <param name="func">RPCFun函数</param>
        /// <param name="pars">RPCFun参数</param>
        public virtual void Send(Player client, string func, params object[] pars)
        {
            Send(client, NetCmd.CallRpc, func, pars);
        }

        /// <summary>
        /// 发送网络数据
        /// </summary>
        /// <param name="client">发送到的客户端</param>
        /// <param name="cmd">网络命令</param>
        /// <param name="func">RPCFun函数</param>
        /// <param name="pars">RPCFun参数</param>
        public virtual void Send(Player client, byte cmd, string func, params object[] pars)
        {
            if (client.CloseSend)
                return;
            if (client.udpRPCModels.Count >= LimitQueueCount)
            {
                Debug.LogError($"[{client}]数据缓存列表超出限制!");
                return;
            }
            client.udpRPCModels.Enqueue(new RPCModel(cmd, func, pars));
        }

        public virtual void Send(Player client, ushort methodHash, params object[] pars)
        {
            Send(client, NetCmd.CallRpc, methodHash, pars);
        }

        public virtual void Send(Player client, byte cmd, ushort methodHash, params object[] pars)
        {
            if (client.CloseSend)
                return;
            if (client.udpRPCModels.Count >= LimitQueueCount)
            {
                Debug.LogError($"[{client}]数据缓存列表超出限制!");
                return;
            }
            client.udpRPCModels.Enqueue(new RPCModel(cmd, methodHash, pars));
        }

        /// <summary>
        /// 发送网络数据
        /// </summary>
        /// <param name="client">发送到的客户端</param>
        /// <param name="func">RPCFun函数</param>
        /// <param name="pars">RPCFun参数</param>
        public virtual void Send(Player client, bool reliable, string func, params object[] pars)
        {
            if (reliable)
                SendRT(client, func, pars);
            else
                Send(client, func, pars);
        }

        /// <summary>
        /// 发送网络数据
        /// </summary>
        /// <param name="client">发送到的客户端</param>
        /// <param name="cmd">网络命令</param>
        /// <param name="func">RPCFun函数</param>
        /// <param name="pars">RPCFun参数</param>
        public virtual void Send(Player client, byte cmd, bool reliable, string func, params object[] pars)
        {
            if (reliable)
                SendRT(client, cmd, func, pars);
            else
                Send(client, cmd, func, pars);
        }

        /// <summary>
        /// 发送灵活数据包
        /// </summary>
        /// <param name="client">客户端集合</param>
        /// <param name="cmd"></param>
        /// <param name="buffer">要包装的数据,你自己来定</param>
        /// <param name="kernel">内核? 你包装的数据在客户端是否被内核NetConvert序列化?</param>
        /// <param name="serialize">序列化? 你包装的数据是否在服务器即将发送时NetConvert序列化?</param>
        public void Send(Player client, byte cmd, byte[] buffer, bool kernel, bool serialize)
        {
            if (client.CloseSend)
                return;
            if (client.udpRPCModels.Count >= LimitQueueCount)
            {
                Debug.LogError($"[{client}]数据缓存列表超出限制!");
                return;
            }
            if (buffer.Length > 65507)
            {
                Debug.LogError($"[{client}]数据太大，请分块发送!");
                return;
            }
            client.udpRPCModels.Enqueue(new RPCModel(cmd, buffer, kernel, serialize) { bigData = buffer.Length > short.MaxValue });
        }

        public void Send(Player client, RPCModel model)
        {
            if (client.CloseSend)
                return;
            if (client.udpRPCModels.Count >= LimitQueueCount)
            {
                Debug.LogError($"[{client}]数据缓存列表超出限制!");
                return;
            }
            client.udpRPCModels.Enqueue(model);
        }

        /// <summary>
        /// 发送网络可靠传输数据, 可以发送大型文件数据
        /// 调用此方法通常情况下是一定把数据发送成功为止, 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="func">函数名</param>
        /// <param name="pars">参数</param>
        public virtual void SendRT(Player client, string func, params object[] pars)
        {
            SendRT(client, NetCmd.CallRpc, func, pars);
        }

        /// <summary>
        /// 发送可靠网络传输, 可以发送大型文件数据
        /// 调用此方法通常情况下是一定把数据发送成功为止, 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="cmd">网络命令</param>
        /// <param name="func">函数名</param>
        /// <param name="pars">参数</param>
        public virtual void SendRT(Player client, byte cmd, string func, params object[] pars)
        {
            if (client.CloseSend)
                return;
            if (client.tcpRPCModels.Count >= LimitQueueCount)
            {
                Debug.LogError($"[{client}]数据缓存列表超出限制!");
                return;
            }
            client.tcpRPCModels.Enqueue(new RPCModel(cmd, func, pars, true, true));
        }

        public virtual void SendRT(Player client, ushort methodHash, params object[] pars)
        {
            SendRT(client, NetCmd.CallRpc, methodHash, pars);
        }

        public virtual void SendRT(Player client, byte cmd, ushort methodHash, params object[] pars)
        {
            if (client.CloseSend)
                return;
            if (client.tcpRPCModels.Count >= LimitQueueCount)
            {
                Debug.LogError($"[{client}]数据缓存列表超出限制!");
                return;
            }
            client.tcpRPCModels.Enqueue(new RPCModel(cmd, string.Empty, pars, true, true, methodHash));
        }

        /// <summary>
        /// 发送可靠网络传输, 可发送大数据流
        /// 调用此方法通常情况下是一定把数据发送成功为止, 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="buffer"></param>
        public virtual void SendRT(Player client, byte[] buffer)
        {
            SendRT(client, NetCmd.OtherCmd, buffer);
        }

        /// <summary>
        /// 发送可靠网络传输, 可发送大数据流
        /// 调用此方法通常情况下是一定把数据发送成功为止, 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="cmd">网络命令</param>
        /// <param name="buffer"></param>
        public virtual void SendRT(Player client, byte cmd, byte[] buffer)
        {
            if (client.CloseSend)
                return;
            if (client.tcpRPCModels.Count >= LimitQueueCount)
            {
                Debug.LogError($"[{client}]数据缓存列表超出限制!");
                return;
            }
            if (buffer.Length / MTU > LimitQueueCount)
            {
                Debug.LogError($"[{client}]数据太大，请分块发送!");
                return;
            }
            client.tcpRPCModels.Enqueue(new RPCModel(cmd, buffer, false, false) { bigData = buffer.Length > short.MaxValue });
        }

        /// <summary>
        /// 发送灵活数据包
        /// </summary>
        /// <param name="client">客户端集合</param>
        /// <param name="cmd"></param>
        /// <param name="buffer">要包装的数据,你自己来定</param>
        /// <param name="kernel">内核? 你包装的数据在客户端是否被内核NetConvert序列化?</param>
        /// <param name="serialize">序列化? 你包装的数据是否在服务器即将发送时NetConvert序列化?</param>
        public void SendRT(Player client, byte cmd, byte[] buffer, bool kernel, bool serialize)
        {
            if (client.CloseSend)
                return;
            if (client.tcpRPCModels.Count >= LimitQueueCount)
            {
                Debug.LogError($"[{client}]数据缓存列表超出限制!");
                return;
            }
            if (buffer.Length / MTU > LimitQueueCount)
            {
                Debug.LogError($"[{client}]数据太大，请分块发送!");
                return;
            }
            client.tcpRPCModels.Enqueue(new RPCModel(cmd, buffer, kernel, serialize) { bigData = buffer.Length > short.MaxValue });
        }

        public void SendRT(Player client, RPCModel model)
        {
            if (client.CloseSend)
                return;
            if (client.tcpRPCModels.Count >= LimitQueueCount)
            {
                Debug.LogError($"[{client}]数据缓存列表超出限制!");
                return;
            }
            client.tcpRPCModels.Enqueue(model);
        }

        /// <summary>
        /// 网络多播, 发送自定义数据到clients集合的客户端
        /// </summary>
        /// <param name="clients">客户端集合</param>
        /// <param name="buffer">自定义字节数组</param>
        public virtual void Multicast(IList<Player> clients, byte[] buffer)
        {
            Multicast(clients, false, NetCmd.CallRpc, buffer);
        }

        /// <summary>
        /// 网络多播, 发送自定义数据到clients集合的客户端
        /// </summary>
        /// <param name="clients">客户端集合</param>
        /// <param name="cmd"></param>
        /// <param name="buffer">自定义字节数组</param>
        public virtual void Multicast(IList<Player> clients, byte cmd, byte[] buffer)
        {
            Multicast(clients, false, cmd, buffer);
        }

        /// <summary>
        /// 网络多播, 发送自定义数据到clients集合的客户端
        /// </summary>
        /// <param name="clients">客户端集合</param>
        /// <param name="reliable"></param>
        /// <param name="buffer">自定义字节数组</param>
        public virtual void Multicast(IList<Player> clients, bool reliable, byte[] buffer)
        {
            Multicast(clients, reliable, NetCmd.OtherCmd, buffer);
        }

        /// <summary>
        /// 网络多播, 发送自定义数据到clients集合的客户端
        /// </summary>
        /// <param name="clients">客户端集合</param>
        /// <param name="reliable">使用可靠传输?</param>
        /// <param name="cmd">网络命令</param>
        /// <param name="buffer">自定义字节数组</param>
        public virtual void Multicast(IList<Player> clients, bool reliable, byte cmd, byte[] buffer)
        {
            Multicast(clients, reliable, new RPCModel(cmd, buffer, false, false) { bigData = buffer.Length > short.MaxValue });
        }

        /// <summary>
        /// 网络多播, 发送数据到clients集合的客户端 (灵活数据包)
        /// </summary>
        /// <param name="clients">客户端集合</param>
        /// <param name="reliable"></param>
        /// <param name="cmd"></param>
        /// <param name="buffer">要包装的数据,你自己来定</param>
        /// <param name="kernel">内核? 你包装的数据在客户端是否被内核NetConvert序列化?</param>
        /// <param name="serialize">序列化? 你包装的数据是否在服务器即将发送时NetConvert序列化?</param>
        public virtual void Multicast(IList<Player> clients, bool reliable, byte cmd, byte[] buffer, bool kernel, bool serialize)
        {
            Multicast(clients, reliable, new RPCModel(cmd, buffer, kernel, serialize) { bigData = buffer.Length > short.MaxValue });
        }

        /// <summary>
        /// 网络多播, 发送数据到clients集合的客户端
        /// </summary>
        /// <param name="clients">客户端集合</param>
        /// <param name="func">本地客户端rpc函数</param>
        /// <param name="pars">本地客户端rpc参数</param>
        public virtual void Multicast(IList<Player> clients, string func, params object[] pars)
        {
            Multicast(clients, false, NetCmd.CallRpc, func, pars);
        }

        /// <summary>
        /// 网络多播, 发送数据到clients集合的客户端
        /// </summary>
        /// <param name="clients">客户端集合</param>
        /// <param name="reliable">使用可靠传输?</param>
        /// <param name="func">本地客户端rpc函数</param>
        /// <param name="pars">本地客户端rpc参数</param>
        public virtual void Multicast(IList<Player> clients, bool reliable, string func, params object[] pars)
        {
            Multicast(clients, reliable, NetCmd.CallRpc, func, pars);
        }

        /// <summary>
        /// 网络多播, 发送数据到clients集合的客户端
        /// </summary>
        /// <param name="clients">客户端集合</param>
        /// <param name="cmd">网络命令</param>
        /// <param name="func">本地客户端rpc函数</param>
        /// <param name="pars">本地客户端rpc参数</param>
        public virtual void Multicast(IList<Player> clients, byte cmd, string func, params object[] pars)
        {
            Multicast(clients, false, cmd, func, pars);
        }

        /// <summary>
        /// 网络多播, 发送数据到clients集合的客户端
        /// </summary>
        /// <param name="clients">客户端集合</param>
        /// <param name="reliable">使用可靠传输?</param>
        /// <param name="cmd">网络命令</param>
        /// <param name="func">本地客户端rpc函数</param>
        /// <param name="pars">本地客户端rpc参数</param>
        public virtual void Multicast(IList<Player> clients, bool reliable, byte cmd, string func, params object[] pars)
        {
            byte[] buffer = OnSerializeRpc(new RPCModel(1, func, pars));
            Multicast(clients, reliable, new RPCModel(cmd, buffer, true, false));
        }

        public virtual void Multicast(IList<Player> clients, bool reliable, ushort methodHash, params object[] pars)
        {
            Multicast(clients, reliable, new RPCModel(NetCmd.CallRpc, methodHash, pars));
        }

        public virtual void Multicast(IList<Player> clients, bool reliable, byte cmd, ushort methodHash, params object[] pars)
        {
            Multicast(clients, reliable, new RPCModel(cmd, methodHash, pars));
        }

        public virtual void Multicast(IList<Player> clients, bool reliable, RPCModel model)
        {
            if (model.buffer != null)
            {
                if (model.buffer.Length / MTU > LimitQueueCount)
                {
                    Debug.LogError("Multocast数据太大，请分块发送!");
                    return;
                }
            }
            for (int i = 0; i < clients.Count; i++)
            {
                var client = clients[i];
                if (client == null)
                    continue;
                if (client.CloseSend)
                    continue;
                if (!reliable)
                {
                    if (client.udpRPCModels.Count >= LimitQueueCount)
                    {
                        Debug.LogError($"[{client}]数据缓存列表超出限制!");
                        return;
                    }
                    client.udpRPCModels.Enqueue(model);
                }
                else
                {
                    if (client.tcpRPCModels.Count >= LimitQueueCount)
                    {
                        Debug.LogError($"[{client}]数据缓存列表超出限制!");
                        return;
                    }
                    client.tcpRPCModels.Enqueue(model);
                }
            }
        }

        /// <summary>
        /// 添加Rpc
        /// </summary>
        /// <param name="target">注册的对象实例</param>
        /// <param name="append">一个Rpc方法是否可以多次添加到Rpcs里面？</param>
        public void AddRpc(object target, bool append = false, Action<SyncVarInfo> onSyncVarCollect = null)
        {
            AddRpcHandle(target, append, onSyncVarCollect);
        }

        /// <summary>
        /// 添加网络Rpc(注册远程方法)
        /// </summary>
        /// <param name="target">注册的对象实例</param>
        public void AddRpcHandle(object target)
        {
            AddRpcHandle(target, false);
        }

        /// <summary>
        /// 添加网络Rpc(注册远程方法)
        /// </summary>
        /// <param name="target">注册的对象实例</param>
        /// <param name="append">一个Rpc方法是否可以多次添加到Rpcs里面？</param>
        public void AddRpcHandle(object target, bool append, Action<SyncVarInfo> onSyncVarCollect = null)
        {
            if (OnAddRpcHandle == null)
                OnAddRpcHandle = AddRpcInternal;
            OnAddRpcHandle(target, append, onSyncVarCollect);
        }

        protected void AddRpcInternal(object target, bool append, Action<SyncVarInfo> onSyncVarCollect = null)
        {
            RpcHelper.AddRpc(this, target, append, onSyncVarCollect);
        }

        /// <summary>
        /// 移除对象的Rpc注册
        /// </summary>
        /// <param name="target">将此对象的所有带有RPCFun特性的函数移除</param>
        public void RemoveRpc(object target)
        {
            if (OnRemoveRpc == null)
                OnRemoveRpc = RemoveRpcInternal;
            OnRemoveRpc(target);
        }

        protected void RemoveRpcInternal(object target)
        {
            RpcHelper.RemoveRpc(this, target);
        }

        //public void CheckRpc() 
        //{
        //    RpcHelper.CheckRpc(this);
        //}

        /// <summary>
        /// playerID玩家是否在线?
        /// </summary>
        /// <param name="playerID"></param>
        /// <returns></returns>
        public virtual bool IsOnline(string playerID)
        {
            return Players.ContainsKey(playerID);
        }

        /// <summary>
        /// playerID玩家是否在线? 并且如果在线则out 在线玩家的对象
        /// </summary>
        /// <param name="playerID"></param>
        /// <returns></returns>
        public virtual bool IsOnline(string playerID, out Player client)
        {
            return Players.TryGetValue(playerID, out client);
        }

        /// <summary>
		/// 强制下线处理, 将client客户端从在线字段<see cref="Players"/>和<see cref="UIDClients"/>和<see cref="AllClients"/>字段中移除
		/// </summary>
		/// <param name="client"></param>
		public virtual void OfflineHandle(Player client)
        {
            SendDirect(client);
            RemoveClient(client);
            Debug.Log("[" + client.Name + "]被强制下线...!");
        }

        /// <summary>
        /// 退出登录, 将client客户端从在线字段<see cref="Players"/>和<see cref="UIDClients"/>字段中移除
        /// </summary>
        /// <param name="client"></param>
        public virtual void SignOut(Player client)
        {
            if (!client.Login)
                return;
            SendDirect(client);
            Players.TryRemove(client.PlayerID, out _);
            UIDClients.TryRemove(client.UserID, out _);
            ExitScene(client, false);
            client.Login = false;
            OnSignOut(client);
            client.OnSignOut();
            Debug.Log("[" + client.Name + "]退出登录...!");
        }

        /// <summary>
        /// 当客户端退出登录
        /// </summary>
        /// <param name="client"></param>
        public virtual void OnSignOut(Player client)
        {
        }

        /// <summary>
        /// 设置心跳时间
        /// </summary>
        /// <param name="timeoutLimit">心跳检测次数, 默认检测5次</param>
        /// <param name="interval">心跳时间间隔, 每interval毫秒会检测一次</param>
        public void SetHeartTime(byte timeoutLimit, int interval)
        {
            HeartLimit = timeoutLimit;
            HeartInterval = interval;
            var evt = ThreadManager.Event.GetEvent(taskIDs[3]);
            if(evt != null)
                evt.timeMax = (ulong)interval;
        }

        /// <summary>
        /// ping测试网络延迟, 通过<see cref="OnPingCallback"/>事件回调
        /// </summary>
        /// <param name="client"></param>
        public void Ping(Player client)
        {
            uint tick = (uint)Environment.TickCount;
            Send(client, NetCmd.Ping, BitConverter.GetBytes(tick));
        }

        /// <summary>
        /// ping测试网络延迟, 此方法帮你监听<see cref="OnPingCallback"/>事件, 如果不使用的时候必须保证能移除委托, 建议不要用框名函数, 那样会无法移除委托
        /// </summary>
        /// <param name="client"></param>
        /// <param name="callback"></param>
        public void Ping(Player client, Action<Player, uint> callback)
        {
            uint tick = (uint)Environment.TickCount;
            Send(client, NetCmd.Ping, BitConverter.GetBytes(tick));
            OnPingCallback += callback;
        }

        internal void SetRAC(int length)
        {
            receiveCount += length;
            receiveAmount++;
        }

        /// <summary>
        /// 添加适配器
        /// </summary>
        /// <param name="adapter"></param>
        public void AddAdapter(IAdapter adapter)
        {
            if (adapter is ISerializeAdapter ser)
                AddAdapter(AdapterType.Serialize, ser);
            else if (adapter is IRPCAdapter<Player> rpc)
                AddAdapter(AdapterType.RPC, rpc);
            else throw new Exception("无法识别的适配器!， 注意: IRPCAdapter<Player>是服务器的RPC适配器，IRPCAdapter是客户端适配器！");
        }

        /// <summary>
        /// 添加适配器
        /// </summary>
        /// <param name="type"></param>
        /// <param name="adapter"></param>
        public void AddAdapter(AdapterType type, IAdapter adapter)
        {
            switch (type)
            {
                case AdapterType.Serialize:
                    var ser = (ISerializeAdapter)adapter;
                    OnSerializeRPC = ser.OnSerializeRpc;
                    OnDeserializeRPC = ser.OnDeserializeRpc;
                    OnSerializeOPT = ser.OnSerializeOpt;
                    OnDeserializeOPT = ser.OnDeserializeOpt;
                    break;
                case AdapterType.RPC:
                    var rpc = (IRPCAdapter<Player>)adapter;
                    OnAddRpcHandle = rpc.AddRpcHandle;
                    OnRPCExecute = rpc.OnRpcExecute;
                    OnRemoveRpc = rpc.RemoveRpc;
                    break;
            }
        }

        /// <summary>
        /// 字段,属性同步线程
        /// </summary>
        protected virtual bool SyncVarHandler()
        {
            try
            {
                foreach (var client in AllClients)
                {
                    if (client.Value == null)
                        continue;
                    var buffer = SyncVarHelper.CheckSyncVar(true, client.Value.SyncVarDic);
                    if (buffer != null)
                        SendRT(client.Value, NetCmd.SyncVarP2P, buffer);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            return IsRunServer;
        }

        /// <summary>
        /// 发送文件, 客户端可以使用事件<see cref="Client.ClientBase.OnReceiveFileHandle"/>来监听并处理
        /// </summary>
        /// <param name="client"></param>
        /// <param name="filePath"></param>
        /// <param name="bufferSize">每次发送数据大小</param>
        /// <returns></returns>
        public bool SendFile(Player client, string filePath, int bufferSize = 50000)
        {
            var path1 = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(path1))
            {
                Debug.LogError($"[{client}]文件不存在! 或者文件路径字符串编码错误! 提示:可以使用Notepad++查看, 编码是ANSI,不是UTF8");
                return false;
            }
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, bufferSize);
            var fileData = new FileData
            {
                ID = fileStream.GetHashCode(),
                fileStream = fileStream,
                fileName = Path.GetFileName(filePath),
                bufferSize = bufferSize
            };
            client.ftpDic.Add(fileData.ID, fileData);
            SendFile(client, fileData.ID, fileData);
            return true;
        }

        private void SendFile(Player client, int key, FileData fileData)
        {
            var fileStream = fileData.fileStream;
            bool complete = false;
            long bufferSize = fileData.bufferSize;
            if (fileStream.Position + fileData.bufferSize >= fileStream.Length)
            {
                bufferSize = fileStream.Length - fileStream.Position;
                complete = true;
            }
            byte[] buffer = new byte[bufferSize];
            fileStream.Read(buffer, 0, buffer.Length);
            var segment1 = BufferPool.Take((int)bufferSize + 50);
            segment1.Write(fileData.ID);
            segment1.Write(fileData.fileStream.Length);
            segment1.Write(fileData.fileName);
            segment1.Write(buffer);
            SendRT(client, NetCmd.SendFile, segment1.ToArray(true));
            if (complete)
            {
                OnSendFileProgress?.Invoke(client, new RTProgress(fileData.fileName, fileStream.Position / (float)fileStream.Length * 100f, RTState.Complete));
                client.ftpDic.Remove(key);
                fileData.fileStream.Close();
            }
            else if (Environment.TickCount >= sendFileTick)
            {
                sendFileTick = Environment.TickCount + 1000;
                OnSendFileProgress?.Invoke(client, new RTProgress(fileData.fileName, fileStream.Position / (float)fileStream.Length * 100f, RTState.Sending));
            }
        }

        /// <summary>
        /// 检查send方法的发送队列是否已到达极限, 到达极限则不允许新的数据放入发送队列, 需要等待队列消耗后才能放入新的发送数据
        /// </summary>
        /// <returns>是否可发送数据</returns>
        public bool CheckSend(Player client)
        {
            return client.udpRPCModels.Count < LimitQueueCount;
        }

        /// <summary>
        /// 检查send方法的发送队列是否已到达极限, 到达极限则不允许新的数据放入发送队列, 需要等待队列消耗后才能放入新的发送数据
        /// </summary>
        /// <returns>是否可发送数据</returns>
        public bool CheckSendRT(Player client)
        {
            return client.udpRPCModels.Count < LimitQueueCount;
        }

        /// <summary>
        /// 设置攻击防护(SYN-ACK攻击)
        /// </summary>
        /// <param name="synAttackProtect">0:不开启 1:系统通过减少重传次数和延迟未连接时路由缓冲项(route cache entry)防范SYN攻击 2:(Microsoft推荐使用此值)</param>
        /// <param name="tcpMaxConnectResponseRetransmissions">确定 TCP 重新传输未应答的 SYN-ACK（连接请求确认）的次数</param>
        /// <param name="tcpMaxHalfOpen">服务器可以保持多少个连接处于半开（SYN-RCVD）状态</param>
        /// <param name="tcpMaxHalfOpenRetried">确定服务器可以在半打开 (SYN-RCVD) 状态下保持多少连接, 此条目的值应小于TCPMaxHalfOpen条目的值</param>
        /// <param name="tcpMaxPortsExhausted">指定触发 SYN 洪水攻击保护所必须超过的 TCP 连接请求数的阈值。</param>
        public virtual void SetAttackProtect(int synAttackProtect = 1, int tcpMaxConnectResponseRetransmissions = 2, int tcpMaxHalfOpen = 500, int tcpMaxHalfOpenRetried = 400, int tcpMaxPortsExhausted = 5)
        {
# if WINDOWS
            RegistryKey hklm = Registry.LocalMachine;
            RegistryKey tcpParams = hklm.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters", true);
            tcpParams.SetValue("SynAttackProtect", synAttackProtect, RegistryValueKind.DWord);
            tcpParams.SetValue("TcpMaxConnectResponseRetransmissions", tcpMaxConnectResponseRetransmissions, RegistryValueKind.DWord);
            tcpParams.SetValue("TcpMaxHalfOpen", tcpMaxHalfOpen, RegistryValueKind.DWord);
            tcpParams.SetValue("TcpMaxHalfOpenRetried", tcpMaxHalfOpenRetried, RegistryValueKind.DWord);
            tcpParams.SetValue("TcpMaxPortsExhausted", tcpMaxPortsExhausted, RegistryValueKind.DWord);
            hklm.Close();
            tcpParams.Close();
#endif
        }
    }
}