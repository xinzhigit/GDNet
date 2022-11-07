using Net.System;
using System.Collections.Generic;

namespace Net.AOI
{
    /// <summary>
    /// 格子类
    /// </summary>
    public class Grid
    {
        public Rect rect;
        public List<Grid> grids = new List<Grid>();//九宫格列表
        public HashSetSafe<IGridBody> gridBodies = new HashSetSafe<IGridBody>();//格子的物体
        public override string ToString()
        {
            return $"{rect}";
        }
        /// <summary>
        /// 获取九宫格的所有物体
        /// </summary>
        /// <returns></returns>
        public List<IGridBody> GetGridBodiesAll()
        {
            var gridBodies = new List<IGridBody>();
            foreach (var item in grids)
                gridBodies.AddRange(item.gridBodies);
            return gridBodies;
        }
    }
}
