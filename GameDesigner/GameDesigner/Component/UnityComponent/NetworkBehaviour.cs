#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA || UNITY_WEBGL
namespace Net.UnityComponent
{
    using global::System;
    using Net.Share;
    using UnityEngine;

    /// <summary>
    /// 网络行为基础组件
    /// </summary>
    [DefaultExecutionOrder(1000)]
    public abstract class NetworkBehaviour : MonoBehaviour
    {
        public NetworkObject netObj;
        private int index = -1;
        /// <summary>
        /// 此组件是netobj的第几个组件
        /// </summary>
        public int Index
        {
            get { return index; }
            set
            {
                if (value > 10)
                    throw new Exception("组件最多不能超过10个");
                index = value;
            }
        }
        /// <summary>
        /// 这个物体是本机生成的?
        /// true:这个物体是从你本机实例化后, 同步给其他客户端的, 其他客户端的IsLocal为false
        /// false:这个物体是其他客户端实例化后,同步到本机客户端上, IsLocal为false
        /// </summary>
        public bool IsLocal => netObj.isLocal;
        private bool isInit;
        private bool isEnabled;
        public virtual void Start()
        {
            Init();
        }
        private void OnValidate()
        {
            netObj = GetComponent<NetworkObject>();//在预制体上用GetComponentInParent获取不到自身组件，所以需要这样
            if (netObj != null)
                return;
            netObj = GetComponentInParent<NetworkObject>();
            if (netObj == null)
                netObj = gameObject.AddComponent<NetworkObject>();
        }
        public void Init(Operation opt = default)
        {
            if (isInit)
                return;
            isInit = true;
            netObj = GetComponentInParent<NetworkObject>();
            if (Index == -1)
            {
                Index = netObj.networkBehaviours.Count;
                netObj.networkBehaviours.Add(this);
            }
            else
            {
                while (netObj.networkBehaviours.Count <= Index)
                    netObj.networkBehaviours.Add(null);
                netObj.networkBehaviours[Index] = this;
            }
            netObj.InitSyncVar(this);
            if (IsLocal)
                OnNetworkObjectInit(netObj.Identity);
            else
                OnNetworkObjectCreate(opt);
        }
        /// <summary>
        /// 当网络物体被初始化, 只有本机实例化的物体才会被调用
        /// </summary>
        /// <param name="identity"></param>
        public virtual void OnNetworkObjectInit(int identity) { }
        /// <summary>
        /// 当网络物体被创建后调用, 只有其他客户端发送创建信息给本机后才会被调用
        /// </summary>
        /// <param name="opt"></param>
        public virtual void OnNetworkObjectCreate(Operation opt) { }
        /// <summary>
        /// 当网络操作到达后应当开发者进行处理
        /// </summary>
        /// <param name="opt"></param>
        public virtual void OnNetworkOperationHandler(Operation opt) { }
        /// <summary>
        /// 当属性自动同步检查
        /// </summary>
        public virtual void OnPropertyAutoCheck() { }
        /// <summary>
        /// 检查组件是否启用
        /// </summary>
        /// <returns></returns>
        public virtual bool CheckEnabled() { return isEnabled; }
        public virtual void OnEnable()
        {
            isEnabled = true;
        }
        public virtual void OnDisable()
        {
            isEnabled = false;
        }
        public virtual void OnDestroy()
        {
            netObj.RemoveSyncVar(this);
            var nbs = netObj.networkBehaviours;
            for (int i = 0; i < nbs.Count; i++)
            {
                var nb = nbs[i];
                nb.Index = i;
                if (nb == this)
                {
                    nbs.RemoveAt(i);
                    if (i >= 0) i--;
                }
            }
        }
    }
}
#endif