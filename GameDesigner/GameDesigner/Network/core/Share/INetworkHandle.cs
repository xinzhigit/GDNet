namespace Net.Share
{
    /// <summary>
    /// 网络状态处理接口
    /// </summary>
    public interface INetworkHandle
    {
        /// <summary>
        /// 当连接成功
        /// </summary>
        void OnConnected();
        /// <summary>
        /// 当连接失败
        /// </summary>
        void OnConnectFailed();
        /// <summary>
        /// 当连接中断
        /// </summary>
        void OnConnectLost();
        /// <summary>
        /// 当主动断开连接
        /// </summary>
        void OnDisconnect();
        /// <summary>
        /// 当尝试重连
        /// </summary>
        void OnTryToConnect();
        /// <summary>
        /// 当重连成功
        /// </summary>
        void OnReconnect();
        /// <summary>
        /// 当关闭连接
        /// </summary>
        void OnCloseConnect();
        /// <summary>
        /// 当排队时调用
        /// </summary>
        /// <param name="count"></param>
        void OnWhenQueuing(int totalCount, int count);
        /// <summary>
        /// 当排队结束调用
        /// </summary>
        void OnQueueCancellation();
        /// <summary>
        /// 当服务器爆炸，积极拒绝客户端连接
        /// </summary>
        void OnServerFull();
    }
}
