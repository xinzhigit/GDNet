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
    /// Animator同步组件, 此代码由BuildComponentTools工具生成, 如果同步发生相互影响的字段或属性, 请自行检查处理一下!
    /// </summary>
    [RequireComponent(typeof(UnityEngine.Animator))]
    public class NetworkAnimator : NetworkBehaviour
    {
        private UnityEngine.Animator self;
        public bool autoCheck;
        private object[] fields;
		private int[] eventsId;
		
        public void Awake()
        {
            self = GetComponent<UnityEngine.Animator>();
			fields = new object[188];
			eventsId = new int[188];
            fields[1] = self.rootPosition;
            fields[2] = self.rootRotation;
            fields[3] = self.applyRootMotion;
            fields[4] = self.updateMode;
            fields[5] = self.bodyPosition;
            fields[6] = self.bodyRotation;
            fields[7] = self.stabilizeFeet;
            fields[8] = self.feetPivotActive;
            fields[9] = self.speed;
            fields[10] = self.cullingMode;
            fields[11] = self.playbackTime;
            fields[12] = self.recorderStartTime;
            fields[13] = self.recorderStopTime;
            fields[14] = self.runtimeAnimatorController;
            fields[15] = self.avatar;
            fields[16] = self.layersAffectMassCenter;
            fields[17] = self.logWarnings;
            fields[18] = self.fireEvents;
            fields[19] = self.keepAnimatorControllerStateOnDisable;
        }

        public UnityEngine.Vector3 rootPosition
        {
            get
            {
                return self.rootPosition;
            }
            set
            {
                if (value.Equals(fields[1]))
                    return;
                fields[1] = value;
                self.rootPosition = value;
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
        public UnityEngine.Quaternion rootRotation
        {
            get
            {
                return self.rootRotation;
            }
            set
            {
                if (value.Equals(fields[2]))
                    return;
                fields[2] = value;
                self.rootRotation = value;
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
        public System.Boolean applyRootMotion
        {
            get
            {
                return self.applyRootMotion;
            }
            set
            {
                if (value.Equals(fields[3]))
                    return;
                fields[3] = value;
                self.applyRootMotion = value;
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
        public UnityEngine.AnimatorUpdateMode updateMode
        {
            get
            {
                return self.updateMode;
            }
            set
            {
                if (value.Equals(fields[4]))
                    return;
                fields[4] = value;
                self.updateMode = value;
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
        public UnityEngine.Vector3 bodyPosition
        {
            get
            {
                return self.bodyPosition;
            }
            set
            {
                if (value.Equals(fields[5]))
                    return;
                fields[5] = value;
                self.bodyPosition = value;
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
        public UnityEngine.Quaternion bodyRotation
        {
            get
            {
                return self.bodyRotation;
            }
            set
            {
                if (value.Equals(fields[6]))
                    return;
                fields[6] = value;
                self.bodyRotation = value;
                ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = Index,
                    index2 = 6,
                    buffer = SerializeObject(value).ToArray(true),
                    uid = ClientBase.Instance.UID
                });
            }
        }
        public System.Boolean stabilizeFeet
        {
            get
            {
                return self.stabilizeFeet;
            }
            set
            {
                if (value.Equals(fields[7]))
                    return;
                fields[7] = value;
                self.stabilizeFeet = value;
                ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = Index,
                    index2 = 7,
                    buffer = SerializeObject(value).ToArray(true),
                    uid = ClientBase.Instance.UID
                });
            }
        }
        public System.Single feetPivotActive
        {
            get
            {
                return self.feetPivotActive;
            }
            set
            {
                if (value.Equals(fields[8]))
                    return;
                fields[8] = value;
                self.feetPivotActive = value;
                ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = Index,
                    index2 = 8,
                    buffer = SerializeObject(value).ToArray(true),
                    uid = ClientBase.Instance.UID
                });
            }
        }
        public System.Single speed
        {
            get
            {
                return self.speed;
            }
            set
            {
                if (value.Equals(fields[9]))
                    return;
                fields[9] = value;
                self.speed = value;
                ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = Index,
                    index2 = 9,
                    buffer = SerializeObject(value).ToArray(true),
                    uid = ClientBase.Instance.UID
                });
            }
        }
        public UnityEngine.AnimatorCullingMode cullingMode
        {
            get
            {
                return self.cullingMode;
            }
            set
            {
                if (value.Equals(fields[10]))
                    return;
                fields[10] = value;
                self.cullingMode = value;
                ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = Index,
                    index2 = 10,
                    buffer = SerializeObject(value).ToArray(true),
                    uid = ClientBase.Instance.UID
                });
            }
        }
        public System.Single playbackTime
        {
            get
            {
                return self.playbackTime;
            }
            set
            {
                if (value.Equals(fields[11]))
                    return;
                fields[11] = value;
                self.playbackTime = value;
                ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = Index,
                    index2 = 11,
                    buffer = SerializeObject(value).ToArray(true),
                    uid = ClientBase.Instance.UID
                });
            }
        }
        public System.Single recorderStartTime
        {
            get
            {
                return self.recorderStartTime;
            }
            set
            {
                if (value.Equals(fields[12]))
                    return;
                fields[12] = value;
                self.recorderStartTime = value;
                ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = Index,
                    index2 = 12,
                    buffer = SerializeObject(value).ToArray(true),
                    uid = ClientBase.Instance.UID
                });
            }
        }
        public System.Single recorderStopTime
        {
            get
            {
                return self.recorderStopTime;
            }
            set
            {
                if (value.Equals(fields[13]))
                    return;
                fields[13] = value;
                self.recorderStopTime = value;
                ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = Index,
                    index2 = 13,
                    buffer = SerializeObject(value).ToArray(true),
                    uid = ClientBase.Instance.UID
                });
            }
        }
        public UnityEngine.RuntimeAnimatorController runtimeAnimatorController
        {
            get
            {
                return self.runtimeAnimatorController;
            }
            set
            {
                if (value.Equals(fields[14]))
                    return;
                fields[14] = value;
                self.runtimeAnimatorController = value;
                ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = Index,
                    index2 = 14,
                    buffer = SerializeObject(value).ToArray(true),
                    uid = ClientBase.Instance.UID
                });
            }
        }
        public UnityEngine.Avatar avatar
        {
            get
            {
                return self.avatar;
            }
            set
            {
                if (value.Equals(fields[15]))
                    return;
                fields[15] = value;
                self.avatar = value;
                ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = Index,
                    index2 = 15,
                    buffer = SerializeObject(value).ToArray(true),
                    uid = ClientBase.Instance.UID
                });
            }
        }
        public System.Boolean layersAffectMassCenter
        {
            get
            {
                return self.layersAffectMassCenter;
            }
            set
            {
                if (value.Equals(fields[16]))
                    return;
                fields[16] = value;
                self.layersAffectMassCenter = value;
                ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = Index,
                    index2 = 16,
                    buffer = SerializeObject(value).ToArray(true),
                    uid = ClientBase.Instance.UID
                });
            }
        }
        public System.Boolean logWarnings
        {
            get
            {
                return self.logWarnings;
            }
            set
            {
                if (value.Equals(fields[17]))
                    return;
                fields[17] = value;
                self.logWarnings = value;
                ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = Index,
                    index2 = 17,
                    buffer = SerializeObject(value).ToArray(true),
                    uid = ClientBase.Instance.UID
                });
            }
        }
        public System.Boolean fireEvents
        {
            get
            {
                return self.fireEvents;
            }
            set
            {
                if (value.Equals(fields[18]))
                    return;
                fields[18] = value;
                self.fireEvents = value;
                ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = Index,
                    index2 = 18,
                    buffer = SerializeObject(value).ToArray(true),
                    uid = ClientBase.Instance.UID
                });
            }
        }
        public System.Boolean keepAnimatorControllerStateOnDisable
        {
            get
            {
                return self.keepAnimatorControllerStateOnDisable;
            }
            set
            {
                if (value.Equals(fields[19]))
                    return;
                fields[19] = value;
                self.keepAnimatorControllerStateOnDisable = value;
                ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = Index,
                    index2 = 19,
                    buffer = SerializeObject(value).ToArray(true),
                    uid = ClientBase.Instance.UID
                });
            }
        }
        public override void OnPropertyAutoCheck()
        {
            if (!autoCheck)
                return;

            rootPosition = rootPosition;
            rootRotation = rootRotation;
            applyRootMotion = applyRootMotion;
            updateMode = updateMode;
            bodyPosition = bodyPosition;
            bodyRotation = bodyRotation;
            stabilizeFeet = stabilizeFeet;
            feetPivotActive = feetPivotActive;
            speed = speed;
            cullingMode = cullingMode;
            playbackTime = playbackTime;
            recorderStartTime = recorderStartTime;
            recorderStopTime = recorderStopTime;
            runtimeAnimatorController = runtimeAnimatorController;
            avatar = avatar;
            layersAffectMassCenter = layersAffectMassCenter;
            logWarnings = logWarnings;
            fireEvents = fireEvents;
            keepAnimatorControllerStateOnDisable = keepAnimatorControllerStateOnDisable;
        }

		public System.Single GetFloat(System.String name)
        {
            return self.GetFloat(name);
        }
		public System.Single GetFloat(System.Int32 id)
        {
            return self.GetFloat(id);
        }
        public void SetFloat(System.String name,System.Single value, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (name.Equals(fields[21]) & value.Equals(fields[22]) &  !always) return;
			fields[21] = name;
			fields[22] = value;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { name,value, } });
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
                    SetFloat(name,value, true, 0, 0);
                }, null);
            }
        }
        public void SetFloat(System.String name,System.Single value,System.Single dampTime,System.Single deltaTime, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (name.Equals(fields[24]) & value.Equals(fields[25]) & dampTime.Equals(fields[26]) & deltaTime.Equals(fields[27]) &  !always) return;
			fields[24] = name;
			fields[25] = value;
			fields[26] = dampTime;
			fields[27] = deltaTime;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { name,value,dampTime,deltaTime, } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 23,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[23]);
                eventsId[23] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    SetFloat(name,value,dampTime,deltaTime, true, 0, 0);
                }, null);
            }
        }
        public void SetFloat(System.Int32 id,System.Single value, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (id.Equals(fields[29]) & value.Equals(fields[30]) &  !always) return;
			fields[29] = id;
			fields[30] = value;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { id,value, } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 28,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[28]);
                eventsId[28] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    SetFloat(id,value, true, 0, 0);
                }, null);
            }
        }
        public void SetFloat(System.Int32 id,System.Single value,System.Single dampTime,System.Single deltaTime, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (id.Equals(fields[32]) & value.Equals(fields[33]) & dampTime.Equals(fields[34]) & deltaTime.Equals(fields[35]) &  !always) return;
			fields[32] = id;
			fields[33] = value;
			fields[34] = dampTime;
			fields[35] = deltaTime;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { id,value,dampTime,deltaTime, } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 31,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[31]);
                eventsId[31] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    SetFloat(id,value,dampTime,deltaTime, true, 0, 0);
                }, null);
            }
        }
		public System.Boolean GetBool(System.String name)
        {
            return self.GetBool(name);
        }
		public System.Boolean GetBool(System.Int32 id)
        {
            return self.GetBool(id);
        }
        public void SetBool(System.String name,System.Boolean value, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (name.Equals(fields[37]) & value.Equals(fields[38]) &  !always) return;
			fields[37] = name;
			fields[38] = value;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { name,value, } });
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
                    SetBool(name,value, true, 0, 0);
                }, null);
            }
        }
        public void SetBool(System.Int32 id,System.Boolean value, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (id.Equals(fields[40]) & value.Equals(fields[41]) &  !always) return;
			fields[40] = id;
			fields[41] = value;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { id,value, } });
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
                    SetBool(id,value, true, 0, 0);
                }, null);
            }
        }
		public System.Int32 GetInteger(System.String name)
        {
            return self.GetInteger(name);
        }
		public System.Int32 GetInteger(System.Int32 id)
        {
            return self.GetInteger(id);
        }
        public void SetInteger(System.String name,System.Int32 value, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (name.Equals(fields[43]) & value.Equals(fields[44]) &  !always) return;
			fields[43] = name;
			fields[44] = value;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { name,value, } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 42,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[42]);
                eventsId[42] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    SetInteger(name,value, true, 0, 0);
                }, null);
            }
        }
        public void SetInteger(System.Int32 id,System.Int32 value, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (id.Equals(fields[46]) & value.Equals(fields[47]) &  !always) return;
			fields[46] = id;
			fields[47] = value;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { id,value, } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 45,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[45]);
                eventsId[45] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    SetInteger(id,value, true, 0, 0);
                }, null);
            }
        }
        public void SetTrigger(System.String name, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (name.Equals(fields[49]) &  !always) return;
			fields[49] = name;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { name, } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 48,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[48]);
                eventsId[48] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    SetTrigger(name, true, 0, 0);
                }, null);
            }
        }
        public void SetTrigger(System.Int32 id, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (id.Equals(fields[51]) &  !always) return;
			fields[51] = id;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { id, } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 50,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[50]);
                eventsId[50] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    SetTrigger(id, true, 0, 0);
                }, null);
            }
        }
        public void ResetTrigger(System.String name, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (name.Equals(fields[53]) &  !always) return;
			fields[53] = name;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { name, } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 52,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[52]);
                eventsId[52] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    ResetTrigger(name, true, 0, 0);
                }, null);
            }
        }
        public void ResetTrigger(System.Int32 id, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (id.Equals(fields[55]) &  !always) return;
			fields[55] = id;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { id, } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 54,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[54]);
                eventsId[54] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    ResetTrigger(id, true, 0, 0);
                }, null);
            }
        }
		public System.Boolean IsParameterControlledByCurve(System.String name)
        {
            return self.IsParameterControlledByCurve(name);
        }
		public System.Boolean IsParameterControlledByCurve(System.Int32 id)
        {
            return self.IsParameterControlledByCurve(id);
        }
		public UnityEngine.Vector3 GetIKPosition(UnityEngine.AvatarIKGoal goal)
        {
            return self.GetIKPosition(goal);
        }
        public void SetIKPosition(UnityEngine.AvatarIKGoal goal,UnityEngine.Vector3 goalPosition, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (goal.Equals(fields[57]) & goalPosition.Equals(fields[58]) &  !always) return;
			fields[57] = goal;
			fields[58] = goalPosition;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { goal,goalPosition, } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 56,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[56]);
                eventsId[56] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    SetIKPosition(goal,goalPosition, true, 0, 0);
                }, null);
            }
        }
		public UnityEngine.Quaternion GetIKRotation(UnityEngine.AvatarIKGoal goal)
        {
            return self.GetIKRotation(goal);
        }
        public void SetIKRotation(UnityEngine.AvatarIKGoal goal,UnityEngine.Quaternion goalRotation, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (goal.Equals(fields[60]) & goalRotation.Equals(fields[61]) &  !always) return;
			fields[60] = goal;
			fields[61] = goalRotation;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { goal,goalRotation, } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 59,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[59]);
                eventsId[59] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    SetIKRotation(goal,goalRotation, true, 0, 0);
                }, null);
            }
        }
		public System.Single GetIKPositionWeight(UnityEngine.AvatarIKGoal goal)
        {
            return self.GetIKPositionWeight(goal);
        }
        public void SetIKPositionWeight(UnityEngine.AvatarIKGoal goal,System.Single value, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (goal.Equals(fields[63]) & value.Equals(fields[64]) &  !always) return;
			fields[63] = goal;
			fields[64] = value;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { goal,value, } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 62,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[62]);
                eventsId[62] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    SetIKPositionWeight(goal,value, true, 0, 0);
                }, null);
            }
        }
		public System.Single GetIKRotationWeight(UnityEngine.AvatarIKGoal goal)
        {
            return self.GetIKRotationWeight(goal);
        }
        public void SetIKRotationWeight(UnityEngine.AvatarIKGoal goal,System.Single value, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (goal.Equals(fields[66]) & value.Equals(fields[67]) &  !always) return;
			fields[66] = goal;
			fields[67] = value;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { goal,value, } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 65,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[65]);
                eventsId[65] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    SetIKRotationWeight(goal,value, true, 0, 0);
                }, null);
            }
        }
		public UnityEngine.Vector3 GetIKHintPosition(UnityEngine.AvatarIKHint hint)
        {
            return self.GetIKHintPosition(hint);
        }
        public void SetIKHintPosition(UnityEngine.AvatarIKHint hint,UnityEngine.Vector3 hintPosition, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (hint.Equals(fields[69]) & hintPosition.Equals(fields[70]) &  !always) return;
			fields[69] = hint;
			fields[70] = hintPosition;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { hint,hintPosition, } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 68,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[68]);
                eventsId[68] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    SetIKHintPosition(hint,hintPosition, true, 0, 0);
                }, null);
            }
        }
		public System.Single GetIKHintPositionWeight(UnityEngine.AvatarIKHint hint)
        {
            return self.GetIKHintPositionWeight(hint);
        }
        public void SetIKHintPositionWeight(UnityEngine.AvatarIKHint hint,System.Single value, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (hint.Equals(fields[72]) & value.Equals(fields[73]) &  !always) return;
			fields[72] = hint;
			fields[73] = value;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { hint,value, } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 71,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[71]);
                eventsId[71] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    SetIKHintPositionWeight(hint,value, true, 0, 0);
                }, null);
            }
        }
        public void SetLookAtPosition(UnityEngine.Vector3 lookAtPosition, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (lookAtPosition.Equals(fields[75]) &  !always) return;
			fields[75] = lookAtPosition;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { lookAtPosition, } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 74,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[74]);
                eventsId[74] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    SetLookAtPosition(lookAtPosition, true, 0, 0);
                }, null);
            }
        }
        public void SetLookAtWeight(System.Single weight, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (weight.Equals(fields[77]) &  !always) return;
			fields[77] = weight;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { weight, } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 76,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[76]);
                eventsId[76] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    SetLookAtWeight(weight, true, 0, 0);
                }, null);
            }
        }
        public void SetLookAtWeight(System.Single weight,System.Single bodyWeight, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (weight.Equals(fields[79]) & bodyWeight.Equals(fields[80]) &  !always) return;
			fields[79] = weight;
			fields[80] = bodyWeight;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { weight,bodyWeight, } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 78,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[78]);
                eventsId[78] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    SetLookAtWeight(weight,bodyWeight, true, 0, 0);
                }, null);
            }
        }
        public void SetLookAtWeight(System.Single weight,System.Single bodyWeight,System.Single headWeight, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (weight.Equals(fields[82]) & bodyWeight.Equals(fields[83]) & headWeight.Equals(fields[84]) &  !always) return;
			fields[82] = weight;
			fields[83] = bodyWeight;
			fields[84] = headWeight;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { weight,bodyWeight,headWeight, } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 81,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[81]);
                eventsId[81] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    SetLookAtWeight(weight,bodyWeight,headWeight, true, 0, 0);
                }, null);
            }
        }
        public void SetLookAtWeight(System.Single weight,System.Single bodyWeight,System.Single headWeight,System.Single eyesWeight, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (weight.Equals(fields[86]) & bodyWeight.Equals(fields[87]) & headWeight.Equals(fields[88]) & eyesWeight.Equals(fields[89]) &  !always) return;
			fields[86] = weight;
			fields[87] = bodyWeight;
			fields[88] = headWeight;
			fields[89] = eyesWeight;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { weight,bodyWeight,headWeight,eyesWeight, } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 85,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[85]);
                eventsId[85] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    SetLookAtWeight(weight,bodyWeight,headWeight,eyesWeight, true, 0, 0);
                }, null);
            }
        }
        public void SetLookAtWeight(System.Single weight,System.Single bodyWeight,System.Single headWeight,System.Single eyesWeight,System.Single clampWeight, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (weight.Equals(fields[91]) & bodyWeight.Equals(fields[92]) & headWeight.Equals(fields[93]) & eyesWeight.Equals(fields[94]) & clampWeight.Equals(fields[95]) &  !always) return;
			fields[91] = weight;
			fields[92] = bodyWeight;
			fields[93] = headWeight;
			fields[94] = eyesWeight;
			fields[95] = clampWeight;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { weight,bodyWeight,headWeight,eyesWeight,clampWeight, } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 90,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[90]);
                eventsId[90] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    SetLookAtWeight(weight,bodyWeight,headWeight,eyesWeight,clampWeight, true, 0, 0);
                }, null);
            }
        }
        public void SetBoneLocalRotation(UnityEngine.HumanBodyBones humanBoneId,UnityEngine.Quaternion rotation, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (humanBoneId.Equals(fields[97]) & rotation.Equals(fields[98]) &  !always) return;
			fields[97] = humanBoneId;
			fields[98] = rotation;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { humanBoneId,rotation, } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 96,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[96]);
                eventsId[96] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    SetBoneLocalRotation(humanBoneId,rotation, true, 0, 0);
                }, null);
            }
        }
		public UnityEngine.StateMachineBehaviour[] GetBehaviours(System.Int32 fullPathHash,System.Int32 layerIndex)
        {
            return self.GetBehaviours(fullPathHash,layerIndex);
        }
		public UnityEngine.AnimatorStateInfo GetCurrentAnimatorStateInfo(System.Int32 layerIndex)
        {
            return self.GetCurrentAnimatorStateInfo(layerIndex);
        }
		public UnityEngine.AnimatorStateInfo GetNextAnimatorStateInfo(System.Int32 layerIndex)
        {
            return self.GetNextAnimatorStateInfo(layerIndex);
        }
		public UnityEngine.AnimatorTransitionInfo GetAnimatorTransitionInfo(System.Int32 layerIndex)
        {
            return self.GetAnimatorTransitionInfo(layerIndex);
        }
		public System.Int32 GetCurrentAnimatorClipInfoCount(System.Int32 layerIndex)
        {
            return self.GetCurrentAnimatorClipInfoCount(layerIndex);
        }
		public System.Int32 GetNextAnimatorClipInfoCount(System.Int32 layerIndex)
        {
            return self.GetNextAnimatorClipInfoCount(layerIndex);
        }
		public UnityEngine.AnimatorControllerParameter GetParameter(System.Int32 index)
        {
            return self.GetParameter(index);
        }
        public void InterruptMatchTarget( bool always = false, int executeNumber = 0, float time = 0)
        {
            if ( !always) return;
			
            var buffer = SerializeModel(new RPCModel() { pars = new object[] {  } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 99,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[99]);
                eventsId[99] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    InterruptMatchTarget( true, 0, 0);
                }, null);
            }
        }
        public void CrossFadeInFixedTime(System.String stateName,System.Single fixedTransitionDuration, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (stateName.Equals(fields[101]) & fixedTransitionDuration.Equals(fields[102]) &  !always) return;
			fields[101] = stateName;
			fields[102] = fixedTransitionDuration;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { stateName,fixedTransitionDuration, } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 100,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[100]);
                eventsId[100] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    CrossFadeInFixedTime(stateName,fixedTransitionDuration, true, 0, 0);
                }, null);
            }
        }
        public void CrossFadeInFixedTime(System.String stateName,System.Single fixedTransitionDuration,System.Int32 layer, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (stateName.Equals(fields[104]) & fixedTransitionDuration.Equals(fields[105]) & layer.Equals(fields[106]) &  !always) return;
			fields[104] = stateName;
			fields[105] = fixedTransitionDuration;
			fields[106] = layer;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { stateName,fixedTransitionDuration,layer, } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 103,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[103]);
                eventsId[103] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    CrossFadeInFixedTime(stateName,fixedTransitionDuration,layer, true, 0, 0);
                }, null);
            }
        }
        public void CrossFadeInFixedTime(System.String stateName,System.Single fixedTransitionDuration,System.Int32 layer,System.Single fixedTimeOffset, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (stateName.Equals(fields[108]) & fixedTransitionDuration.Equals(fields[109]) & layer.Equals(fields[110]) & fixedTimeOffset.Equals(fields[111]) &  !always) return;
			fields[108] = stateName;
			fields[109] = fixedTransitionDuration;
			fields[110] = layer;
			fields[111] = fixedTimeOffset;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { stateName,fixedTransitionDuration,layer,fixedTimeOffset, } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 107,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[107]);
                eventsId[107] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    CrossFadeInFixedTime(stateName,fixedTransitionDuration,layer,fixedTimeOffset, true, 0, 0);
                }, null);
            }
        }
        public void CrossFadeInFixedTime(System.String stateName,System.Single fixedTransitionDuration,System.Int32 layer,System.Single fixedTimeOffset,System.Single normalizedTransitionTime, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (stateName.Equals(fields[113]) & fixedTransitionDuration.Equals(fields[114]) & layer.Equals(fields[115]) & fixedTimeOffset.Equals(fields[116]) & normalizedTransitionTime.Equals(fields[117]) &  !always) return;
			fields[113] = stateName;
			fields[114] = fixedTransitionDuration;
			fields[115] = layer;
			fields[116] = fixedTimeOffset;
			fields[117] = normalizedTransitionTime;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { stateName,fixedTransitionDuration,layer,fixedTimeOffset,normalizedTransitionTime, } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 112,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[112]);
                eventsId[112] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    CrossFadeInFixedTime(stateName,fixedTransitionDuration,layer,fixedTimeOffset,normalizedTransitionTime, true, 0, 0);
                }, null);
            }
        }
        public void CrossFadeInFixedTime(System.Int32 stateHashName,System.Single fixedTransitionDuration,System.Int32 layer,System.Single fixedTimeOffset, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (stateHashName.Equals(fields[119]) & fixedTransitionDuration.Equals(fields[120]) & layer.Equals(fields[121]) & fixedTimeOffset.Equals(fields[122]) &  !always) return;
			fields[119] = stateHashName;
			fields[120] = fixedTransitionDuration;
			fields[121] = layer;
			fields[122] = fixedTimeOffset;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { stateHashName,fixedTransitionDuration,layer,fixedTimeOffset, } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 118,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[118]);
                eventsId[118] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    CrossFadeInFixedTime(stateHashName,fixedTransitionDuration,layer,fixedTimeOffset, true, 0, 0);
                }, null);
            }
        }
        public void CrossFadeInFixedTime(System.Int32 stateHashName,System.Single fixedTransitionDuration,System.Int32 layer, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (stateHashName.Equals(fields[124]) & fixedTransitionDuration.Equals(fields[125]) & layer.Equals(fields[126]) &  !always) return;
			fields[124] = stateHashName;
			fields[125] = fixedTransitionDuration;
			fields[126] = layer;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { stateHashName,fixedTransitionDuration,layer, } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 123,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[123]);
                eventsId[123] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    CrossFadeInFixedTime(stateHashName,fixedTransitionDuration,layer, true, 0, 0);
                }, null);
            }
        }
        public void CrossFadeInFixedTime(System.Int32 stateHashName,System.Single fixedTransitionDuration, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (stateHashName.Equals(fields[128]) & fixedTransitionDuration.Equals(fields[129]) &  !always) return;
			fields[128] = stateHashName;
			fields[129] = fixedTransitionDuration;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { stateHashName,fixedTransitionDuration, } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 127,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[127]);
                eventsId[127] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    CrossFadeInFixedTime(stateHashName,fixedTransitionDuration, true, 0, 0);
                }, null);
            }
        }
        public void CrossFade(System.String stateName,System.Single normalizedTransitionDuration,System.Int32 layer,System.Single normalizedTimeOffset, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (stateName.Equals(fields[131]) & normalizedTransitionDuration.Equals(fields[132]) & layer.Equals(fields[133]) & normalizedTimeOffset.Equals(fields[134]) &  !always) return;
			fields[131] = stateName;
			fields[132] = normalizedTransitionDuration;
			fields[133] = layer;
			fields[134] = normalizedTimeOffset;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { stateName,normalizedTransitionDuration,layer,normalizedTimeOffset, } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 130,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[130]);
                eventsId[130] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    CrossFade(stateName,normalizedTransitionDuration,layer,normalizedTimeOffset, true, 0, 0);
                }, null);
            }
        }
        public void CrossFade(System.String stateName,System.Single normalizedTransitionDuration,System.Int32 layer, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (stateName.Equals(fields[136]) & normalizedTransitionDuration.Equals(fields[137]) & layer.Equals(fields[138]) &  !always) return;
			fields[136] = stateName;
			fields[137] = normalizedTransitionDuration;
			fields[138] = layer;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { stateName,normalizedTransitionDuration,layer, } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 135,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[135]);
                eventsId[135] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    CrossFade(stateName,normalizedTransitionDuration,layer, true, 0, 0);
                }, null);
            }
        }
        public void CrossFade(System.String stateName,System.Single normalizedTransitionDuration, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (stateName.Equals(fields[140]) & normalizedTransitionDuration.Equals(fields[141]) &  !always) return;
			fields[140] = stateName;
			fields[141] = normalizedTransitionDuration;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { stateName,normalizedTransitionDuration, } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 139,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[139]);
                eventsId[139] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    CrossFade(stateName,normalizedTransitionDuration, true, 0, 0);
                }, null);
            }
        }
        public void CrossFade(System.String stateName,System.Single normalizedTransitionDuration,System.Int32 layer,System.Single normalizedTimeOffset,System.Single normalizedTransitionTime, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (stateName.Equals(fields[143]) & normalizedTransitionDuration.Equals(fields[144]) & layer.Equals(fields[145]) & normalizedTimeOffset.Equals(fields[146]) & normalizedTransitionTime.Equals(fields[147]) &  !always) return;
			fields[143] = stateName;
			fields[144] = normalizedTransitionDuration;
			fields[145] = layer;
			fields[146] = normalizedTimeOffset;
			fields[147] = normalizedTransitionTime;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { stateName,normalizedTransitionDuration,layer,normalizedTimeOffset,normalizedTransitionTime, } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 142,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[142]);
                eventsId[142] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    CrossFade(stateName,normalizedTransitionDuration,layer,normalizedTimeOffset,normalizedTransitionTime, true, 0, 0);
                }, null);
            }
        }
        public void CrossFade(System.Int32 stateHashName,System.Single normalizedTransitionDuration,System.Int32 layer,System.Single normalizedTimeOffset, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (stateHashName.Equals(fields[149]) & normalizedTransitionDuration.Equals(fields[150]) & layer.Equals(fields[151]) & normalizedTimeOffset.Equals(fields[152]) &  !always) return;
			fields[149] = stateHashName;
			fields[150] = normalizedTransitionDuration;
			fields[151] = layer;
			fields[152] = normalizedTimeOffset;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { stateHashName,normalizedTransitionDuration,layer,normalizedTimeOffset, } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 148,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[148]);
                eventsId[148] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    CrossFade(stateHashName,normalizedTransitionDuration,layer,normalizedTimeOffset, true, 0, 0);
                }, null);
            }
        }
        public void CrossFade(System.Int32 stateHashName,System.Single normalizedTransitionDuration,System.Int32 layer, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (stateHashName.Equals(fields[154]) & normalizedTransitionDuration.Equals(fields[155]) & layer.Equals(fields[156]) &  !always) return;
			fields[154] = stateHashName;
			fields[155] = normalizedTransitionDuration;
			fields[156] = layer;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { stateHashName,normalizedTransitionDuration,layer, } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 153,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[153]);
                eventsId[153] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    CrossFade(stateHashName,normalizedTransitionDuration,layer, true, 0, 0);
                }, null);
            }
        }
        public void CrossFade(System.Int32 stateHashName,System.Single normalizedTransitionDuration, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (stateHashName.Equals(fields[158]) & normalizedTransitionDuration.Equals(fields[159]) &  !always) return;
			fields[158] = stateHashName;
			fields[159] = normalizedTransitionDuration;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { stateHashName,normalizedTransitionDuration, } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 157,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[157]);
                eventsId[157] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    CrossFade(stateHashName,normalizedTransitionDuration, true, 0, 0);
                }, null);
            }
        }
        public void PlayInFixedTime(System.String stateName,System.Int32 layer, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (stateName.Equals(fields[161]) & layer.Equals(fields[162]) &  !always) return;
			fields[161] = stateName;
			fields[162] = layer;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { stateName,layer, } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 160,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[160]);
                eventsId[160] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    PlayInFixedTime(stateName,layer, true, 0, 0);
                }, null);
            }
        }
        public void PlayInFixedTime(System.String stateName, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (stateName.Equals(fields[164]) &  !always) return;
			fields[164] = stateName;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { stateName, } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 163,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[163]);
                eventsId[163] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    PlayInFixedTime(stateName, true, 0, 0);
                }, null);
            }
        }
        public void PlayInFixedTime(System.String stateName,System.Int32 layer,System.Single fixedTime, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (stateName.Equals(fields[166]) & layer.Equals(fields[167]) & fixedTime.Equals(fields[168]) &  !always) return;
			fields[166] = stateName;
			fields[167] = layer;
			fields[168] = fixedTime;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { stateName,layer,fixedTime, } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 165,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[165]);
                eventsId[165] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    PlayInFixedTime(stateName,layer,fixedTime, true, 0, 0);
                }, null);
            }
        }
        public void PlayInFixedTime(System.Int32 stateNameHash,System.Int32 layer, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (stateNameHash.Equals(fields[170]) & layer.Equals(fields[171]) &  !always) return;
			fields[170] = stateNameHash;
			fields[171] = layer;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { stateNameHash,layer, } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 169,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[169]);
                eventsId[169] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    PlayInFixedTime(stateNameHash,layer, true, 0, 0);
                }, null);
            }
        }
        public void PlayInFixedTime(System.Int32 stateNameHash, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (stateNameHash.Equals(fields[173]) &  !always) return;
			fields[173] = stateNameHash;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { stateNameHash, } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 172,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[172]);
                eventsId[172] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    PlayInFixedTime(stateNameHash, true, 0, 0);
                }, null);
            }
        }
        public void Play(System.String stateName,System.Int32 layer, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (stateName.Equals(fields[175]) & layer.Equals(fields[176]) &  !always) return;
			fields[175] = stateName;
			fields[176] = layer;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { stateName,layer, } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 174,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[174]);
                eventsId[174] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    Play(stateName,layer, true, 0, 0);
                }, null);
            }
        }
        public void Play(System.String stateName, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (stateName.Equals(fields[178]) &  !always) return;
			fields[178] = stateName;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { stateName, } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 177,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[177]);
                eventsId[177] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    Play(stateName, true, 0, 0);
                }, null);
            }
        }
        public void Play(System.String stateName,System.Int32 layer,System.Single normalizedTime, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (stateName.Equals(fields[180]) & layer.Equals(fields[181]) & normalizedTime.Equals(fields[182]) &  !always) return;
			fields[180] = stateName;
			fields[181] = layer;
			fields[182] = normalizedTime;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { stateName,layer,normalizedTime, } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 179,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[179]);
                eventsId[179] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    Play(stateName,layer,normalizedTime, true, 0, 0);
                }, null);
            }
        }
        public void Play(System.Int32 stateNameHash,System.Int32 layer, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (stateNameHash.Equals(fields[184]) & layer.Equals(fields[185]) &  !always) return;
			fields[184] = stateNameHash;
			fields[185] = layer;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { stateNameHash,layer, } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 183,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[183]);
                eventsId[183] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    Play(stateNameHash,layer, true, 0, 0);
                }, null);
            }
        }
        public void Play(System.Int32 stateNameHash, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (stateNameHash.Equals(fields[187]) &  !always) return;
			fields[187] = stateNameHash;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { stateNameHash, } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 186,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[186]);
                eventsId[186] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    Play(stateNameHash, true, 0, 0);
                }, null);
            }
        }
		public UnityEngine.Transform GetBoneTransform(UnityEngine.HumanBodyBones humanBoneId)
        {
            return self.GetBoneTransform(humanBoneId);
        }
        public void Rebind( bool always = false, int executeNumber = 0, float time = 0)
        {
            if ( !always) return;
			
            var buffer = SerializeModel(new RPCModel() { pars = new object[] {  } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 188,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[188]);
                eventsId[188] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    Rebind( true, 0, 0);
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
						var rootPosition = DeserializeObject<UnityEngine.Vector3>(new Segment(opt.buffer, false));
						fields[1] = rootPosition;
						self.rootPosition = rootPosition;
					}
                    break;
                case 2:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var rootRotation = DeserializeObject<UnityEngine.Quaternion>(new Segment(opt.buffer, false));
						fields[2] = rootRotation;
						self.rootRotation = rootRotation;
					}
                    break;
                case 3:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var applyRootMotion = DeserializeObject<System.Boolean>(new Segment(opt.buffer, false));
						fields[3] = applyRootMotion;
						self.applyRootMotion = applyRootMotion;
					}
                    break;
                case 4:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var updateMode = DeserializeObject<UnityEngine.AnimatorUpdateMode>(new Segment(opt.buffer, false));
						fields[4] = updateMode;
						self.updateMode = updateMode;
					}
                    break;
                case 5:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var bodyPosition = DeserializeObject<UnityEngine.Vector3>(new Segment(opt.buffer, false));
						fields[5] = bodyPosition;
						self.bodyPosition = bodyPosition;
					}
                    break;
                case 6:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var bodyRotation = DeserializeObject<UnityEngine.Quaternion>(new Segment(opt.buffer, false));
						fields[6] = bodyRotation;
						self.bodyRotation = bodyRotation;
					}
                    break;
                case 7:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var stabilizeFeet = DeserializeObject<System.Boolean>(new Segment(opt.buffer, false));
						fields[7] = stabilizeFeet;
						self.stabilizeFeet = stabilizeFeet;
					}
                    break;
                case 8:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var feetPivotActive = DeserializeObject<System.Single>(new Segment(opt.buffer, false));
						fields[8] = feetPivotActive;
						self.feetPivotActive = feetPivotActive;
					}
                    break;
                case 9:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var speed = DeserializeObject<System.Single>(new Segment(opt.buffer, false));
						fields[9] = speed;
						self.speed = speed;
					}
                    break;
                case 10:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var cullingMode = DeserializeObject<UnityEngine.AnimatorCullingMode>(new Segment(opt.buffer, false));
						fields[10] = cullingMode;
						self.cullingMode = cullingMode;
					}
                    break;
                case 11:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var playbackTime = DeserializeObject<System.Single>(new Segment(opt.buffer, false));
						fields[11] = playbackTime;
						self.playbackTime = playbackTime;
					}
                    break;
                case 12:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var recorderStartTime = DeserializeObject<System.Single>(new Segment(opt.buffer, false));
						fields[12] = recorderStartTime;
						self.recorderStartTime = recorderStartTime;
					}
                    break;
                case 13:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var recorderStopTime = DeserializeObject<System.Single>(new Segment(opt.buffer, false));
						fields[13] = recorderStopTime;
						self.recorderStopTime = recorderStopTime;
					}
                    break;
                case 14:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var runtimeAnimatorController = DeserializeObject<UnityEngine.RuntimeAnimatorController>(new Segment(opt.buffer, false));
						fields[14] = runtimeAnimatorController;
						self.runtimeAnimatorController = runtimeAnimatorController;
					}
                    break;
                case 15:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var avatar = DeserializeObject<UnityEngine.Avatar>(new Segment(opt.buffer, false));
						fields[15] = avatar;
						self.avatar = avatar;
					}
                    break;
                case 16:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var layersAffectMassCenter = DeserializeObject<System.Boolean>(new Segment(opt.buffer, false));
						fields[16] = layersAffectMassCenter;
						self.layersAffectMassCenter = layersAffectMassCenter;
					}
                    break;
                case 17:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var logWarnings = DeserializeObject<System.Boolean>(new Segment(opt.buffer, false));
						fields[17] = logWarnings;
						self.logWarnings = logWarnings;
					}
                    break;
                case 18:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var fireEvents = DeserializeObject<System.Boolean>(new Segment(opt.buffer, false));
						fields[18] = fireEvents;
						self.fireEvents = fireEvents;
					}
                    break;
                case 19:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var keepAnimatorControllerStateOnDisable = DeserializeObject<System.Boolean>(new Segment(opt.buffer, false));
						fields[19] = keepAnimatorControllerStateOnDisable;
						self.keepAnimatorControllerStateOnDisable = keepAnimatorControllerStateOnDisable;
					}
                    break;
                case 20:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var name = (System.String)(fields[21] = data.Obj);
						var value = (System.Single)(fields[22] = data.Obj);
						self.SetFloat(name,value);
					}
                    break;
                case 23:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var name = (System.String)(fields[24] = data.Obj);
						var value = (System.Single)(fields[25] = data.Obj);
						var dampTime = (System.Single)(fields[26] = data.Obj);
						var deltaTime = (System.Single)(fields[27] = data.Obj);
						self.SetFloat(name,value,dampTime,deltaTime);
					}
                    break;
                case 28:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var id = (System.Int32)(fields[29] = data.Obj);
						var value = (System.Single)(fields[30] = data.Obj);
						self.SetFloat(id,value);
					}
                    break;
                case 31:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var id = (System.Int32)(fields[32] = data.Obj);
						var value = (System.Single)(fields[33] = data.Obj);
						var dampTime = (System.Single)(fields[34] = data.Obj);
						var deltaTime = (System.Single)(fields[35] = data.Obj);
						self.SetFloat(id,value,dampTime,deltaTime);
					}
                    break;
                case 36:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var name = (System.String)(fields[37] = data.Obj);
						var value = (System.Boolean)(fields[38] = data.Obj);
						self.SetBool(name,value);
					}
                    break;
                case 39:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var id = (System.Int32)(fields[40] = data.Obj);
						var value = (System.Boolean)(fields[41] = data.Obj);
						self.SetBool(id,value);
					}
                    break;
                case 42:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var name = (System.String)(fields[43] = data.Obj);
						var value = (System.Int32)(fields[44] = data.Obj);
						self.SetInteger(name,value);
					}
                    break;
                case 45:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var id = (System.Int32)(fields[46] = data.Obj);
						var value = (System.Int32)(fields[47] = data.Obj);
						self.SetInteger(id,value);
					}
                    break;
                case 48:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var name = (System.String)(fields[49] = data.Obj);
						self.SetTrigger(name);
					}
                    break;
                case 50:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var id = (System.Int32)(fields[51] = data.Obj);
						self.SetTrigger(id);
					}
                    break;
                case 52:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var name = (System.String)(fields[53] = data.Obj);
						self.ResetTrigger(name);
					}
                    break;
                case 54:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var id = (System.Int32)(fields[55] = data.Obj);
						self.ResetTrigger(id);
					}
                    break;
                case 56:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var goal = (UnityEngine.AvatarIKGoal)(fields[57] = data.Obj);
						var goalPosition = (UnityEngine.Vector3)(fields[58] = data.Obj);
						self.SetIKPosition(goal,goalPosition);
					}
                    break;
                case 59:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var goal = (UnityEngine.AvatarIKGoal)(fields[60] = data.Obj);
						var goalRotation = (UnityEngine.Quaternion)(fields[61] = data.Obj);
						self.SetIKRotation(goal,goalRotation);
					}
                    break;
                case 62:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var goal = (UnityEngine.AvatarIKGoal)(fields[63] = data.Obj);
						var value = (System.Single)(fields[64] = data.Obj);
						self.SetIKPositionWeight(goal,value);
					}
                    break;
                case 65:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var goal = (UnityEngine.AvatarIKGoal)(fields[66] = data.Obj);
						var value = (System.Single)(fields[67] = data.Obj);
						self.SetIKRotationWeight(goal,value);
					}
                    break;
                case 68:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var hint = (UnityEngine.AvatarIKHint)(fields[69] = data.Obj);
						var hintPosition = (UnityEngine.Vector3)(fields[70] = data.Obj);
						self.SetIKHintPosition(hint,hintPosition);
					}
                    break;
                case 71:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var hint = (UnityEngine.AvatarIKHint)(fields[72] = data.Obj);
						var value = (System.Single)(fields[73] = data.Obj);
						self.SetIKHintPositionWeight(hint,value);
					}
                    break;
                case 74:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var lookAtPosition = (UnityEngine.Vector3)(fields[75] = data.Obj);
						self.SetLookAtPosition(lookAtPosition);
					}
                    break;
                case 76:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var weight = (System.Single)(fields[77] = data.Obj);
						self.SetLookAtWeight(weight);
					}
                    break;
                case 78:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var weight = (System.Single)(fields[79] = data.Obj);
						var bodyWeight = (System.Single)(fields[80] = data.Obj);
						self.SetLookAtWeight(weight,bodyWeight);
					}
                    break;
                case 81:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var weight = (System.Single)(fields[82] = data.Obj);
						var bodyWeight = (System.Single)(fields[83] = data.Obj);
						var headWeight = (System.Single)(fields[84] = data.Obj);
						self.SetLookAtWeight(weight,bodyWeight,headWeight);
					}
                    break;
                case 85:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var weight = (System.Single)(fields[86] = data.Obj);
						var bodyWeight = (System.Single)(fields[87] = data.Obj);
						var headWeight = (System.Single)(fields[88] = data.Obj);
						var eyesWeight = (System.Single)(fields[89] = data.Obj);
						self.SetLookAtWeight(weight,bodyWeight,headWeight,eyesWeight);
					}
                    break;
                case 90:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var weight = (System.Single)(fields[91] = data.Obj);
						var bodyWeight = (System.Single)(fields[92] = data.Obj);
						var headWeight = (System.Single)(fields[93] = data.Obj);
						var eyesWeight = (System.Single)(fields[94] = data.Obj);
						var clampWeight = (System.Single)(fields[95] = data.Obj);
						self.SetLookAtWeight(weight,bodyWeight,headWeight,eyesWeight,clampWeight);
					}
                    break;
                case 96:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var humanBoneId = (UnityEngine.HumanBodyBones)(fields[97] = data.Obj);
						var rotation = (UnityEngine.Quaternion)(fields[98] = data.Obj);
						self.SetBoneLocalRotation(humanBoneId,rotation);
					}
                    break;
                case 99:
                    {
						self.InterruptMatchTarget();
					}
                    break;
                case 100:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var stateName = (System.String)(fields[101] = data.Obj);
						var fixedTransitionDuration = (System.Single)(fields[102] = data.Obj);
						self.CrossFadeInFixedTime(stateName,fixedTransitionDuration);
					}
                    break;
                case 103:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var stateName = (System.String)(fields[104] = data.Obj);
						var fixedTransitionDuration = (System.Single)(fields[105] = data.Obj);
						var layer = (System.Int32)(fields[106] = data.Obj);
						self.CrossFadeInFixedTime(stateName,fixedTransitionDuration,layer);
					}
                    break;
                case 107:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var stateName = (System.String)(fields[108] = data.Obj);
						var fixedTransitionDuration = (System.Single)(fields[109] = data.Obj);
						var layer = (System.Int32)(fields[110] = data.Obj);
						var fixedTimeOffset = (System.Single)(fields[111] = data.Obj);
						self.CrossFadeInFixedTime(stateName,fixedTransitionDuration,layer,fixedTimeOffset);
					}
                    break;
                case 112:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var stateName = (System.String)(fields[113] = data.Obj);
						var fixedTransitionDuration = (System.Single)(fields[114] = data.Obj);
						var layer = (System.Int32)(fields[115] = data.Obj);
						var fixedTimeOffset = (System.Single)(fields[116] = data.Obj);
						var normalizedTransitionTime = (System.Single)(fields[117] = data.Obj);
						self.CrossFadeInFixedTime(stateName,fixedTransitionDuration,layer,fixedTimeOffset,normalizedTransitionTime);
					}
                    break;
                case 118:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var stateHashName = (System.Int32)(fields[119] = data.Obj);
						var fixedTransitionDuration = (System.Single)(fields[120] = data.Obj);
						var layer = (System.Int32)(fields[121] = data.Obj);
						var fixedTimeOffset = (System.Single)(fields[122] = data.Obj);
						self.CrossFadeInFixedTime(stateHashName,fixedTransitionDuration,layer,fixedTimeOffset);
					}
                    break;
                case 123:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var stateHashName = (System.Int32)(fields[124] = data.Obj);
						var fixedTransitionDuration = (System.Single)(fields[125] = data.Obj);
						var layer = (System.Int32)(fields[126] = data.Obj);
						self.CrossFadeInFixedTime(stateHashName,fixedTransitionDuration,layer);
					}
                    break;
                case 127:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var stateHashName = (System.Int32)(fields[128] = data.Obj);
						var fixedTransitionDuration = (System.Single)(fields[129] = data.Obj);
						self.CrossFadeInFixedTime(stateHashName,fixedTransitionDuration);
					}
                    break;
                case 130:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var stateName = (System.String)(fields[131] = data.Obj);
						var normalizedTransitionDuration = (System.Single)(fields[132] = data.Obj);
						var layer = (System.Int32)(fields[133] = data.Obj);
						var normalizedTimeOffset = (System.Single)(fields[134] = data.Obj);
						self.CrossFade(stateName,normalizedTransitionDuration,layer,normalizedTimeOffset);
					}
                    break;
                case 135:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var stateName = (System.String)(fields[136] = data.Obj);
						var normalizedTransitionDuration = (System.Single)(fields[137] = data.Obj);
						var layer = (System.Int32)(fields[138] = data.Obj);
						self.CrossFade(stateName,normalizedTransitionDuration,layer);
					}
                    break;
                case 139:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var stateName = (System.String)(fields[140] = data.Obj);
						var normalizedTransitionDuration = (System.Single)(fields[141] = data.Obj);
						self.CrossFade(stateName,normalizedTransitionDuration);
					}
                    break;
                case 142:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var stateName = (System.String)(fields[143] = data.Obj);
						var normalizedTransitionDuration = (System.Single)(fields[144] = data.Obj);
						var layer = (System.Int32)(fields[145] = data.Obj);
						var normalizedTimeOffset = (System.Single)(fields[146] = data.Obj);
						var normalizedTransitionTime = (System.Single)(fields[147] = data.Obj);
						self.CrossFade(stateName,normalizedTransitionDuration,layer,normalizedTimeOffset,normalizedTransitionTime);
					}
                    break;
                case 148:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var stateHashName = (System.Int32)(fields[149] = data.Obj);
						var normalizedTransitionDuration = (System.Single)(fields[150] = data.Obj);
						var layer = (System.Int32)(fields[151] = data.Obj);
						var normalizedTimeOffset = (System.Single)(fields[152] = data.Obj);
						self.CrossFade(stateHashName,normalizedTransitionDuration,layer,normalizedTimeOffset);
					}
                    break;
                case 153:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var stateHashName = (System.Int32)(fields[154] = data.Obj);
						var normalizedTransitionDuration = (System.Single)(fields[155] = data.Obj);
						var layer = (System.Int32)(fields[156] = data.Obj);
						self.CrossFade(stateHashName,normalizedTransitionDuration,layer);
					}
                    break;
                case 157:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var stateHashName = (System.Int32)(fields[158] = data.Obj);
						var normalizedTransitionDuration = (System.Single)(fields[159] = data.Obj);
						self.CrossFade(stateHashName,normalizedTransitionDuration);
					}
                    break;
                case 160:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var stateName = (System.String)(fields[161] = data.Obj);
						var layer = (System.Int32)(fields[162] = data.Obj);
						self.PlayInFixedTime(stateName,layer);
					}
                    break;
                case 163:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var stateName = (System.String)(fields[164] = data.Obj);
						self.PlayInFixedTime(stateName);
					}
                    break;
                case 165:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var stateName = (System.String)(fields[166] = data.Obj);
						var layer = (System.Int32)(fields[167] = data.Obj);
						var fixedTime = (System.Single)(fields[168] = data.Obj);
						self.PlayInFixedTime(stateName,layer,fixedTime);
					}
                    break;
                case 169:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var stateNameHash = (System.Int32)(fields[170] = data.Obj);
						var layer = (System.Int32)(fields[171] = data.Obj);
						self.PlayInFixedTime(stateNameHash,layer);
					}
                    break;
                case 172:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var stateNameHash = (System.Int32)(fields[173] = data.Obj);
						self.PlayInFixedTime(stateNameHash);
					}
                    break;
                case 174:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var stateName = (System.String)(fields[175] = data.Obj);
						var layer = (System.Int32)(fields[176] = data.Obj);
						self.Play(stateName,layer);
					}
                    break;
                case 177:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var stateName = (System.String)(fields[178] = data.Obj);
						self.Play(stateName);
					}
                    break;
                case 179:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var stateName = (System.String)(fields[180] = data.Obj);
						var layer = (System.Int32)(fields[181] = data.Obj);
						var normalizedTime = (System.Single)(fields[182] = data.Obj);
						self.Play(stateName,layer,normalizedTime);
					}
                    break;
                case 183:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var stateNameHash = (System.Int32)(fields[184] = data.Obj);
						var layer = (System.Int32)(fields[185] = data.Obj);
						self.Play(stateNameHash,layer);
					}
                    break;
                case 186:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var stateNameHash = (System.Int32)(fields[187] = data.Obj);
						self.Play(stateNameHash);
					}
                    break;
                case 188:
                    {
						self.Rebind();
					}
                    break;
            }
        }
    }
}
#endif