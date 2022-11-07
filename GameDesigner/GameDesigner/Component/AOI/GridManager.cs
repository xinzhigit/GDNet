using System;
using System.Collections.Generic;
using Net.System;

namespace Net.AOI
{
    public enum GridType
    {
        Horizontal,
        Vertical
    }

    /// <summary>
    /// 九宫格网络同步管理器
    /// </summary>
    [Serializable]
    public class GridManager
    {
        public List<Grid> grids = new List<Grid>();
        public HashSetSafe<IGridBody> gridBodies = new HashSetSafe<IGridBody>();
        public Rect worldSize;
        public GridType gridType = GridType.Horizontal;

        /// <summary>
        /// 初始化九宫格
        /// </summary>
        /// <param name="xPos">x开始位置</param>
        /// <param name="zPos">z开始位置</param>
        /// <param name="xMax">x列最大值</param>
        /// <param name="zMax">z列最大值</param>
        /// <param name="width">格子长度</param>
        /// <param name="height">格子高度</param>
        public void Init(float xPos, float zPos, uint xMax, uint zMax, int width, int height)
        {
            grids.Clear();
            worldSize = new Rect(xPos, zPos, width * xMax, height * zMax);
            for (int z = 0; z < zMax; z++)
            {
                float xPos1 = xPos;
                for (int x = 0; x < xMax; x++)
                {
                    var rect = new Rect(xPos1, zPos, width, height);
                    grids.Add(new Grid() { rect = rect });
                    xPos1 += width;
                }
                zPos += height;
            }
            foreach (var item in grids)
            {
                var rect = new Rect(item.rect.x - width, item.rect.y - height, width * 3, height * 3);
                foreach (var item1 in grids)
                {
                    if (rect.Contains(item1.rect.position))
                    {
                        item.grids.Add(item1);
                    }
                }
            }
        }
        /// <summary>
        /// 插入物体到九宫格感兴趣区域
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public bool Insert(IGridBody body)
        {
            foreach (var grid in grids)
            {
                if (Contains(grid, body))
                {
                    grid.gridBodies.Add(body);
                    body.Grid = grid;
                    body.ID = gridBodies.Count;
                    gridBodies.Add(body);
                    foreach (var grid1 in grid.grids)
                    {
                        foreach (var item2 in grid1.gridBodies)//通知新格子, 此物体进来了
                        {
                            item2.OnEnter(body);
                        }
                    }
                    return true;
                }
            }
            return false;
        }
        public bool Contains(Grid grid, IGridBody body) 
        {
            if (gridType == GridType.Horizontal)
                return grid.rect.ContainsXZ(body.Position);
            return grid.rect.Contains(body.Position);
        }

        /// <summary>
        /// 获取物体的感兴趣九宫格区域
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public Grid TryGetGrid(IGridBody body)
        {
        JMP: if (body.Grid == null)
            {
                if (gridType == GridType.Horizontal)
                    if (!worldSize.ContainsXZ(body.Position))
                        goto J;
                else
                    if (!worldSize.Contains(body.Position))
                        goto J;
                foreach(var grid in grids)
                {
                    if (Contains(grid, body))
                    {
                        grid.gridBodies.Add(body);
                        body.Grid = grid;
                        foreach (var grid1 in grid.grids)
                        {
                            foreach (var item2 in grid1.gridBodies)//通知新格子, 此物体进来了
                            {
                                item2.OnEnter(body);
                            }
                        }
                        return body.Grid;
                    }
                }
                goto J;
            }
            if (Contains(body.Grid, body))
                return body.Grid;
            var oldGrids = body.Grid.grids;
            foreach (var grid in oldGrids)//遍历当前物体所在的九宫格
            {
                if (Contains(grid, body))//如果物体还在九宫格的其中一个格子
                {
                    var newGrids = grid.grids;
                    foreach (var grid1 in newGrids)//遍历当前所在的九宫格,如果是新进来的格子, 则通知新格子此物体进来了
                    {
                        if (!oldGrids.Contains(grid1))//如果当前为新格子, 则通知
                        {
                            foreach (var item2 in grid1.gridBodies)//通知新格子, 此物体进来了
                            {
                                item2.OnEnter(body);
                            }
                        }
                    }
                    foreach (var grid1 in oldGrids)//遍历当前所在的九宫格,如果已经离开的九宫格则调用离开方法
                    {
                        if (!newGrids.Contains(grid1))//如果已经离开了旧的格子
                        {
                            foreach (var item2 in grid1.gridBodies)//通知离开的格子的所有物体, 此物体离开了
                            {
                                item2.OnExit(body);
                            }
                        }
                    }
                    //更新物体所在的格子
                    body.Grid.gridBodies.Remove(body);
                    grid.gridBodies.Add(body);
                    body.Grid = grid;
                    return body.Grid;
                }
            }
            foreach (var grid in oldGrids)//拖拽瞬移过程
            {
                foreach (var item2 in grid.gridBodies)//通知离开的格子的所有物体, 此物体离开了
                {
                    item2.OnExit(body);
                }
            }
            body.Grid.gridBodies.Remove(body);
            body.Grid = null;
            goto JMP;
#if UNITY_EDITOR
        J: UnityEngine.Debug.Log($"{body.ID}越界了,位置:{body.Position}");
#else
        J: Event.NDebug.LogError($"{body.ID}越界了,位置:{body.Position}");
#endif
            return null;
        }
        /// <summary>
        /// 移除感兴趣物体
        /// </summary>
        /// <param name="body"></param>
        public void Remove(IGridBody body)
        {
            if (body.Grid == null)
                return;
            body.Grid.gridBodies.Remove(body);
            gridBodies.Remove(body);
        }
        /// <summary>
        /// 更新感兴趣的移除和添加物体
        /// </summary>
        public void UpdateHandler()
        {
            foreach (var body in gridBodies)
            {
                body.OnBodyUpdate();
                TryGetGrid(body);
            }
        }
    }
}
