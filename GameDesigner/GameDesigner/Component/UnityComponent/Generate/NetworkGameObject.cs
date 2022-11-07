#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA || UNITY_WEBGL
using Net.Client;
using Net.Share;
using Net.Component;
using Net.UnityComponent;
using UnityEngine;
using Net.System;
using static Net.Serialize.NetConvertFast2;

namespace BuildComponent
{
    /// <summary>
    /// GameObject同步组件, 此代码由BuildComponentTools工具生成, 如果同步发生相互影响的字段或属性, 请自行检查处理一下!
    /// </summary>
    [RequireComponent(typeof(UnityEngine.GameObject))]
    public class NetworkGameObject : NetworkBehaviour
    {
        private UnityEngine.GameObject self;
        public bool autoCheck;
        private object[] fields;
		private int[] eventsId;
		
        public void Awake()
        {
            self = GetComponent<UnityEngine.GameObject>();
			fields = new object[22];
			eventsId = new int[22];
            fields[1] = self.layer;
            fields[2] = self.isStatic;
            fields[3] = self.tag;
            fields[4] = self.name;
            fields[5] = self.hideFlags;
        }

        public System.Int32 layer
        {
            get
            {
                return self.layer;
            }
            set
            {
                if (value.Equals(fields[1]))
                    return;
                fields[1] = value;
                self.layer = value;
                ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = Index,
                    index2 = 1,
                    buffer = SerializeObject(value).ToArray(true),
                    uid = ClientBase.Instance.UID
                });
            }
        }
        public System.Boolean isStatic
        {
            get
            {
                return self.isStatic;
            }
            set
            {
                if (value.Equals(fields[2]))
                    return;
                fields[2] = value;
                self.isStatic = value;
                ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = Index,
                    index2 = 2,
                    buffer = SerializeObject(value).ToArray(true),
                    uid = ClientBase.Instance.UID
                });
            }
        }
        public System.String tag
        {
            get
            {
                return self.tag;
            }
            set
            {
                if (value.Equals(fields[3]))
                    return;
                fields[3] = value;
                self.tag = value;
                ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = Index,
                    index2 = 3,
                    buffer = SerializeObject(value).ToArray(true),
                    uid = ClientBase.Instance.UID
                });
            }
        }
        public System.String name
        {
            get
            {
                return self.name;
            }
            set
            {
                if (value.Equals(fields[4]))
                    return;
                fields[4] = value;
                self.name = value;
                ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = Index,
                    index2 = 4,
                    buffer = SerializeObject(value).ToArray(true),
                    uid = ClientBase.Instance.UID
                });
            }
        }
        public UnityEngine.HideFlags hideFlags
        {
            get
            {
                return self.hideFlags;
            }
            set
            {
                if (value.Equals(fields[5]))
                    return;
                fields[5] = value;
                self.hideFlags = value;
                ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = Index,
                    index2 = 5,
                    buffer = SerializeObject(value).ToArray(true),
                    uid = ClientBase.Instance.UID
                });
            }
        }
        public override void OnPropertyAutoCheck()
        {
            if (!autoCheck)
                return;

            layer = layer;
            isStatic = isStatic;
            tag = tag;
            name = name;
            hideFlags = hideFlags;
        }

        public void GetComponent(System.String type, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (type.Equals(fields[7]) &  !always) return;
			fields[7] = type;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { type, } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 6,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[6]);
                eventsId[6] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    GetComponent(type, true, 0, 0);
                }, null);
            }
        }
        public void SendMessageUpwards(System.String methodName,UnityEngine.SendMessageOptions options, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (methodName.Equals(fields[9]) & options.Equals(fields[10]) &  !always) return;
			fields[9] = methodName;
			fields[10] = options;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { methodName,options, } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 8,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[8]);
                eventsId[8] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    SendMessageUpwards(methodName,options, true, 0, 0);
                }, null);
            }
        }
        public void SendMessage(System.String methodName,UnityEngine.SendMessageOptions options, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (methodName.Equals(fields[12]) & options.Equals(fields[13]) &  !always) return;
			fields[12] = methodName;
			fields[13] = options;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { methodName,options, } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 11,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[11]);
                eventsId[11] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    SendMessage(methodName,options, true, 0, 0);
                }, null);
            }
        }
        public void BroadcastMessage(System.String methodName,UnityEngine.SendMessageOptions options, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (methodName.Equals(fields[15]) & options.Equals(fields[16]) &  !always) return;
			fields[15] = methodName;
			fields[16] = options;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { methodName,options, } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 14,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[14]);
                eventsId[14] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    BroadcastMessage(methodName,options, true, 0, 0);
                }, null);
            }
        }
        public void SendMessageUpwards(System.String methodName, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (methodName.Equals(fields[18]) &  !always) return;
			fields[18] = methodName;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { methodName, } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 17,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[17]);
                eventsId[17] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    SendMessageUpwards(methodName, true, 0, 0);
                }, null);
            }
        }
        public void SendMessage(System.String methodName, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (methodName.Equals(fields[20]) &  !always) return;
			fields[20] = methodName;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { methodName, } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 19,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[19]);
                eventsId[19] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    SendMessage(methodName, true, 0, 0);
                }, null);
            }
        }
        public void BroadcastMessage(System.String methodName, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (methodName.Equals(fields[22]) &  !always) return;
			fields[22] = methodName;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { methodName, } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 21,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[21]);
                eventsId[21] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    BroadcastMessage(methodName, true, 0, 0);
                }, null);
            }
        }
        public override void OnNetworkOperationHandler(Operation opt)
        {
            switch (opt.index2)
            {

                case 1:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var layer = DeserializeObject<System.Int32>(new Segment(opt.buffer, false));
						fields[1] = layer;
						self.layer = layer;
					}
                    break;
                case 2:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var isStatic = DeserializeObject<System.Boolean>(new Segment(opt.buffer, false));
						fields[2] = isStatic;
						self.isStatic = isStatic;
					}
                    break;
                case 3:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var tag = DeserializeObject<System.String>(new Segment(opt.buffer, false));
						fields[3] = tag;
						self.tag = tag;
					}
                    break;
                case 4:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var name = DeserializeObject<System.String>(new Segment(opt.buffer, false));
						fields[4] = name;
						self.name = name;
					}
                    break;
                case 5:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var hideFlags = DeserializeObject<UnityEngine.HideFlags>(new Segment(opt.buffer, false));
						fields[5] = hideFlags;
						self.hideFlags = hideFlags;
					}
                    break;
                case 6:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var type = (System.String)(fields[7] = data.Obj);
						self.GetComponent(type);
					}
                    break;
                case 8:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var methodName = (System.String)(fields[9] = data.Obj);
						var options = (UnityEngine.SendMessageOptions)(fields[10] = data.Obj);
						self.SendMessageUpwards(methodName,options);
					}
                    break;
                case 11:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var methodName = (System.String)(fields[12] = data.Obj);
						var options = (UnityEngine.SendMessageOptions)(fields[13] = data.Obj);
						self.SendMessage(methodName,options);
					}
                    break;
                case 14:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var methodName = (System.String)(fields[15] = data.Obj);
						var options = (UnityEngine.SendMessageOptions)(fields[16] = data.Obj);
						self.BroadcastMessage(methodName,options);
					}
                    break;
                case 17:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var methodName = (System.String)(fields[18] = data.Obj);
						self.SendMessageUpwards(methodName);
					}
                    break;
                case 19:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var methodName = (System.String)(fields[20] = data.Obj);
						self.SendMessage(methodName);
					}
                    break;
                case 21:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var methodName = (System.String)(fields[22] = data.Obj);
						self.BroadcastMessage(methodName);
					}
                    break;
            }
        }
    }
}
#endif