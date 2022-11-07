#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA || UNITY_WEBGL
using ECS;
using Net.Client;
using Net.Component;
using Net.Share;
using Net.System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace LockStep.Client
{
    public class Game : SingleCase<Game>
    {
        private int frame;
        private readonly List<OperationList> snapshots=new List<OperationList>();
        private int logicFrame, frame1;
        public int frame2;
        public uint delay;
        public GameSystem gameSystem = new GameSystem();
        public GameObject @object;
        public GameObject enemyObj;
        public Net.Vector3 direction;

        // Use this for initialization
        async void Start()
        {
            gameSystem.OnCreate += (opt) => 
            {
                var entity = gameSystem.Create<Entity>();
                Player actor = entity.AddComponent<Player>();
                actor.name = opt.identity.ToString();
                actor.gameObject = Instantiate(@object);
                actor.objectView = actor.gameObject.GetComponent<ObjectView>();
                actor.objectView.actor = actor;
                actor.objectView.anim = actor.gameObject.GetComponent<Animation>();
                actor.rigidBody = actor.gameObject.GetComponent<Rigidbody>();
                if (opt.identity == ClientBase.Instance.UID)
                    FindObjectOfType<ARPGcamera>().target = actor.gameObject.transform;
                return actor;
            };
            gameSystem.OnExitBattle += ()=> {
                frame = 0;
                logicFrame = 0;
                snapshots.Clear();
            };
            while (ClientBase.Instance == null)
            {
                await Task.Yield();
            }
            while (!ClientBase.Instance.Connected)
            {
                await Task.Yield();
            }
            ClientBase.Instance.OnOperationSync += OnOperationSync;
            ClientBase.Instance.AddRpcHandle(gameSystem);

            Physics.autoSimulation = false;
            Physics.autoSyncTransforms = false;

            ThreadManager.Invoke("", 1f, ()=> 
            {
                frame2 = frame - frame1;
                frame1 = frame;
                ClientBase.Instance?.Ping();
                return ClientBase.Instance != null;
            });

            ClientBase.Instance.OnPingCallback += (delay) => {
                this.delay = delay;
            };
        }

        private void OnOperationSync(OperationList list)
        {
            if (frame != list.frame)
            {
                Debug.Log($"帧错误:{frame} - {list.frame}");
                return;
            }
            frame++;
            snapshots.Add(list);
            ClientBase.Instance.AddOperation(new Operation(Command.Input, ClientBase.Instance.UID, direction));
        }

        // Update is called once per frame
        void Update()
        {
            direction = Transform3Dir(Camera.main.transform, Direction);
            int forLogic = frame - logicFrame;
            for (int i = 0; i < forLogic; i++)
            {
                if (logicFrame >= snapshots.Count - 2)
                    return;
                var list = snapshots[logicFrame];
                logicFrame++;
                gameSystem.Run(list);
            }
        }

        public Vector3 Direction
        {
            get { return new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")); }
        }
        public Vector3 Transform3Dir(Transform t, Vector3 dir)
        {
            var f = Mathf.Deg2Rad * (-t.rotation.eulerAngles.y);
            dir.Normalize();
            var ret = new Vector3(dir.x * Mathf.Cos(f) - dir.z * Mathf.Sin(f), 0, dir.x * Mathf.Sin(f) + dir.z * Mathf.Cos(f));
            return ret;
        }

        private void OnDestroy()
        {
            if(ClientBase.Instance != null)
                ClientBase.Instance.OnOperationSync -= OnOperationSync;
        }
    }
}
#endif