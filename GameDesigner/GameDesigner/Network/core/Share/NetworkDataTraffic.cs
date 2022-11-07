namespace Net.Share
{
    /// <summary>
    /// 网络流量数据统计
    /// </summary>
    public delegate void NetworkDataTraffic(Dataflow dataflow);

    /// <summary>
    /// 当处理缓冲区数据
    /// </summary>
    /// <param name="client">处理此客户端的数据请求</param>
    /// <param name="model"></param>
    public delegate void RevdBufferHandle<Player>(Player client, RPCModel model);

    /// <summary>
    /// webSocket当处理缓冲区数据
    /// </summary>
    /// <param name="client">处理此客户端的数据请求</param>
    /// <param name="model"></param>
    public delegate void WSRevdBufferHandle<Player>(Player client, MessageModel model);

    /// <summary>
    /// 数据流量统计结构类
    /// </summary>
    public struct Dataflow
    {
        /// <summary>
        /// 每秒发送数据次数
        /// </summary>
        public int sendNumber;
        /// <summary>
        /// 每秒发送字节长度
        /// </summary>
        public int sendCount;
        /// <summary>
        /// 每秒接收数据次数
        /// </summary>
        public int receiveNumber;
        /// <summary>
        /// 每秒接收到的字节长度
        /// </summary>
        public int receiveCount;
        /// <summary>
        /// 解析RPC函数次数
        /// </summary>
        public int resolveNumber;
        /// <summary>
        /// 发送线程循环次数(FPS)
        /// </summary>
        public int sendLoopNum;
        /// <summary>
        /// 接收线程循环次数(FPS)
        /// </summary>
        public int revdLoopNum;
        /// <summary>
        /// 从启动到现在总流出的数据流量
        /// </summary>
        public long outflowTotal;
        /// <summary>
        /// 从启动到现在总流入的数据流量
        /// </summary>
        public long inflowTotal;
    }
}