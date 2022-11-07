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
    /// Light同步组件, 此代码由BuildComponentTools工具生成, 如果同步发生相互影响的字段或属性, 请自行检查处理一下!
    /// </summary>
    [RequireComponent(typeof(UnityEngine.Light))]
    public class NetworkLight : NetworkBehaviour
    {
        private UnityEngine.Light self;
        public bool autoCheck;
        private object[] fields;
		
        public void Awake()
        {
            self = GetComponent<UnityEngine.Light>();
			fields = new object[32];
            fields[1] = self.type;
            fields[2] = self.shape;
            fields[3] = self.spotAngle;
            fields[4] = self.innerSpotAngle;
            fields[5] = self.color;
            fields[6] = self.colorTemperature;
            fields[7] = self.useColorTemperature;
            fields[8] = self.intensity;
            fields[9] = self.bounceIntensity;
            fields[10] = self.useBoundingSphereOverride;
            fields[11] = self.boundingSphereOverride;
            //fields[12] = self.useViewFrustumForShadowCasterCull;
            fields[13] = self.shadowCustomResolution;
            fields[14] = self.shadowBias;
            fields[15] = self.shadowNormalBias;
            fields[16] = self.shadowNearPlane;
            fields[17] = self.useShadowMatrixOverride;
            fields[18] = self.range;
            fields[19] = self.flare;
            fields[20] = self.cullingMask;
            fields[21] = self.renderingLayerMask;
            fields[22] = self.lightShadowCasterMode;
            fields[25] = self.shadows;
            fields[26] = self.shadowStrength;
            fields[27] = self.shadowResolution;
            fields[28] = self.cookieSize;
            fields[29] = self.cookie;
            fields[30] = self.renderMode;
        }

        public UnityEngine.LightType type
        {
            get
            {
                return self.type;
            }
            set
            {
                if (value.Equals(fields[1]))
                    return;
                fields[1] = value;
                self.type = value;
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
        public UnityEngine.LightShape shape
        {
            get
            {
                return self.shape;
            }
            set
            {
                if (value.Equals(fields[2]))
                    return;
                fields[2] = value;
                self.shape = value;
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
        public System.Single spotAngle
        {
            get
            {
                return self.spotAngle;
            }
            set
            {
                if (value.Equals(fields[3]))
                    return;
                fields[3] = value;
                self.spotAngle = value;
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
        public System.Single innerSpotAngle
        {
            get
            {
                return self.innerSpotAngle;
            }
            set
            {
                if (value.Equals(fields[4]))
                    return;
                fields[4] = value;
                self.innerSpotAngle = value;
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
        public UnityEngine.Color color
        {
            get
            {
                return self.color;
            }
            set
            {
                if (value.Equals(fields[5]))
                    return;
                fields[5] = value;
                self.color = value;
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
        public System.Single colorTemperature
        {
            get
            {
                return self.colorTemperature;
            }
            set
            {
                if (value.Equals(fields[6]))
                    return;
                fields[6] = value;
                self.colorTemperature = value;
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
        public System.Boolean useColorTemperature
        {
            get
            {
                return self.useColorTemperature;
            }
            set
            {
                if (value.Equals(fields[7]))
                    return;
                fields[7] = value;
                self.useColorTemperature = value;
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
        public System.Single intensity
        {
            get
            {
                return self.intensity;
            }
            set
            {
                if (value.Equals(fields[8]))
                    return;
                fields[8] = value;
                self.intensity = value;
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
        public System.Single bounceIntensity
        {
            get
            {
                return self.bounceIntensity;
            }
            set
            {
                if (value.Equals(fields[9]))
                    return;
                fields[9] = value;
                self.bounceIntensity = value;
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
        public System.Boolean useBoundingSphereOverride
        {
            get
            {
                return self.useBoundingSphereOverride;
            }
            set
            {
                if (value.Equals(fields[10]))
                    return;
                fields[10] = value;
                self.useBoundingSphereOverride = value;
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
        public UnityEngine.Vector4 boundingSphereOverride
        {
            get
            {
                return self.boundingSphereOverride;
            }
            set
            {
                if (value.Equals(fields[11]))
                    return;
                fields[11] = value;
                self.boundingSphereOverride = value;
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
        //public System.Boolean useViewFrustumForShadowCasterCull
        //{
        //    get
        //    {
        //        return self.useViewFrustumForShadowCasterCull;
        //    }
        //    set
        //    {
        //        if (value.Equals(fields[12]))
        //            return;
        //        fields[12] = value;
        //        self.useViewFrustumForShadowCasterCull = value;
        //        ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
        //        {
        //            index = netObj.registerObjectIndex,
        //            index1 = Index,
        //            index2 = 12,
        //            buffer = SerializeObject(value).ToArray(true),
        //            uid = ClientBase.Instance.UID
        //        });
        //    }
        //}
        public System.Int32 shadowCustomResolution
        {
            get
            {
                return self.shadowCustomResolution;
            }
            set
            {
                if (value.Equals(fields[13]))
                    return;
                fields[13] = value;
                self.shadowCustomResolution = value;
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
        public System.Single shadowBias
        {
            get
            {
                return self.shadowBias;
            }
            set
            {
                if (value.Equals(fields[14]))
                    return;
                fields[14] = value;
                self.shadowBias = value;
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
        public System.Single shadowNormalBias
        {
            get
            {
                return self.shadowNormalBias;
            }
            set
            {
                if (value.Equals(fields[15]))
                    return;
                fields[15] = value;
                self.shadowNormalBias = value;
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
        public System.Single shadowNearPlane
        {
            get
            {
                return self.shadowNearPlane;
            }
            set
            {
                if (value.Equals(fields[16]))
                    return;
                fields[16] = value;
                self.shadowNearPlane = value;
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
        public System.Boolean useShadowMatrixOverride
        {
            get
            {
                return self.useShadowMatrixOverride;
            }
            set
            {
                if (value.Equals(fields[17]))
                    return;
                fields[17] = value;
                self.useShadowMatrixOverride = value;
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
        public System.Single range
        {
            get
            {
                return self.range;
            }
            set
            {
                if (value.Equals(fields[18]))
                    return;
                fields[18] = value;
                self.range = value;
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
        public UnityEngine.Flare flare
        {
            get
            {
                return self.flare;
            }
            set
            {
                if (value == null)
                    return;
                if (value.Equals(fields[19]))
                    return;
                fields[19] = value;
                self.flare = value;
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
        public System.Int32 cullingMask
        {
            get
            {
                return self.cullingMask;
            }
            set
            {
                if (value.Equals(fields[20]))
                    return;
                fields[20] = value;
                self.cullingMask = value;
                ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = Index,
                    index2 = 20,
                    buffer = SerializeObject(value).ToArray(true),
                    uid = ClientBase.Instance.UID
                });
            }
        }
        public System.Int32 renderingLayerMask
        {
            get
            {
                return self.renderingLayerMask;
            }
            set
            {
                if (value.Equals(fields[21]))
                    return;
                fields[21] = value;
                self.renderingLayerMask = value;
                ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = Index,
                    index2 = 21,
                    buffer = SerializeObject(value).ToArray(true),
                    uid = ClientBase.Instance.UID
                });
            }
        }
        public UnityEngine.LightShadowCasterMode lightShadowCasterMode
        {
            get
            {
                return self.lightShadowCasterMode;
            }
            set
            {
                if (value.Equals(fields[22]))
                    return;
                fields[22] = value;
                self.lightShadowCasterMode = value;
                ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = Index,
                    index2 = 22,
                    buffer = SerializeObject(value).ToArray(true),
                    uid = ClientBase.Instance.UID
                });
            }
        }
        public UnityEngine.LightShadows shadows
        {
            get
            {
                return self.shadows;
            }
            set
            {
                if (value.Equals(fields[25]))
                    return;
                fields[25] = value;
                self.shadows = value;
                ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = Index,
                    index2 = 25,
                    buffer = SerializeObject(value).ToArray(true),
                    uid = ClientBase.Instance.UID
                });
            }
        }
        public System.Single shadowStrength
        {
            get
            {
                return self.shadowStrength;
            }
            set
            {
                if (value.Equals(fields[26]))
                    return;
                fields[26] = value;
                self.shadowStrength = value;
                ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = Index,
                    index2 = 26,
                    buffer = SerializeObject(value).ToArray(true),
                    uid = ClientBase.Instance.UID
                });
            }
        }
        public UnityEngine.Rendering.LightShadowResolution shadowResolution
        {
            get
            {
                return self.shadowResolution;
            }
            set
            {
                if (value.Equals(fields[27]))
                    return;
                fields[27] = value;
                self.shadowResolution = value;
                ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = Index,
                    index2 = 27,
                    buffer = SerializeObject(value).ToArray(true),
                    uid = ClientBase.Instance.UID
                });
            }
        }
        public System.Single cookieSize
        {
            get
            {
                return self.cookieSize;
            }
            set
            {
                if (value.Equals(fields[28]))
                    return;
                fields[28] = value;
                self.cookieSize = value;
                ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = Index,
                    index2 = 28,
                    buffer = SerializeObject(value).ToArray(true),
                    uid = ClientBase.Instance.UID
                });
            }
        }
        public UnityEngine.Texture cookie
        {
            get
            {
                return self.cookie;
            }
            set
            {
                if (value == null)
                    return;
                if (value.Equals(fields[29]))
                    return;
                fields[29] = value;
                self.cookie = value;
                ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = Index,
                    index2 = 29,
                    buffer = SerializeObject(value).ToArray(true),
                    uid = ClientBase.Instance.UID
                });
            }
        }
        public UnityEngine.LightRenderMode renderMode
        {
            get
            {
                return self.renderMode;
            }
            set
            {
                if (value.Equals(fields[30]))
                    return;
                fields[30] = value;
                self.renderMode = value;
                ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = Index,
                    index2 = 30,
                    buffer = SerializeObject(value).ToArray(true),
                    uid = ClientBase.Instance.UID
                });
            }
        }
        public override void OnPropertyAutoCheck()
        {
            if (!autoCheck)
                return;

            type = type;
            shape = shape;
            spotAngle = spotAngle;
            innerSpotAngle = innerSpotAngle;
            color = color;
            colorTemperature = colorTemperature;
            useColorTemperature = useColorTemperature;
            intensity = intensity;
            bounceIntensity = bounceIntensity;
            useBoundingSphereOverride = useBoundingSphereOverride;
            boundingSphereOverride = boundingSphereOverride;
            //useViewFrustumForShadowCasterCull = useViewFrustumForShadowCasterCull;
            shadowCustomResolution = shadowCustomResolution;
            shadowBias = shadowBias;
            shadowNormalBias = shadowNormalBias;
            shadowNearPlane = shadowNearPlane;
            useShadowMatrixOverride = useShadowMatrixOverride;
            range = range;
            flare = flare;
            cullingMask = cullingMask;
            renderingLayerMask = renderingLayerMask;
            lightShadowCasterMode = lightShadowCasterMode;
            shadows = shadows;
            shadowStrength = shadowStrength;
            shadowResolution = shadowResolution;
            cookieSize = cookieSize;
            cookie = cookie;
            renderMode = renderMode;
        }

        public override void OnNetworkOperationHandler(Operation opt)
        {
            switch (opt.index2)
            {

                case 1:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var type = DeserializeObject<UnityEngine.LightType>(new Segment(opt.buffer, false));
						fields[1] = type;
						self.type = type;
					}
                    break;
                case 2:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var shape = DeserializeObject<UnityEngine.LightShape>(new Segment(opt.buffer, false));
						fields[2] = shape;
						self.shape = shape;
					}
                    break;
                case 3:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var spotAngle = DeserializeObject<System.Single>(new Segment(opt.buffer, false));
						fields[3] = spotAngle;
						self.spotAngle = spotAngle;
					}
                    break;
                case 4:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var innerSpotAngle = DeserializeObject<System.Single>(new Segment(opt.buffer, false));
						fields[4] = innerSpotAngle;
						self.innerSpotAngle = innerSpotAngle;
					}
                    break;
                case 5:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var color = DeserializeObject<UnityEngine.Color>(new Segment(opt.buffer, false));
						fields[5] = color;
						self.color = color;
					}
                    break;
                case 6:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var colorTemperature = DeserializeObject<System.Single>(new Segment(opt.buffer, false));
						fields[6] = colorTemperature;
						self.colorTemperature = colorTemperature;
					}
                    break;
                case 7:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var useColorTemperature = DeserializeObject<System.Boolean>(new Segment(opt.buffer, false));
						fields[7] = useColorTemperature;
						self.useColorTemperature = useColorTemperature;
					}
                    break;
                case 8:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var intensity = DeserializeObject<System.Single>(new Segment(opt.buffer, false));
						fields[8] = intensity;
						self.intensity = intensity;
					}
                    break;
                case 9:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var bounceIntensity = DeserializeObject<System.Single>(new Segment(opt.buffer, false));
						fields[9] = bounceIntensity;
						self.bounceIntensity = bounceIntensity;
					}
                    break;
                case 10:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var useBoundingSphereOverride = DeserializeObject<System.Boolean>(new Segment(opt.buffer, false));
						fields[10] = useBoundingSphereOverride;
						self.useBoundingSphereOverride = useBoundingSphereOverride;
					}
                    break;
                case 11:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var boundingSphereOverride = DeserializeObject<UnityEngine.Vector4>(new Segment(opt.buffer, false));
						fields[11] = boundingSphereOverride;
						self.boundingSphereOverride = boundingSphereOverride;
					}
                    break;
     //           case 12:
     //               {
					//	if (opt.uid == ClientBase.Instance.UID)
					//		return;
					//	var useViewFrustumForShadowCasterCull = DeserializeObject<System.Boolean>(new Segment(opt.buffer, false));
					//	fields[12] = useViewFrustumForShadowCasterCull;
					//	self.useViewFrustumForShadowCasterCull = useViewFrustumForShadowCasterCull;
					//}
     //               break;
                case 13:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var shadowCustomResolution = DeserializeObject<System.Int32>(new Segment(opt.buffer, false));
						fields[13] = shadowCustomResolution;
						self.shadowCustomResolution = shadowCustomResolution;
					}
                    break;
                case 14:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var shadowBias = DeserializeObject<System.Single>(new Segment(opt.buffer, false));
						fields[14] = shadowBias;
						self.shadowBias = shadowBias;
					}
                    break;
                case 15:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var shadowNormalBias = DeserializeObject<System.Single>(new Segment(opt.buffer, false));
						fields[15] = shadowNormalBias;
						self.shadowNormalBias = shadowNormalBias;
					}
                    break;
                case 16:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var shadowNearPlane = DeserializeObject<System.Single>(new Segment(opt.buffer, false));
						fields[16] = shadowNearPlane;
						self.shadowNearPlane = shadowNearPlane;
					}
                    break;
                case 17:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var useShadowMatrixOverride = DeserializeObject<System.Boolean>(new Segment(opt.buffer, false));
						fields[17] = useShadowMatrixOverride;
						self.useShadowMatrixOverride = useShadowMatrixOverride;
					}
                    break;
                case 18:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var range = DeserializeObject<System.Single>(new Segment(opt.buffer, false));
						fields[18] = range;
						self.range = range;
					}
                    break;
                case 19:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var flare = DeserializeObject<UnityEngine.Flare>(new Segment(opt.buffer, false));
						fields[19] = flare;
						self.flare = flare;
					}
                    break;
                case 20:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var cullingMask = DeserializeObject<System.Int32>(new Segment(opt.buffer, false));
						fields[20] = cullingMask;
						self.cullingMask = cullingMask;
					}
                    break;
                case 21:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var renderingLayerMask = DeserializeObject<System.Int32>(new Segment(opt.buffer, false));
						fields[21] = renderingLayerMask;
						self.renderingLayerMask = renderingLayerMask;
					}
                    break;
                case 22:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var lightShadowCasterMode = DeserializeObject<UnityEngine.LightShadowCasterMode>(new Segment(opt.buffer, false));
						fields[22] = lightShadowCasterMode;
						self.lightShadowCasterMode = lightShadowCasterMode;
					}
                    break;
                case 25:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var shadows = DeserializeObject<UnityEngine.LightShadows>(new Segment(opt.buffer, false));
						fields[25] = shadows;
						self.shadows = shadows;
					}
                    break;
                case 26:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var shadowStrength = DeserializeObject<System.Single>(new Segment(opt.buffer, false));
						fields[26] = shadowStrength;
						self.shadowStrength = shadowStrength;
					}
                    break;
                case 27:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var shadowResolution = DeserializeObject<UnityEngine.Rendering.LightShadowResolution>(new Segment(opt.buffer, false));
						fields[27] = shadowResolution;
						self.shadowResolution = shadowResolution;
					}
                    break;
                case 28:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var cookieSize = DeserializeObject<System.Single>(new Segment(opt.buffer, false));
						fields[28] = cookieSize;
						self.cookieSize = cookieSize;
					}
                    break;
                case 29:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var cookie = DeserializeObject<UnityEngine.Texture>(new Segment(opt.buffer, false));
						fields[29] = cookie;
						self.cookie = cookie;
					}
                    break;
                case 30:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var renderMode = DeserializeObject<UnityEngine.LightRenderMode>(new Segment(opt.buffer, false));
						fields[30] = renderMode;
						self.renderMode = renderMode;
					}
                    break;
            }
        }
    }
}
#endif