#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA || UNITY_WEBGL
using UnityEngine;
using Net.AOI;
using Grid = Net.AOI.Grid;

namespace Net.Component
{
    public class AOIComponent : SingleCase<AOIComponent>
    {
        public GridManager gridManager = new GridManager();
        public float xPos = -100f;
        public float zPos = -100f;
        public uint xMax = 100;
        public uint zMax = 100;
        public int width = 20;
        public int height = 20;
        public bool EditInit;
        public bool showText;

        private void Awake()
        {
            gridManager.Init(xPos, zPos, xMax, zMax, width, height);
        }

        private void Update()
        {
            gridManager.UpdateHandler();
        }

        private void OnDrawGizmos()
        {
            if (EditInit)
            {
                EditInit = false;
                gridManager.Init(xPos, zPos, xMax, zMax, width, height);
            }
            Gizmos.color = Color.cyan;
            for (int i = 0; i < gridManager.grids.Count; i++)
            {
                Draw(gridManager.grids[i]);
            }
        }

        private void Draw(Grid grid)
        {
            var pos = grid.rect.center;
            var size = grid.rect.size;
            if(gridManager.gridType == GridType.Horizontal)
                Gizmos.DrawWireCube(new UnityEngine.Vector3(pos.x, 0f, pos.y), new UnityEngine.Vector3(size.x, 0, size.y));
            else
                Gizmos.DrawWireCube(new UnityEngine.Vector3(pos.x, pos.y, 0f), new UnityEngine.Vector3(size.x, size.y, 0));
#if UNITY_EDITOR
            if (showText) 
            {
                if (gridManager.gridType == GridType.Horizontal)
                    UnityEditor.Handles.Label(new UnityEngine.Vector3(grid.rect.x, 1f, grid.rect.y), grid.rect.position.ToString());
                else
                    UnityEditor.Handles.Label(new UnityEngine.Vector3(grid.rect.x, grid.rect.y, 0f), grid.rect.position.ToString());
            }
#endif
        }
    }
}
#endif