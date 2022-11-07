namespace Net.Share
{
    /// <summary>
    /// 网络命令基类 - 可继承此类定义自己的网络命令 19.7.16 (系统命令使用0-100) 请从100开始自定义命令
    /// </summary>
    public abstract class NetCmd
    {
        /// <summary>
        /// 面向实体类型调用远程函数
        /// 使用此命令即可在派生于<see cref="Net.Server.NetPlayer"/>类型定义<see cref="Rpc"/>函数进行调用
        /// </summary>
        public const byte EntityRpc = 0;
        /// <summary>
        /// 公共Rpc调用指令(<see langword="服务器的公共资源"/>)
        /// 如果是客户端调用则在服务器执行 如果是服务器调用则在客户端执行.
        /// 在服务器端,如果出现多线程抢夺资源调用Client错误时，可使用SafeCall命令来执行
        /// </summary>
        public const byte CallRpc = 1;
        /// <summary>
        /// 安全调用服务器函数(<see langword="针对Client独立调用"/>),当多线程并行时会有概率发生资源竞争，导致数据错乱！
        /// 如果在RPC函数内部调用client的时候是其他客户端的client对象。出现这种情况时建议使用此命令，
        /// 否则可以使用CallRpc命令，
        /// 使用此命令时,函数第一个参数必须是派生于<see cref="Net.Server.NetPlayer"/>类型的参数
        /// </summary>
        public const byte SafeCall = 2;
        /// <summary>
        /// (自身转发)服务器只转发给发送方客户端
        /// </summary>
        public const byte Local = 3;
        /// <summary>
        /// (场景转发)服务器负责转发给在同一房间或场景内的玩家
        /// </summary>
        public const byte Scene = 4;
        /// <summary>
        /// (场景转发,可靠指令)服务器负责转发给在同一房间或场景内的玩家
        /// </summary>
        public const byte SceneRT = 5;
        /// <summary>
        /// (公告指令)服务器负责转发给所有在线的玩家
        /// </summary>
        public const byte Notice = 6;
        /// <summary>
        /// (公告指令,可靠传输)服务器负责转发给所有在线的玩家
        /// </summary>
        public const byte NoticeRT = 7;
        /// <summary>
        /// 发送心跳包命令, 内部命令
        /// </summary>
        public const byte SendHeartbeat = 8;
        /// <summary>
        /// 回调心跳包命令, 内部命令
        /// </summary>
        public const byte RevdHeartbeat = 9;
        /// <summary>
        /// 多线程远程过程调用函数 (RPC)
        /// </summary>
        public const byte ThreadRpc = 10;
        /// <summary>
        /// 请求服务器移除此客户端
        /// </summary>
        public const byte QuitGame = 11;
        /// <summary>
        /// 其他命令或用户自定义命令
        /// </summary>
        public const byte OtherCmd = 12;
        /// <summary>
        /// Gcp可靠传输协议指令. 内部命令
        /// </summary>
        public const byte ReliableTransport = 15;
        
        /// <summary>
        /// 当客户端连接主服务器(网关服)时, 主服务器检测分区服务器在线人数如果处于爆满状态, 
        /// 服务器发送切换端口让客户端连接新的服务器IP和端口. 内部命令
        /// </summary>
        public const byte SwitchPort = 17;
        /// <summary>
        /// 标记客户端唯一标识, 内部命令
        /// </summary>
        public const byte Identify = 18;
        /// <summary>
        /// 操作同步，服务器使用NetScene.AddOperation方法，客户端UdpClient.AddOperation方法。 内部指令
        /// </summary>
        public const byte OperationSync = 19;
        /// <summary>
        /// 局域网寻找主机命令, 内部使用
        /// </summary>
        public const byte Broadcast = 20;
        /// <summary>
        /// 连接指令 (内部)
        /// </summary>
        public const byte Connect = 21;
        /// <summary>
        /// 断开网络连接, 内部指令
        /// </summary>
        public const byte Disconnect = 22;
        /// <summary>
        /// 自身转发, 可靠传输
        /// </summary>
        public const byte LocalRT = 23;
        /// <summary>
        /// ping测试网络延迟量
        /// </summary>
        public const byte Ping = 24;
        /// <summary>
        /// ping回调 内部指令
        /// </summary>
        public const byte PingCallback = 25;
        /// <summary>
        /// 当你客户端晚于其他客户端进入场景时，同步字段需要发起获取最新的值
        /// </summary>
        public const byte SyncVarGet = 26;
        /// <summary>
        /// 网络物体的同步指令 （NetworkObject之间同步）
        /// </summary>
        public const byte SyncVarNetObj = 27;

        public const byte P2P = 28;
        /// <summary>
        /// 字段或属性同步指令 内部指令 (客户端与服务器的Player字段同步)
        /// </summary>
        public const byte SyncVarP2P = 29;
        /// <summary>
        /// 发送文件
        /// </summary>
        public const byte SendFile = 30;
        /// <summary>
        /// 下载文件
        /// </summary>
        public const byte Download = 31;
        /// <summary>
        /// 单线程安全调用服务器函数,当多线程同时访问一个方法时, 感觉这个方法不安全时, 需要使用单线程处理时, 可使用此命令
        /// 否则可以使用SafeCall或CallRpc命令，
        /// 使用此命令时,函数第一个参数将会嵌入NetPlayer参数
        /// </summary>
        public const byte SingleCall = 32;
        /// <summary>
        /// 既有响应的rpc请求
        /// </summary>
        public const byte Response = 33;
        /// <summary>
        /// 网关转发
        /// </summary>
        public const byte GatewayRelay = 34;
        /// <summary>
        /// 如果被排队，服务器会发此命令给客户端
        /// </summary>
        public const byte QueueUp = 35;
        /// <summary>
        /// 如果排队解除，服务器会发此命令给客户端
        /// </summary>
        public const byte QueueCancellation = 36;
        /// <summary>
        /// 服务器爆满状态
        /// </summary>
        public const byte ServerFull = 37;
    }
}