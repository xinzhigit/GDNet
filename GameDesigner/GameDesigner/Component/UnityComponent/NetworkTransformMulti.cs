#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA || UNITY_WEBGL
namespace Net.UnityComponent
{
    using Net.Component;
    using Net.Share;
    using global::System;
    using UnityEngine;
    using Net.Client;

    /// <summary>
    /// 网络Transform同步组件基类
    /// </summary>
    [DefaultExecutionOrder(1000)]
    public class NetworkTransformMulti : NetworkTransformBase
    {
        public ChildTransform[] childs;

        public override void Start()
        {
            base.Start();
            for (int i = 0; i < childs.Length; i++)
            {
                childs[i].Init(i + 1);
            }
        }

        public override void Update()
        {
            if (netObj.Identity == -1 | currMode == SyncMode.None)
                return;
            if (currMode == SyncMode.Synchronized)
            {
                SyncTransform();
                for (int i = 0; i < childs.Length; i++)
                {
                    childs[i].SyncTransform();
                }
            }
            else if (Time.time > sendTime)
            {
                Check();
                for (int i = 0; i < childs.Length; i++)
                {
                    childs[i].Check(netObj.Identity, netObj.registerObjectIndex, Index);
                }
                sendTime = Time.time + (1f / rate);
            }
        }

        public override void StartSyncTransformState()
        {
            ClientBase.Instance.AddOperation(new Operation(Command.Transform, netObj.Identity, syncScale ? localScale : Net.Vector3.zero, syncPosition ? position : Net.Vector3.zero, syncRotation ? rotation : Net.Quaternion.zero)
            {
                cmd1 = (byte)currMode,
                index = netObj.registerObjectIndex,
                index1 = Index,
                uid = ClientBase.Instance.UID
            });
        }

        public override void OnNetworkOperationHandler(Operation opt)
        {
            if (ClientBase.Instance.UID == opt.uid)
                return;
            sendTime = Time.time + interval;
            if (opt.index2 == 0)
            {
                netPosition = opt.position;
                netRotation = opt.rotation;
                netLocalScale = opt.direction;
                if (currMode == SyncMode.SynchronizedAll | currMode == SyncMode.Control)
                    SyncControlTransform();
                else if (currMode == SyncMode.None)
                {
                    var mode1 = (SyncMode)opt.cmd1;
                    if (mode1 == SyncMode.Control | mode1 == SyncMode.SynchronizedAll)
                        currMode = SyncMode.SynchronizedAll;
                    else
                        currMode = SyncMode.Synchronized;
                }
            }
            else
            {
                var child = childs[opt.index2 - 1];
                child.netPosition = opt.position;
                child.netRotation = opt.rotation;
                child.netLocalScale = opt.direction;
                if (child.mode == SyncMode.SynchronizedAll | child.mode == SyncMode.Control)
                    child.SyncControlTransform();
            }
        }
    }

    [Serializable]
    public class ChildTransform
    {
        public string name;
        public Transform transform;
        internal Net.Vector3 position;
        internal Net.Quaternion rotation;
        internal Net.Vector3 localScale;
        public SyncMode mode = SyncMode.Control;
        public bool syncPosition = true;
        public bool syncRotation = true;
        public bool syncScale = false;
        [DisplayOnly] public int identity = -1;//自身id
        internal Net.Vector3 netPosition;
        internal Net.Quaternion netRotation;
        internal Net.Vector3 netLocalScale;

        internal void Init(int identity)
        {
            this.identity = identity;
            position = transform.localPosition;
            rotation = transform.localRotation;
            localScale = transform.localScale;
        }

        internal void Check(int identity, int index, int netIndex)
        {
            if (transform.localPosition != position | transform.localRotation != rotation | transform.localScale != localScale)
            {
                position = transform.localPosition;
                rotation = transform.localRotation;
                localScale = transform.localScale;
                ClientBase.Instance.AddOperation(new Operation(Command.Transform, identity, syncScale ? localScale : Net.Vector3.zero, syncPosition ? position : Net.Vector3.zero, syncRotation ? rotation : Net.Quaternion.zero)
                {
                    cmd1 = (byte)mode,
                    uid = ClientBase.Instance.UID,
                    index = index,
                    index1 = netIndex,
                    index2 = this.identity
                });
            }
        }

        public void SyncTransform()
        {
            if (syncPosition)
                transform.localPosition = Vector3.Lerp(transform.localPosition, netPosition, 0.3f);
            if (syncRotation)
                if (netRotation != Net.Quaternion.identity)
                    transform.localRotation = Quaternion.Lerp(transform.localRotation, netRotation, 0.3f);
            if (syncScale)
                transform.localScale = netLocalScale;
        }

        public void SyncControlTransform()
        {
            if (syncPosition)
                transform.localPosition = netPosition;
            if (syncRotation)
                transform.localRotation = netRotation;
            if (syncScale)
                transform.localScale = netLocalScale;
        }
    }
}
#endif