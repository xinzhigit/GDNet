namespace Net.AOI
{
    /// <summary>
    /// 九宫格格子物体接口
    /// </summary>
    public interface IGridBody
    {
        int ID { get; set; }
        Vector3 Position { get; set; }
        Grid Grid { get; set; }
        /// <summary>
        /// 当更新方法, 可以更新位置=unity的transform.position
        /// </summary>
        void OnBodyUpdate();
        /// <summary>
        /// 当body物体进入感兴趣区域
        /// </summary>
        void OnEnter(IGridBody body);
        /// <summary>
        /// 当body物体退出感兴趣区域
        /// </summary>
        void OnExit(IGridBody body);
    }
}
