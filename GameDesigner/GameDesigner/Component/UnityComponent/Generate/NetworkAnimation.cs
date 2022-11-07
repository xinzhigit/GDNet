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
    /// Animation同步组件, 此代码由BuildComponentTools工具生成, 如果同步发生相互影响的字段或属性, 请自行检查处理一下!
    /// </summary>
    [RequireComponent(typeof(UnityEngine.Animation))]
    public class NetworkAnimation : NetworkBehaviour
    {
        private UnityEngine.Animation self;
        public bool autoCheck;
        private object[] fields;
		private int[] eventsId;
		
        public void Awake()
        {
            self = GetComponent<UnityEngine.Animation>();
			fields = new object[40];
			eventsId = new int[40];
            fields[1] = self.clip;
            fields[2] = self.playAutomatically;
            fields[3] = self.wrapMode;
            fields[4] = self.animatePhysics;
            fields[5] = self.cullingType;
        }

        public UnityEngine.AnimationClip clip
        {
            get
            {
                return self.clip;
            }
            set
            {
                if (value.Equals(fields[1]))
                    return;
                fields[1] = value;
                self.clip = value;
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
        public System.Boolean playAutomatically
        {
            get
            {
                return self.playAutomatically;
            }
            set
            {
                if (value.Equals(fields[2]))
                    return;
                fields[2] = value;
                self.playAutomatically = value;
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
        public UnityEngine.WrapMode wrapMode
        {
            get
            {
                return self.wrapMode;
            }
            set
            {
                if (value.Equals(fields[3]))
                    return;
                fields[3] = value;
                self.wrapMode = value;
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
        public System.Boolean animatePhysics
        {
            get
            {
                return self.animatePhysics;
            }
            set
            {
                if (value.Equals(fields[4]))
                    return;
                fields[4] = value;
                self.animatePhysics = value;
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
        public UnityEngine.AnimationCullingType cullingType
        {
            get
            {
                return self.cullingType;
            }
            set
            {
                if (value.Equals(fields[5]))
                    return;
                fields[5] = value;
                self.cullingType = value;
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

            clip = clip;
            playAutomatically = playAutomatically;
            wrapMode = wrapMode;
            animatePhysics = animatePhysics;
            cullingType = cullingType;
        }

        public void Stop(System.String name, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (name.Equals(fields[7]) &  !always) return;
			fields[7] = name;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { name, } });
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
                    Stop(name, true, 0, 0);
                }, null);
            }
        }
        public void Rewind(System.String name, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (name.Equals(fields[9]) &  !always) return;
			fields[9] = name;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { name, } });
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
                    Rewind(name, true, 0, 0);
                }, null);
            }
        }
        public void Play( bool always = false, int executeNumber = 0, float time = 0)
        {
            if ( !always) return;
			
            var buffer = SerializeModel(new RPCModel() { pars = new object[] {  } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 10,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[10]);
                eventsId[10] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    Play( true, 0, 0);
                }, null);
            }
        }
        public void Play(UnityEngine.PlayMode mode, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (mode.Equals(fields[12]) &  !always) return;
			fields[12] = mode;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { mode, } });
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
                    Play(mode, true, 0, 0);
                }, null);
            }
        }
        public void Play(System.String animation, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (animation.Equals(fields[14]) &  !always) return;
			fields[14] = animation;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { animation, } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 13,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[13]);
                eventsId[13] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    Play(animation, true, 0, 0);
                }, null);
            }
        }
        public void CrossFade(System.String animation, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (animation.Equals(fields[16]) &  !always) return;
			fields[16] = animation;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { animation, } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 15,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[15]);
                eventsId[15] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    CrossFade(animation, true, 0, 0);
                }, null);
            }
        }
        public void CrossFade(System.String animation,System.Single fadeLength, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (animation.Equals(fields[18]) & fadeLength.Equals(fields[19]) &  !always) return;
			fields[18] = animation;
			fields[19] = fadeLength;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { animation,fadeLength, } });
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
                    CrossFade(animation,fadeLength, true, 0, 0);
                }, null);
            }
        }
        public void Blend(System.String animation, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (animation.Equals(fields[21]) &  !always) return;
			fields[21] = animation;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { animation, } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 20,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[20]);
                eventsId[20] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    Blend(animation, true, 0, 0);
                }, null);
            }
        }
        public void Blend(System.String animation,System.Single targetWeight, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (animation.Equals(fields[23]) & targetWeight.Equals(fields[24]) &  !always) return;
			fields[23] = animation;
			fields[24] = targetWeight;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { animation,targetWeight, } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 22,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[22]);
                eventsId[22] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    Blend(animation,targetWeight, true, 0, 0);
                }, null);
            }
        }
        public void CrossFadeQueued(System.String animation, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (animation.Equals(fields[26]) &  !always) return;
			fields[26] = animation;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { animation, } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 25,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[25]);
                eventsId[25] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    CrossFadeQueued(animation, true, 0, 0);
                }, null);
            }
        }
        public void CrossFadeQueued(System.String animation,System.Single fadeLength, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (animation.Equals(fields[28]) & fadeLength.Equals(fields[29]) &  !always) return;
			fields[28] = animation;
			fields[29] = fadeLength;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { animation,fadeLength, } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 27,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[27]);
                eventsId[27] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    CrossFadeQueued(animation,fadeLength, true, 0, 0);
                }, null);
            }
        }
        public void CrossFadeQueued(System.String animation,System.Single fadeLength,UnityEngine.QueueMode queue, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (animation.Equals(fields[31]) & fadeLength.Equals(fields[32]) & queue.Equals(fields[33]) &  !always) return;
			fields[31] = animation;
			fields[32] = fadeLength;
			fields[33] = queue;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { animation,fadeLength,queue, } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 30,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[30]);
                eventsId[30] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    CrossFadeQueued(animation,fadeLength,queue, true, 0, 0);
                }, null);
            }
        }
        public void PlayQueued(System.String animation, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (animation.Equals(fields[35]) &  !always) return;
			fields[35] = animation;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { animation, } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 34,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[34]);
                eventsId[34] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    PlayQueued(animation, true, 0, 0);
                }, null);
            }
        }
        public void PlayQueued(System.String animation,UnityEngine.QueueMode queue, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (animation.Equals(fields[37]) & queue.Equals(fields[38]) &  !always) return;
			fields[37] = animation;
			fields[38] = queue;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { animation,queue, } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 36,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[36]);
                eventsId[36] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    PlayQueued(animation,queue, true, 0, 0);
                }, null);
            }
        }
        public void RemoveClip(System.String clipName, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (clipName.Equals(fields[40]) &  !always) return;
			fields[40] = clipName;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { clipName, } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 39,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[39]);
                eventsId[39] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    RemoveClip(clipName, true, 0, 0);
                }, null);
            }
        }
		public System.Collections.IEnumerator GetEnumerator()
        {
            return self.GetEnumerator();
        }
		public UnityEngine.AnimationClip GetClip(System.String name)
        {
            return self.GetClip(name);
        }
        public override void OnNetworkOperationHandler(Operation opt)
        {
            switch (opt.index2)
            {

                case 1:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var clip = DeserializeObject<UnityEngine.AnimationClip>(new Segment(opt.buffer, false));
						fields[1] = clip;
						self.clip = clip;
					}
                    break;
                case 2:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var playAutomatically = DeserializeObject<System.Boolean>(new Segment(opt.buffer, false));
						fields[2] = playAutomatically;
						self.playAutomatically = playAutomatically;
					}
                    break;
                case 3:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var wrapMode = DeserializeObject<UnityEngine.WrapMode>(new Segment(opt.buffer, false));
						fields[3] = wrapMode;
						self.wrapMode = wrapMode;
					}
                    break;
                case 4:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var animatePhysics = DeserializeObject<System.Boolean>(new Segment(opt.buffer, false));
						fields[4] = animatePhysics;
						self.animatePhysics = animatePhysics;
					}
                    break;
                case 5:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var cullingType = DeserializeObject<UnityEngine.AnimationCullingType>(new Segment(opt.buffer, false));
						fields[5] = cullingType;
						self.cullingType = cullingType;
					}
                    break;
                case 6:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var name = (System.String)(fields[7] = data.Obj);
						self.Stop(name);
					}
                    break;
                case 8:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var name = (System.String)(fields[9] = data.Obj);
						self.Rewind(name);
					}
                    break;
                case 10:
                    {
						self.Play();
					}
                    break;
                case 11:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var mode = (UnityEngine.PlayMode)(fields[12] = data.Obj);
						self.Play(mode);
					}
                    break;
                case 13:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var animation = (System.String)(fields[14] = data.Obj);
						self.Play(animation);
					}
                    break;
                case 15:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var animation = (System.String)(fields[16] = data.Obj);
						self.CrossFade(animation);
					}
                    break;
                case 17:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var animation = (System.String)(fields[18] = data.Obj);
						var fadeLength = (System.Single)(fields[19] = data.Obj);
						self.CrossFade(animation,fadeLength);
					}
                    break;
                case 20:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var animation = (System.String)(fields[21] = data.Obj);
						self.Blend(animation);
					}
                    break;
                case 22:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var animation = (System.String)(fields[23] = data.Obj);
						var targetWeight = (System.Single)(fields[24] = data.Obj);
						self.Blend(animation,targetWeight);
					}
                    break;
                case 25:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var animation = (System.String)(fields[26] = data.Obj);
						self.CrossFadeQueued(animation);
					}
                    break;
                case 27:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var animation = (System.String)(fields[28] = data.Obj);
						var fadeLength = (System.Single)(fields[29] = data.Obj);
						self.CrossFadeQueued(animation,fadeLength);
					}
                    break;
                case 30:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var animation = (System.String)(fields[31] = data.Obj);
						var fadeLength = (System.Single)(fields[32] = data.Obj);
						var queue = (UnityEngine.QueueMode)(fields[33] = data.Obj);
						self.CrossFadeQueued(animation,fadeLength,queue);
					}
                    break;
                case 34:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var animation = (System.String)(fields[35] = data.Obj);
						self.PlayQueued(animation);
					}
                    break;
                case 36:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var animation = (System.String)(fields[37] = data.Obj);
						var queue = (UnityEngine.QueueMode)(fields[38] = data.Obj);
						self.PlayQueued(animation,queue);
					}
                    break;
                case 39:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var clipName = (System.String)(fields[40] = data.Obj);
						self.RemoveClip(clipName);
					}
                    break;
            }
        }
    }
}
#endif