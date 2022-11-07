namespace Net.Share
{
    /// <summary>
    /// 流量控制模式
    /// </summary>
    public enum FlowControlMode
    {
        /// <summary>
        /// 正常模式 (类似Tcp)，当网络差时，会降低数据传输频率，节约网络流量
        /// </summary>
        Normal,
        /// <summary>
        /// 极速模式 (帧同步游戏)，如果网络差时，也会以一定速度进行数据传输，保证数据流畅
        /// </summary>
        Quick
    }
}
