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
    /// Image同步组件, 此代码由BuildComponentTools工具生成, 如果同步发生相互影响的字段或属性, 请自行检查处理一下!
    /// </summary>
    [RequireComponent(typeof(UnityEngine.UI.Image))]
    public class NetworkImage : NetworkBehaviour
    {
        private UnityEngine.UI.Image self;
        public bool autoCheck;
        private object[] fields;
		private int[] eventsId;
		
        public void Awake()
        {
            self = GetComponent<UnityEngine.UI.Image>();
			fields = new object[61];
			eventsId = new int[61];
            fields[1] = self.sprite;
            fields[2] = self.type;
            fields[3] = self.preserveAspect;
            fields[4] = self.fillCenter;
            fields[5] = self.fillMethod;
            fields[6] = self.fillAmount;
            fields[7] = self.fillClockwise;
            fields[8] = self.fillOrigin;
            fields[9] = self.alphaHitTestMinimumThreshold;
            fields[10] = self.useSpriteMesh;
            fields[11] = self.pixelsPerUnitMultiplier;
            fields[12] = self.material;
            fields[13] = self.maskable;
            fields[14] = self.isMaskingGraphic;
            fields[15] = self.color;
            fields[16] = self.raycastTarget;
            //fields[17] = self.raycastPadding;
            fields[18] = self.useGUILayout;
        }

        public UnityEngine.Sprite sprite
        {
            get
            {
                return self.sprite;
            }
            set
            {
                if (value.Equals(fields[1]))
                    return;
                fields[1] = value;
                self.sprite = value;
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
        public UnityEngine.UI.Image.Type type
        {
            get
            {
                return self.type;
            }
            set
            {
                if (value.Equals(fields[2]))
                    return;
                fields[2] = value;
                self.type = value;
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
        public System.Boolean preserveAspect
        {
            get
            {
                return self.preserveAspect;
            }
            set
            {
                if (value.Equals(fields[3]))
                    return;
                fields[3] = value;
                self.preserveAspect = value;
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
        public System.Boolean fillCenter
        {
            get
            {
                return self.fillCenter;
            }
            set
            {
                if (value.Equals(fields[4]))
                    return;
                fields[4] = value;
                self.fillCenter = value;
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
        public UnityEngine.UI.Image.FillMethod fillMethod
        {
            get
            {
                return self.fillMethod;
            }
            set
            {
                if (value.Equals(fields[5]))
                    return;
                fields[5] = value;
                self.fillMethod = value;
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
        public System.Single fillAmount
        {
            get
            {
                return self.fillAmount;
            }
            set
            {
                if (value.Equals(fields[6]))
                    return;
                fields[6] = value;
                self.fillAmount = value;
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
        public System.Boolean fillClockwise
        {
            get
            {
                return self.fillClockwise;
            }
            set
            {
                if (value.Equals(fields[7]))
                    return;
                fields[7] = value;
                self.fillClockwise = value;
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
        public System.Int32 fillOrigin
        {
            get
            {
                return self.fillOrigin;
            }
            set
            {
                if (value.Equals(fields[8]))
                    return;
                fields[8] = value;
                self.fillOrigin = value;
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
        public System.Single alphaHitTestMinimumThreshold
        {
            get
            {
                return self.alphaHitTestMinimumThreshold;
            }
            set
            {
                if (value.Equals(fields[9]))
                    return;
                fields[9] = value;
                self.alphaHitTestMinimumThreshold = value;
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
        public System.Boolean useSpriteMesh
        {
            get
            {
                return self.useSpriteMesh;
            }
            set
            {
                if (value.Equals(fields[10]))
                    return;
                fields[10] = value;
                self.useSpriteMesh = value;
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
        public System.Single pixelsPerUnitMultiplier
        {
            get
            {
                return self.pixelsPerUnitMultiplier;
            }
            set
            {
                if (value.Equals(fields[11]))
                    return;
                fields[11] = value;
                self.pixelsPerUnitMultiplier = value;
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
        public UnityEngine.Material material
        {
            get
            {
                return self.material;
            }
            set
            {
                if (value.Equals(fields[12]))
                    return;
                fields[12] = value;
                self.material = value;
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
        public System.Boolean maskable
        {
            get
            {
                return self.maskable;
            }
            set
            {
                if (value.Equals(fields[13]))
                    return;
                fields[13] = value;
                self.maskable = value;
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
        public System.Boolean isMaskingGraphic
        {
            get
            {
                return self.isMaskingGraphic;
            }
            set
            {
                if (value.Equals(fields[14]))
                    return;
                fields[14] = value;
                self.isMaskingGraphic = value;
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
        public UnityEngine.Color color
        {
            get
            {
                return self.color;
            }
            set
            {
                if (value.Equals(fields[15]))
                    return;
                fields[15] = value;
                self.color = value;
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
        public System.Boolean raycastTarget
        {
            get
            {
                return self.raycastTarget;
            }
            set
            {
                if (value.Equals(fields[16]))
                    return;
                fields[16] = value;
                self.raycastTarget = value;
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
        //public UnityEngine.Vector4 raycastPadding
        //{
        //    get
        //    {
        //        return self.raycastPadding;
        //    }
        //    set
        //    {
        //        if (value.Equals(fields[17]))
        //            return;
        //        fields[17] = value;
        //        self.raycastPadding = value;
        //        ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
        //        {
        //            index = netObj.registerObjectIndex,
        //            index1 = Index,
        //            index2 = 17,
        //            buffer = SerializeObject(value).ToArray(true),
        //            uid = ClientBase.Instance.UID
        //        });
        //    }
        //}
        public System.Boolean useGUILayout
        {
            get
            {
                return self.useGUILayout;
            }
            set
            {
                if (value.Equals(fields[18]))
                    return;
                fields[18] = value;
                self.useGUILayout = value;
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
        public override void OnPropertyAutoCheck()
        {
            if (!autoCheck)
                return;

            sprite = sprite;
            type = type;
            preserveAspect = preserveAspect;
            fillCenter = fillCenter;
            fillMethod = fillMethod;
            fillAmount = fillAmount;
            fillClockwise = fillClockwise;
            fillOrigin = fillOrigin;
            alphaHitTestMinimumThreshold = alphaHitTestMinimumThreshold;
            useSpriteMesh = useSpriteMesh;
            pixelsPerUnitMultiplier = pixelsPerUnitMultiplier;
            material = material;
            maskable = maskable;
            isMaskingGraphic = isMaskingGraphic;
            color = color;
            raycastTarget = raycastTarget;
            //raycastPadding = raycastPadding;
            useGUILayout = useGUILayout;
        }

        public void DisableSpriteOptimizations( bool always = false, int executeNumber = 0, float time = 0)
        {
            if ( !always) return;
			
            var buffer = SerializeModel(new RPCModel() { pars = new object[] {  } });
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
                    DisableSpriteOptimizations( true, 0, 0);
                }, null);
            }
        }
        public void OnBeforeSerialize( bool always = false, int executeNumber = 0, float time = 0)
        {
            if ( !always) return;
			
            var buffer = SerializeModel(new RPCModel() { pars = new object[] {  } });
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
                    OnBeforeSerialize( true, 0, 0);
                }, null);
            }
        }
        public void OnAfterDeserialize( bool always = false, int executeNumber = 0, float time = 0)
        {
            if ( !always) return;
			
            var buffer = SerializeModel(new RPCModel() { pars = new object[] {  } });
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
                    OnAfterDeserialize( true, 0, 0);
                }, null);
            }
        }
        public void SetNativeSize( bool always = false, int executeNumber = 0, float time = 0)
        {
            if ( !always) return;
			
            var buffer = SerializeModel(new RPCModel() { pars = new object[] {  } });
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
                    SetNativeSize( true, 0, 0);
                }, null);
            }
        }
        public void CalculateLayoutInputHorizontal( bool always = false, int executeNumber = 0, float time = 0)
        {
            if ( !always) return;
			
            var buffer = SerializeModel(new RPCModel() { pars = new object[] {  } });
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
                    CalculateLayoutInputHorizontal( true, 0, 0);
                }, null);
            }
        }
        public void CalculateLayoutInputVertical( bool always = false, int executeNumber = 0, float time = 0)
        {
            if ( !always) return;
			
            var buffer = SerializeModel(new RPCModel() { pars = new object[] {  } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 24,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[24]);
                eventsId[24] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    CalculateLayoutInputVertical( true, 0, 0);
                }, null);
            }
        }
        public void Cull(UnityEngine.Rect clipRect,System.Boolean validRect, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (clipRect.Equals(fields[26]) & validRect.Equals(fields[27]) &  !always) return;
			fields[26] = clipRect;
			fields[27] = validRect;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { clipRect,validRect, } });
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
                    Cull(clipRect,validRect, true, 0, 0);
                }, null);
            }
        }
        public void SetClipRect(UnityEngine.Rect clipRect,System.Boolean validRect, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (clipRect.Equals(fields[29]) & validRect.Equals(fields[30]) &  !always) return;
			fields[29] = clipRect;
			fields[30] = validRect;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { clipRect,validRect, } });
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
                    SetClipRect(clipRect,validRect, true, 0, 0);
                }, null);
            }
        }
        public void SetClipSoftness(UnityEngine.Vector2 clipSoftness, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (clipSoftness.Equals(fields[32]) &  !always) return;
			fields[32] = clipSoftness;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { clipSoftness, } });
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
                    SetClipSoftness(clipSoftness, true, 0, 0);
                }, null);
            }
        }
        public void RecalculateClipping( bool always = false, int executeNumber = 0, float time = 0)
        {
            if ( !always) return;
			
            var buffer = SerializeModel(new RPCModel() { pars = new object[] {  } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 33,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[33]);
                eventsId[33] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    RecalculateClipping( true, 0, 0);
                }, null);
            }
        }
        public void RecalculateMasking( bool always = false, int executeNumber = 0, float time = 0)
        {
            if ( !always) return;
			
            var buffer = SerializeModel(new RPCModel() { pars = new object[] {  } });
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
                    RecalculateMasking( true, 0, 0);
                }, null);
            }
        }
        public void SetAllDirty( bool always = false, int executeNumber = 0, float time = 0)
        {
            if ( !always) return;
			
            var buffer = SerializeModel(new RPCModel() { pars = new object[] {  } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 35,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[35]);
                eventsId[35] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    SetAllDirty( true, 0, 0);
                }, null);
            }
        }
        public void SetLayoutDirty( bool always = false, int executeNumber = 0, float time = 0)
        {
            if ( !always) return;
			
            var buffer = SerializeModel(new RPCModel() { pars = new object[] {  } });
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
                    SetLayoutDirty( true, 0, 0);
                }, null);
            }
        }
        public void SetVerticesDirty( bool always = false, int executeNumber = 0, float time = 0)
        {
            if ( !always) return;
			
            var buffer = SerializeModel(new RPCModel() { pars = new object[] {  } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 37,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[37]);
                eventsId[37] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    SetVerticesDirty( true, 0, 0);
                }, null);
            }
        }
        public void SetMaterialDirty( bool always = false, int executeNumber = 0, float time = 0)
        {
            if ( !always) return;
			
            var buffer = SerializeModel(new RPCModel() { pars = new object[] {  } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 38,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[38]);
                eventsId[38] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    SetMaterialDirty( true, 0, 0);
                }, null);
            }
        }
        public void OnCullingChanged( bool always = false, int executeNumber = 0, float time = 0)
        {
            if ( !always) return;
			
            var buffer = SerializeModel(new RPCModel() { pars = new object[] {  } });
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
                    OnCullingChanged( true, 0, 0);
                }, null);
            }
        }
        public void Rebuild(UnityEngine.UI.CanvasUpdate update, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (update.Equals(fields[41]) &  !always) return;
			fields[41] = update;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { update, } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 40,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[40]);
                eventsId[40] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    Rebuild(update, true, 0, 0);
                }, null);
            }
        }
        public void LayoutComplete( bool always = false, int executeNumber = 0, float time = 0)
        {
            if ( !always) return;
			
            var buffer = SerializeModel(new RPCModel() { pars = new object[] {  } });
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
                    LayoutComplete( true, 0, 0);
                }, null);
            }
        }
        public void GraphicUpdateComplete( bool always = false, int executeNumber = 0, float time = 0)
        {
            if ( !always) return;
			
            var buffer = SerializeModel(new RPCModel() { pars = new object[] {  } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 43,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[43]);
                eventsId[43] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    GraphicUpdateComplete( true, 0, 0);
                }, null);
            }
        }
        public void PixelAdjustPoint(UnityEngine.Vector2 point, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (point.Equals(fields[46]) &  !always) return;
			fields[46] = point;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { point, } });
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
                    PixelAdjustPoint(point, true, 0, 0);
                }, null);
            }
        }
		public UnityEngine.Rect GetPixelAdjustedRect()
        {
            return self.GetPixelAdjustedRect();
        }
        public void CrossFadeColor(UnityEngine.Color targetColor,System.Single duration,System.Boolean ignoreTimeScale,System.Boolean useAlpha, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (targetColor.Equals(fields[48]) & duration.Equals(fields[49]) & ignoreTimeScale.Equals(fields[50]) & useAlpha.Equals(fields[51]) &  !always) return;
			fields[48] = targetColor;
			fields[49] = duration;
			fields[50] = ignoreTimeScale;
			fields[51] = useAlpha;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { targetColor,duration,ignoreTimeScale,useAlpha, } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 47,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[47]);
                eventsId[47] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    CrossFadeColor(targetColor,duration,ignoreTimeScale,useAlpha, true, 0, 0);
                }, null);
            }
        }
        public void CrossFadeColor(UnityEngine.Color targetColor,System.Single duration,System.Boolean ignoreTimeScale,System.Boolean useAlpha,System.Boolean useRGB, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (targetColor.Equals(fields[53]) & duration.Equals(fields[54]) & ignoreTimeScale.Equals(fields[55]) & useAlpha.Equals(fields[56]) & useRGB.Equals(fields[57]) &  !always) return;
			fields[53] = targetColor;
			fields[54] = duration;
			fields[55] = ignoreTimeScale;
			fields[56] = useAlpha;
			fields[57] = useRGB;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { targetColor,duration,ignoreTimeScale,useAlpha,useRGB, } });
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
                    CrossFadeColor(targetColor,duration,ignoreTimeScale,useAlpha,useRGB, true, 0, 0);
                }, null);
            }
        }
        public void CrossFadeAlpha(System.Single alpha,System.Single duration,System.Boolean ignoreTimeScale, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (alpha.Equals(fields[59]) & duration.Equals(fields[60]) & ignoreTimeScale.Equals(fields[61]) &  !always) return;
			fields[59] = alpha;
			fields[60] = duration;
			fields[61] = ignoreTimeScale;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { alpha,duration,ignoreTimeScale, } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = 58,
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[58]);
                eventsId[58] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    CrossFadeAlpha(alpha,duration,ignoreTimeScale, true, 0, 0);
                }, null);
            }
        }
		public System.Boolean IsActive()
        {
            return self.IsActive();
        }
		public System.Boolean IsDestroyed()
        {
            return self.IsDestroyed();
        }
		public System.Boolean IsInvoking()
        {
            return self.IsInvoking();
        }
		public void CancelInvoke()
        {
            self.CancelInvoke();
        }
		public void Invoke(System.String methodName,System.Single time)
        {
            self.Invoke(methodName,time);
        }
		public void InvokeRepeating(System.String methodName,System.Single time,System.Single repeatRate)
        {
            self.InvokeRepeating(methodName,time,repeatRate);
        }
		public void CancelInvoke(System.String methodName)
        {
            self.CancelInvoke(methodName);
        }
		public System.Boolean IsInvoking(System.String methodName)
        {
            return self.IsInvoking(methodName);
        }
		public UnityEngine.Coroutine StartCoroutine(System.String methodName)
        {
            return self.StartCoroutine(methodName);
        }
        public override void OnNetworkOperationHandler(Operation opt)
        {
            switch (opt.index2)
            {

                case 1:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var sprite = DeserializeObject<UnityEngine.Sprite>(new Segment(opt.buffer, false));
						fields[1] = sprite;
						self.sprite = sprite;
					}
                    break;
                case 2:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var type = DeserializeObject<UnityEngine.UI.Image.Type>(new Segment(opt.buffer, false));
						fields[2] = type;
						self.type = type;
					}
                    break;
                case 3:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var preserveAspect = DeserializeObject<System.Boolean>(new Segment(opt.buffer, false));
						fields[3] = preserveAspect;
						self.preserveAspect = preserveAspect;
					}
                    break;
                case 4:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var fillCenter = DeserializeObject<System.Boolean>(new Segment(opt.buffer, false));
						fields[4] = fillCenter;
						self.fillCenter = fillCenter;
					}
                    break;
                case 5:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var fillMethod = DeserializeObject<UnityEngine.UI.Image.FillMethod>(new Segment(opt.buffer, false));
						fields[5] = fillMethod;
						self.fillMethod = fillMethod;
					}
                    break;
                case 6:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var fillAmount = DeserializeObject<System.Single>(new Segment(opt.buffer, false));
						fields[6] = fillAmount;
						self.fillAmount = fillAmount;
					}
                    break;
                case 7:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var fillClockwise = DeserializeObject<System.Boolean>(new Segment(opt.buffer, false));
						fields[7] = fillClockwise;
						self.fillClockwise = fillClockwise;
					}
                    break;
                case 8:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var fillOrigin = DeserializeObject<System.Int32>(new Segment(opt.buffer, false));
						fields[8] = fillOrigin;
						self.fillOrigin = fillOrigin;
					}
                    break;
                case 9:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var alphaHitTestMinimumThreshold = DeserializeObject<System.Single>(new Segment(opt.buffer, false));
						fields[9] = alphaHitTestMinimumThreshold;
						self.alphaHitTestMinimumThreshold = alphaHitTestMinimumThreshold;
					}
                    break;
                case 10:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var useSpriteMesh = DeserializeObject<System.Boolean>(new Segment(opt.buffer, false));
						fields[10] = useSpriteMesh;
						self.useSpriteMesh = useSpriteMesh;
					}
                    break;
                case 11:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var pixelsPerUnitMultiplier = DeserializeObject<System.Single>(new Segment(opt.buffer, false));
						fields[11] = pixelsPerUnitMultiplier;
						self.pixelsPerUnitMultiplier = pixelsPerUnitMultiplier;
					}
                    break;
                case 12:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var material = DeserializeObject<UnityEngine.Material>(new Segment(opt.buffer, false));
						fields[12] = material;
						self.material = material;
					}
                    break;
                case 13:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var maskable = DeserializeObject<System.Boolean>(new Segment(opt.buffer, false));
						fields[13] = maskable;
						self.maskable = maskable;
					}
                    break;
                case 14:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var isMaskingGraphic = DeserializeObject<System.Boolean>(new Segment(opt.buffer, false));
						fields[14] = isMaskingGraphic;
						self.isMaskingGraphic = isMaskingGraphic;
					}
                    break;
                case 15:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var color = DeserializeObject<UnityEngine.Color>(new Segment(opt.buffer, false));
						fields[15] = color;
						self.color = color;
					}
                    break;
                case 16:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var raycastTarget = DeserializeObject<System.Boolean>(new Segment(opt.buffer, false));
						fields[16] = raycastTarget;
						self.raycastTarget = raycastTarget;
					}
                    break;
     //           case 17:
     //               {
					//	if (opt.uid == ClientBase.Instance.UID)
					//		return;
					//	var raycastPadding = DeserializeObject<UnityEngine.Vector4>(new Segment(opt.buffer, false));
					//	fields[17] = raycastPadding;
					//	self.raycastPadding = raycastPadding;
					//}
     //               break;
                case 18:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var useGUILayout = DeserializeObject<System.Boolean>(new Segment(opt.buffer, false));
						fields[18] = useGUILayout;
						self.useGUILayout = useGUILayout;
					}
                    break;
                case 19:
                    {
						self.DisableSpriteOptimizations();
					}
                    break;
                case 20:
                    {
						self.OnBeforeSerialize();
					}
                    break;
                case 21:
                    {
						self.OnAfterDeserialize();
					}
                    break;
                case 22:
                    {
						self.SetNativeSize();
					}
                    break;
                case 23:
                    {
						self.CalculateLayoutInputHorizontal();
					}
                    break;
                case 24:
                    {
						self.CalculateLayoutInputVertical();
					}
                    break;
                case 25:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var clipRect = (UnityEngine.Rect)(fields[26] = data.Obj);
						var validRect = (System.Boolean)(fields[27] = data.Obj);
						self.Cull(clipRect,validRect);
					}
                    break;
                case 28:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var clipRect = (UnityEngine.Rect)(fields[29] = data.Obj);
						var validRect = (System.Boolean)(fields[30] = data.Obj);
						self.SetClipRect(clipRect,validRect);
					}
                    break;
                case 31:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var clipSoftness = (UnityEngine.Vector2)(fields[32] = data.Obj);
						self.SetClipSoftness(clipSoftness);
					}
                    break;
                case 33:
                    {
						self.RecalculateClipping();
					}
                    break;
                case 34:
                    {
						self.RecalculateMasking();
					}
                    break;
                case 35:
                    {
						self.SetAllDirty();
					}
                    break;
                case 36:
                    {
						self.SetLayoutDirty();
					}
                    break;
                case 37:
                    {
						self.SetVerticesDirty();
					}
                    break;
                case 38:
                    {
						self.SetMaterialDirty();
					}
                    break;
                case 39:
                    {
						self.OnCullingChanged();
					}
                    break;
                case 40:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var update = (UnityEngine.UI.CanvasUpdate)(fields[41] = data.Obj);
						self.Rebuild(update);
					}
                    break;
                case 42:
                    {
						self.LayoutComplete();
					}
                    break;
                case 43:
                    {
						self.GraphicUpdateComplete();
					}
                    break;
                case 45:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var point = (UnityEngine.Vector2)(fields[46] = data.Obj);
						self.PixelAdjustPoint(point);
					}
                    break;
                case 47:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var targetColor = (UnityEngine.Color)(fields[48] = data.Obj);
						var duration = (System.Single)(fields[49] = data.Obj);
						var ignoreTimeScale = (System.Boolean)(fields[50] = data.Obj);
						var useAlpha = (System.Boolean)(fields[51] = data.Obj);
						self.CrossFadeColor(targetColor,duration,ignoreTimeScale,useAlpha);
					}
                    break;
                case 52:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var targetColor = (UnityEngine.Color)(fields[53] = data.Obj);
						var duration = (System.Single)(fields[54] = data.Obj);
						var ignoreTimeScale = (System.Boolean)(fields[55] = data.Obj);
						var useAlpha = (System.Boolean)(fields[56] = data.Obj);
						var useRGB = (System.Boolean)(fields[57] = data.Obj);
						self.CrossFadeColor(targetColor,duration,ignoreTimeScale,useAlpha,useRGB);
					}
                    break;
                case 58:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var alpha = (System.Single)(fields[59] = data.Obj);
						var duration = (System.Single)(fields[60] = data.Obj);
						var ignoreTimeScale = (System.Boolean)(fields[61] = data.Obj);
						self.CrossFadeAlpha(alpha,duration,ignoreTimeScale);
					}
                    break;
            }
        }
    }
}
#endif