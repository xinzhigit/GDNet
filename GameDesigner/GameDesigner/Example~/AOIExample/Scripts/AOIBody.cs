#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA || UNITY_WEBGL
namespace AOIExample
{
    using Net.AOI;
    using Net.Component;
    using UnityEngine;
    using Grid = Net.AOI.Grid;

    public class AOIBody : MonoBehaviour, IGridBody
    {
        public int ID { get; set; }
        public Net.Vector3 Position { get; set; }
        public Grid Grid { get; set; }

        public bool IsLocal;
        public bool ShowText = true;

        // Start is called before the first frame update
        void Start()
        {
            Position = transform.position;
            AOIComponent.I.gridManager.Insert(this);
            if (Grid != null)
            {
                if (!IsLocal)//如果是其他玩家
                {
                    bool hasLocal = false;
                    foreach (var grid in Grid.grids)
                    {
                        foreach (var body in grid.gridBodies)
                        {
                            var node = body as AOIBody;
                            if (node == null)
                                continue;
                            if (node.IsLocal)//如果在这里9宫格范围有本机玩家, 显示出来
                            {
                                hasLocal = true;
                                break;
                            }
                        }
                    }
                    GetComponent<MeshRenderer>().enabled = hasLocal;
                }
            }
        }

        void OnDestroy()
        {
            AOIComponent.I.gridManager.Remove(this);
        }

        void OnDrawGizmos() 
        {
            if (!IsLocal)
                return;
            if (Grid == null)
                return; 
            Gizmos.color = Color.green;
            for (int i = 0; i < Grid.grids.Count; i++)
            {
                Draw(Grid.grids[i]);
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (IsLocal)
                return;
            if (!Application.isPlaying)
                return;
            if (AOIComponent.I.gridManager == null)
                return;
            if (Grid == null)
                return;
            Gizmos.color = Color.green;
            for (int i = 0; i < Grid.grids.Count; i++)
            {
                Draw(Grid.grids[i]);
            } 
        }

        private void Draw(Grid grid)
        {
            var pos = grid.rect.center;
            var size = grid.rect.size;
            if (AOIComponent.I.gridManager.gridType == GridType.Horizontal)
                Gizmos.DrawWireCube(new Vector3(pos.x, 0.5f, pos.y), new Vector3(size.x, 0.5f, size.y));
            else 
            {
                if (ShowText)
                    Gizmos.DrawWireCube(new Vector3(pos.x, pos.y, 0), new Vector3(size.x, size.y, 1f));
                else
                    Gizmos.DrawCube(new Vector3(pos.x, pos.y, 0), new Vector3(size.x, size.y, 1f));
            }
#if UNITY_EDITOR
            if (AOIComponent.I.gridManager.gridType == GridType.Horizontal)
            {
                if (ShowText) UnityEditor.Handles.Label(new Vector3(grid.rect.x, 0.5f, grid.rect.y), grid.rect.position.ToString());
            }
            else 
            {
                if (ShowText) UnityEditor.Handles.Label(new Vector3(grid.rect.x, grid.rect.y, 0f), grid.rect.position.ToString());
            }
#endif
        }

        public void OnBodyUpdate()
        {
            Position = transform.position;
        }

        public void OnEnter(IGridBody body)
        {
            var node = body as AOIBody;
            if (IsLocal)//主角进来了
            {
                node.GetComponent<MeshRenderer>().enabled = true;
            }
            else if (node.IsLocal) 
            {
                GetComponent<MeshRenderer>().enabled = true;
            }
        }

        public void OnExit(IGridBody body)
        {
            var node = body as AOIBody;
            if (IsLocal)//如果退出主角范围, 则隐藏
            {
                node.GetComponent<MeshRenderer>().enabled = false;
            }
            else if (node.IsLocal) 
            {
                GetComponent<MeshRenderer>().enabled = false;
            }
        }
    }
}
#endif