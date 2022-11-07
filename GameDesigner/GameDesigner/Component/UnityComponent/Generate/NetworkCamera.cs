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
    /// Camera同步组件, 此代码由BuildComponentTools工具生成, 如果同步发生相互影响的字段或属性, 请自行检查处理一下!
    /// </summary>
    [RequireComponent(typeof(UnityEngine.Camera))]
    public class NetworkCamera : NetworkBehaviour
    {
        private UnityEngine.Camera self;
        public bool autoCheck;
        private object[] fields;
		private int[] eventsId;
		
        public void Awake()
        {
            self = GetComponent<UnityEngine.Camera>();
			fields = new object[40];
			eventsId = new int[40];
            fields[1] = self.nearClipPlane;
            fields[2] = self.farClipPlane;
            fields[3] = self.fieldOfView;
            fields[4] = self.renderingPath;
            fields[5] = self.allowHDR;
            fields[6] = self.allowMSAA;
            fields[7] = self.allowDynamicResolution;
            fields[8] = self.forceIntoRenderTexture;
            fields[9] = self.orthographicSize;
            fields[10] = self.orthographic;
            fields[11] = self.opaqueSortMode;
            fields[12] = self.transparencySortMode;
            fields[13] = self.transparencySortAxis;
            fields[14] = self.depth;
            fields[15] = self.aspect;
            fields[16] = self.cullingMask;
            fields[17] = self.eventMask;
            fields[18] = self.layerCullSpherical;
            fields[19] = self.cameraType;
            fields[20] = self.overrideSceneCullingMask;
            fields[21] = self.useOcclusionCulling;
            fields[22] = self.backgroundColor;
            fields[23] = self.clearFlags;
            fields[24] = self.depthTextureMode;
            fields[25] = self.clearStencilAfterLightingPass;
            fields[26] = self.usePhysicalProperties;
            fields[27] = self.sensorSize;
            fields[28] = self.lensShift;
            fields[29] = self.focalLength;
            fields[30] = self.gateFit;
            fields[31] = self.rect;
            //fields[32] = self.pixelRect;
            fields[33] = self.targetTexture;
            fields[34] = self.targetDisplay;
            fields[35] = self.useJitteredProjectionMatrixForTransparentRendering;
            fields[36] = self.stereoSeparation;
            fields[37] = self.stereoConvergence;
            fields[38] = self.stereoTargetEye;
        }

        public System.Single nearClipPlane
        {
            get
            {
                return self.nearClipPlane;
            }
            set
            {
                if (value.Equals(fields[1]))
                    return;
                fields[1] = value;
                self.nearClipPlane = value;
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
        public System.Single farClipPlane
        {
            get
            {
                return self.farClipPlane;
            }
            set
            {
                if (value.Equals(fields[2]))
                    return;
                fields[2] = value;
                self.farClipPlane = value;
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
        public System.Single fieldOfView
        {
            get
            {
                return self.fieldOfView;
            }
            set
            {
                if (value.Equals(fields[3]))
                    return;
                fields[3] = value;
                self.fieldOfView = value;
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
        public UnityEngine.RenderingPath renderingPath
        {
            get
            {
                return self.renderingPath;
            }
            set
            {
                if (value.Equals(fields[4]))
                    return;
                fields[4] = value;
                self.renderingPath = value;
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
        public System.Boolean allowHDR
        {
            get
            {
                return self.allowHDR;
            }
            set
            {
                if (value.Equals(fields[5]))
                    return;
                fields[5] = value;
                self.allowHDR = value;
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
        public System.Boolean allowMSAA
        {
            get
            {
                return self.allowMSAA;
            }
            set
            {
                if (value.Equals(fields[6]))
                    return;
                fields[6] = value;
                self.allowMSAA = value;
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
        public System.Boolean allowDynamicResolution
        {
            get
            {
                return self.allowDynamicResolution;
            }
            set
            {
                if (value.Equals(fields[7]))
                    return;
                fields[7] = value;
                self.allowDynamicResolution = value;
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
        public System.Boolean forceIntoRenderTexture
        {
            get
            {
                return self.forceIntoRenderTexture;
            }
            set
            {
                if (value.Equals(fields[8]))
                    return;
                fields[8] = value;
                self.forceIntoRenderTexture = value;
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
        public System.Single orthographicSize
        {
            get
            {
                return self.orthographicSize;
            }
            set
            {
                if (value.Equals(fields[9]))
                    return;
                fields[9] = value;
                self.orthographicSize = value;
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
        public System.Boolean orthographic
        {
            get
            {
                return self.orthographic;
            }
            set
            {
                if (value.Equals(fields[10]))
                    return;
                fields[10] = value;
                self.orthographic = value;
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
        public UnityEngine.Rendering.OpaqueSortMode opaqueSortMode
        {
            get
            {
                return self.opaqueSortMode;
            }
            set
            {
                if (value.Equals(fields[11]))
                    return;
                fields[11] = value;
                self.opaqueSortMode = value;
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
        public UnityEngine.TransparencySortMode transparencySortMode
        {
            get
            {
                return self.transparencySortMode;
            }
            set
            {
                if (value.Equals(fields[12]))
                    return;
                fields[12] = value;
                self.transparencySortMode = value;
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
        public UnityEngine.Vector3 transparencySortAxis
        {
            get
            {
                return self.transparencySortAxis;
            }
            set
            {
                if (value.Equals(fields[13]))
                    return;
                fields[13] = value;
                self.transparencySortAxis = value;
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
        public System.Single depth
        {
            get
            {
                return self.depth;
            }
            set
            {
                if (value.Equals(fields[14]))
                    return;
                fields[14] = value;
                self.depth = value;
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
        public System.Single aspect
        {
            get
            {
                return self.aspect;
            }
            set
            {
                if (value.Equals(fields[15]))
                    return;
                fields[15] = value;
                self.aspect = value;
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
        public System.Int32 cullingMask
        {
            get
            {
                return self.cullingMask;
            }
            set
            {
                if (value.Equals(fields[16]))
                    return;
                fields[16] = value;
                self.cullingMask = value;
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
        public System.Int32 eventMask
        {
            get
            {
                return self.eventMask;
            }
            set
            {
                if (value.Equals(fields[17]))
                    return;
                fields[17] = value;
                self.eventMask = value;
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
        public System.Boolean layerCullSpherical
        {
            get
            {
                return self.layerCullSpherical;
            }
            set
            {
                if (value.Equals(fields[18]))
                    return;
                fields[18] = value;
                self.layerCullSpherical = value;
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
        public UnityEngine.CameraType cameraType
        {
            get
            {
                return self.cameraType;
            }
            set
            {
                if (value.Equals(fields[19]))
                    return;
                fields[19] = value;
                self.cameraType = value;
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
        public System.UInt64 overrideSceneCullingMask
        {
            get
            {
                return self.overrideSceneCullingMask;
            }
            set
            {
                if (value.Equals(fields[20]))
                    return;
                fields[20] = value;
                self.overrideSceneCullingMask = value;
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
        public System.Boolean useOcclusionCulling
        {
            get
            {
                return self.useOcclusionCulling;
            }
            set
            {
                if (value.Equals(fields[21]))
                    return;
                fields[21] = value;
                self.useOcclusionCulling = value;
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
        public UnityEngine.Color backgroundColor
        {
            get
            {
                return self.backgroundColor;
            }
            set
            {
                if (value.Equals(fields[22]))
                    return;
                fields[22] = value;
                self.backgroundColor = value;
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
        public UnityEngine.CameraClearFlags clearFlags
        {
            get
            {
                return self.clearFlags;
            }
            set
            {
                if (value.Equals(fields[23]))
                    return;
                fields[23] = value;
                self.clearFlags = value;
                ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = Index,
                    index2 = 23,
                    buffer = SerializeObject(value).ToArray(true),
                    uid = ClientBase.Instance.UID
                });
            }
        }
        public UnityEngine.DepthTextureMode depthTextureMode
        {
            get
            {
                return self.depthTextureMode;
            }
            set
            {
                if (value.Equals(fields[24]))
                    return;
                fields[24] = value;
                self.depthTextureMode = value;
                ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = Index,
                    index2 = 24,
                    buffer = SerializeObject(value).ToArray(true),
                    uid = ClientBase.Instance.UID
                });
            }
        }
        public System.Boolean clearStencilAfterLightingPass
        {
            get
            {
                return self.clearStencilAfterLightingPass;
            }
            set
            {
                if (value.Equals(fields[25]))
                    return;
                fields[25] = value;
                self.clearStencilAfterLightingPass = value;
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
        public System.Boolean usePhysicalProperties
        {
            get
            {
                return self.usePhysicalProperties;
            }
            set
            {
                if (value.Equals(fields[26]))
                    return;
                fields[26] = value;
                self.usePhysicalProperties = value;
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
        public UnityEngine.Vector2 sensorSize
        {
            get
            {
                return self.sensorSize;
            }
            set
            {
                if (value.Equals(fields[27]))
                    return;
                fields[27] = value;
                self.sensorSize = value;
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
        public UnityEngine.Vector2 lensShift
        {
            get
            {
                return self.lensShift;
            }
            set
            {
                if (value.Equals(fields[28]))
                    return;
                fields[28] = value;
                self.lensShift = value;
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
        public System.Single focalLength
        {
            get
            {
                return self.focalLength;
            }
            set
            {
                if (value.Equals(fields[29]))
                    return;
                fields[29] = value;
                self.focalLength = value;
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
        public UnityEngine.Camera.GateFitMode gateFit
        {
            get
            {
                return self.gateFit;
            }
            set
            {
                if (value.Equals(fields[30]))
                    return;
                fields[30] = value;
                self.gateFit = value;
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
        public UnityEngine.Rect rect
        {
            get
            {
                return self.rect;
            }
            set
            {
                if (value.Equals(fields[31]))
                    return;
                fields[31] = value;
                self.rect = value;
                ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = Index,
                    index2 = 31,
                    buffer = SerializeObject(value).ToArray(true),
                    uid = ClientBase.Instance.UID
                });
            }
        }
        //public UnityEngine.Rect pixelRect
        //{
        //    get
        //    {
        //        return self.pixelRect;
        //    }
        //    set
        //    {
        //        if (value.Equals(fields[32]))
        //            return;
        //        fields[32] = value;
        //        self.pixelRect = value;
        //        ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
        //        {
        //            index = netObj.registerObjectIndex,
        //            index1 = Index,
        //            index2 = 32,
        //            buffer = SerializeObject(value).ToArray(true),
        //            uid = ClientBase.Instance.UID
        //        });
        //    }
        //}
        public UnityEngine.RenderTexture targetTexture
        {
            get
            {
                return self.targetTexture;
            }
            set
            {
                if (value == null)
                    return;
                if (value.Equals(fields[33]))
                    return;
                fields[33] = value;
                self.targetTexture = value;
                ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = Index,
                    index2 = 33,
                    buffer = SerializeObject(value).ToArray(true),
                    uid = ClientBase.Instance.UID
                });
            }
        }
        public System.Int32 targetDisplay
        {
            get
            {
                return self.targetDisplay;
            }
            set
            {
                if (value.Equals(fields[34]))
                    return;
                fields[34] = value;
                self.targetDisplay = value;
                ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = Index,
                    index2 = 34,
                    buffer = SerializeObject(value).ToArray(true),
                    uid = ClientBase.Instance.UID
                });
            }
        }
        public System.Boolean useJitteredProjectionMatrixForTransparentRendering
        {
            get
            {
                return self.useJitteredProjectionMatrixForTransparentRendering;
            }
            set
            {
                if (value.Equals(fields[35]))
                    return;
                fields[35] = value;
                self.useJitteredProjectionMatrixForTransparentRendering = value;
                ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = Index,
                    index2 = 35,
                    buffer = SerializeObject(value).ToArray(true),
                    uid = ClientBase.Instance.UID
                });
            }
        }
        public System.Single stereoSeparation
        {
            get
            {
                return self.stereoSeparation;
            }
            set
            {
                if (value.Equals(fields[36]))
                    return;
                fields[36] = value;
                self.stereoSeparation = value;
                ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = Index,
                    index2 = 36,
                    buffer = SerializeObject(value).ToArray(true),
                    uid = ClientBase.Instance.UID
                });
            }
        }
        public System.Single stereoConvergence
        {
            get
            {
                return self.stereoConvergence;
            }
            set
            {
                if (value.Equals(fields[37]))
                    return;
                fields[37] = value;
                self.stereoConvergence = value;
                ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = Index,
                    index2 = 37,
                    buffer = SerializeObject(value).ToArray(true),
                    uid = ClientBase.Instance.UID
                });
            }
        }
        public UnityEngine.StereoTargetEyeMask stereoTargetEye
        {
            get
            {
                return self.stereoTargetEye;
            }
            set
            {
                if (value.Equals(fields[38]))
                    return;
                fields[38] = value;
                self.stereoTargetEye = value;
                ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = Index,
                    index2 = 38,
                    buffer = SerializeObject(value).ToArray(true),
                    uid = ClientBase.Instance.UID
                });
            }
        }
        public override void OnPropertyAutoCheck()
        {
            if (!autoCheck)
                return;

            nearClipPlane = nearClipPlane;
            farClipPlane = farClipPlane;
            fieldOfView = fieldOfView;
            renderingPath = renderingPath;
            allowHDR = allowHDR;
            allowMSAA = allowMSAA;
            allowDynamicResolution = allowDynamicResolution;
            forceIntoRenderTexture = forceIntoRenderTexture;
            orthographicSize = orthographicSize;
            orthographic = orthographic;
            opaqueSortMode = opaqueSortMode;
            transparencySortMode = transparencySortMode;
            transparencySortAxis = transparencySortAxis;
            depth = depth;
            aspect = aspect;
            cullingMask = cullingMask;
            eventMask = eventMask;
            layerCullSpherical = layerCullSpherical;
            cameraType = cameraType;
            overrideSceneCullingMask = overrideSceneCullingMask;
            useOcclusionCulling = useOcclusionCulling;
            backgroundColor = backgroundColor;
            clearFlags = clearFlags;
            depthTextureMode = depthTextureMode;
            clearStencilAfterLightingPass = clearStencilAfterLightingPass;
            usePhysicalProperties = usePhysicalProperties;
            sensorSize = sensorSize;
            lensShift = lensShift;
            focalLength = focalLength;
            gateFit = gateFit;
            rect = rect;
            //pixelRect = pixelRect;
            targetTexture = targetTexture;
            targetDisplay = targetDisplay;
            useJitteredProjectionMatrixForTransparentRendering = useJitteredProjectionMatrixForTransparentRendering;
            stereoSeparation = stereoSeparation;
            stereoConvergence = stereoConvergence;
            stereoTargetEye = stereoTargetEye;
        }

		public UnityEngine.Vector2 GetGateFittedLensShift()
        {
            return self.GetGateFittedLensShift();
        }
        public void CalculateObliqueMatrix(UnityEngine.Vector4 clipPlane, bool always = false, int executeNumber = 0, float time = 0)
        {
            if (clipPlane.Equals(fields[40]) &  !always) return;
			fields[40] = clipPlane;
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { clipPlane, } });
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
                    CalculateObliqueMatrix(clipPlane, true, 0, 0);
                }, null);
            }
        }
		public UnityEngine.Vector3 WorldToScreenPoint(UnityEngine.Vector3 position,UnityEngine.Camera.MonoOrStereoscopicEye eye)
        {
            return self.WorldToScreenPoint(position,eye);
        }
		public UnityEngine.Vector3 WorldToViewportPoint(UnityEngine.Vector3 position,UnityEngine.Camera.MonoOrStereoscopicEye eye)
        {
            return self.WorldToViewportPoint(position,eye);
        }
		public UnityEngine.Vector3 ViewportToWorldPoint(UnityEngine.Vector3 position,UnityEngine.Camera.MonoOrStereoscopicEye eye)
        {
            return self.ViewportToWorldPoint(position,eye);
        }
		public UnityEngine.Vector3 ScreenToWorldPoint(UnityEngine.Vector3 position,UnityEngine.Camera.MonoOrStereoscopicEye eye)
        {
            return self.ScreenToWorldPoint(position,eye);
        }
		public UnityEngine.Vector3 WorldToScreenPoint(UnityEngine.Vector3 position)
        {
            return self.WorldToScreenPoint(position);
        }
		public UnityEngine.Vector3 WorldToViewportPoint(UnityEngine.Vector3 position)
        {
            return self.WorldToViewportPoint(position);
        }
		public UnityEngine.Vector3 ViewportToWorldPoint(UnityEngine.Vector3 position)
        {
            return self.ViewportToWorldPoint(position);
        }
		public UnityEngine.Vector3 ScreenToWorldPoint(UnityEngine.Vector3 position)
        {
            return self.ScreenToWorldPoint(position);
        }
		public UnityEngine.Vector3 ScreenToViewportPoint(UnityEngine.Vector3 position)
        {
            return self.ScreenToViewportPoint(position);
        }
		public UnityEngine.Vector3 ViewportToScreenPoint(UnityEngine.Vector3 position)
        {
            return self.ViewportToScreenPoint(position);
        }
		public UnityEngine.Ray ViewportPointToRay(UnityEngine.Vector3 pos,UnityEngine.Camera.MonoOrStereoscopicEye eye)
        {
            return self.ViewportPointToRay(pos,eye);
        }
		public UnityEngine.Ray ViewportPointToRay(UnityEngine.Vector3 pos)
        {
            return self.ViewportPointToRay(pos);
        }
		public UnityEngine.Ray ScreenPointToRay(UnityEngine.Vector3 pos,UnityEngine.Camera.MonoOrStereoscopicEye eye)
        {
            return self.ScreenPointToRay(pos,eye);
        }
		public UnityEngine.Ray ScreenPointToRay(UnityEngine.Vector3 pos)
        {
            return self.ScreenPointToRay(pos);
        }
		public UnityEngine.Matrix4x4 GetStereoNonJitteredProjectionMatrix(UnityEngine.Camera.StereoscopicEye eye)
        {
            return self.GetStereoNonJitteredProjectionMatrix(eye);
        }
		public UnityEngine.Matrix4x4 GetStereoViewMatrix(UnityEngine.Camera.StereoscopicEye eye)
        {
            return self.GetStereoViewMatrix(eye);
        }
		public UnityEngine.Matrix4x4 GetStereoProjectionMatrix(UnityEngine.Camera.StereoscopicEye eye)
        {
            return self.GetStereoProjectionMatrix(eye);
        }
        public override void OnNetworkOperationHandler(Operation opt)
        {
            switch (opt.index2)
            {

                case 1:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var nearClipPlane = DeserializeObject<System.Single>(new Segment(opt.buffer, false));
						fields[1] = nearClipPlane;
						self.nearClipPlane = nearClipPlane;
					}
                    break;
                case 2:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var farClipPlane = DeserializeObject<System.Single>(new Segment(opt.buffer, false));
						fields[2] = farClipPlane;
						self.farClipPlane = farClipPlane;
					}
                    break;
                case 3:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var fieldOfView = DeserializeObject<System.Single>(new Segment(opt.buffer, false));
						fields[3] = fieldOfView;
						self.fieldOfView = fieldOfView;
					}
                    break;
                case 4:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var renderingPath = DeserializeObject<UnityEngine.RenderingPath>(new Segment(opt.buffer, false));
						fields[4] = renderingPath;
						self.renderingPath = renderingPath;
					}
                    break;
                case 5:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var allowHDR = DeserializeObject<System.Boolean>(new Segment(opt.buffer, false));
						fields[5] = allowHDR;
						self.allowHDR = allowHDR;
					}
                    break;
                case 6:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var allowMSAA = DeserializeObject<System.Boolean>(new Segment(opt.buffer, false));
						fields[6] = allowMSAA;
						self.allowMSAA = allowMSAA;
					}
                    break;
                case 7:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var allowDynamicResolution = DeserializeObject<System.Boolean>(new Segment(opt.buffer, false));
						fields[7] = allowDynamicResolution;
						self.allowDynamicResolution = allowDynamicResolution;
					}
                    break;
                case 8:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var forceIntoRenderTexture = DeserializeObject<System.Boolean>(new Segment(opt.buffer, false));
						fields[8] = forceIntoRenderTexture;
						self.forceIntoRenderTexture = forceIntoRenderTexture;
					}
                    break;
                case 9:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var orthographicSize = DeserializeObject<System.Single>(new Segment(opt.buffer, false));
						fields[9] = orthographicSize;
						self.orthographicSize = orthographicSize;
					}
                    break;
                case 10:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var orthographic = DeserializeObject<System.Boolean>(new Segment(opt.buffer, false));
						fields[10] = orthographic;
						self.orthographic = orthographic;
					}
                    break;
                case 11:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var opaqueSortMode = DeserializeObject<UnityEngine.Rendering.OpaqueSortMode>(new Segment(opt.buffer, false));
						fields[11] = opaqueSortMode;
						self.opaqueSortMode = opaqueSortMode;
					}
                    break;
                case 12:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var transparencySortMode = DeserializeObject<UnityEngine.TransparencySortMode>(new Segment(opt.buffer, false));
						fields[12] = transparencySortMode;
						self.transparencySortMode = transparencySortMode;
					}
                    break;
                case 13:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var transparencySortAxis = DeserializeObject<UnityEngine.Vector3>(new Segment(opt.buffer, false));
						fields[13] = transparencySortAxis;
						self.transparencySortAxis = transparencySortAxis;
					}
                    break;
                case 14:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var depth = DeserializeObject<System.Single>(new Segment(opt.buffer, false));
						fields[14] = depth;
						self.depth = depth;
					}
                    break;
                case 15:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var aspect = DeserializeObject<System.Single>(new Segment(opt.buffer, false));
						fields[15] = aspect;
						self.aspect = aspect;
					}
                    break;
                case 16:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var cullingMask = DeserializeObject<System.Int32>(new Segment(opt.buffer, false));
						fields[16] = cullingMask;
						self.cullingMask = cullingMask;
					}
                    break;
                case 17:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var eventMask = DeserializeObject<System.Int32>(new Segment(opt.buffer, false));
						fields[17] = eventMask;
						self.eventMask = eventMask;
					}
                    break;
                case 18:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var layerCullSpherical = DeserializeObject<System.Boolean>(new Segment(opt.buffer, false));
						fields[18] = layerCullSpherical;
						self.layerCullSpherical = layerCullSpherical;
					}
                    break;
                case 19:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var cameraType = DeserializeObject<UnityEngine.CameraType>(new Segment(opt.buffer, false));
						fields[19] = cameraType;
						self.cameraType = cameraType;
					}
                    break;
                case 20:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var overrideSceneCullingMask = DeserializeObject<System.UInt64>(new Segment(opt.buffer, false));
						fields[20] = overrideSceneCullingMask;
						self.overrideSceneCullingMask = overrideSceneCullingMask;
					}
                    break;
                case 21:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var useOcclusionCulling = DeserializeObject<System.Boolean>(new Segment(opt.buffer, false));
						fields[21] = useOcclusionCulling;
						self.useOcclusionCulling = useOcclusionCulling;
					}
                    break;
                case 22:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var backgroundColor = DeserializeObject<UnityEngine.Color>(new Segment(opt.buffer, false));
						fields[22] = backgroundColor;
						self.backgroundColor = backgroundColor;
					}
                    break;
                case 23:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var clearFlags = DeserializeObject<UnityEngine.CameraClearFlags>(new Segment(opt.buffer, false));
						fields[23] = clearFlags;
						self.clearFlags = clearFlags;
					}
                    break;
                case 24:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var depthTextureMode = DeserializeObject<UnityEngine.DepthTextureMode>(new Segment(opt.buffer, false));
						fields[24] = depthTextureMode;
						self.depthTextureMode = depthTextureMode;
					}
                    break;
                case 25:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var clearStencilAfterLightingPass = DeserializeObject<System.Boolean>(new Segment(opt.buffer, false));
						fields[25] = clearStencilAfterLightingPass;
						self.clearStencilAfterLightingPass = clearStencilAfterLightingPass;
					}
                    break;
                case 26:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var usePhysicalProperties = DeserializeObject<System.Boolean>(new Segment(opt.buffer, false));
						fields[26] = usePhysicalProperties;
						self.usePhysicalProperties = usePhysicalProperties;
					}
                    break;
                case 27:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var sensorSize = DeserializeObject<UnityEngine.Vector2>(new Segment(opt.buffer, false));
						fields[27] = sensorSize;
						self.sensorSize = sensorSize;
					}
                    break;
                case 28:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var lensShift = DeserializeObject<UnityEngine.Vector2>(new Segment(opt.buffer, false));
						fields[28] = lensShift;
						self.lensShift = lensShift;
					}
                    break;
                case 29:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var focalLength = DeserializeObject<System.Single>(new Segment(opt.buffer, false));
						fields[29] = focalLength;
						self.focalLength = focalLength;
					}
                    break;
                case 30:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var gateFit = DeserializeObject<UnityEngine.Camera.GateFitMode>(new Segment(opt.buffer, false));
						fields[30] = gateFit;
						self.gateFit = gateFit;
					}
                    break;
                case 31:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var rect = DeserializeObject<UnityEngine.Rect>(new Segment(opt.buffer, false));
						fields[31] = rect;
						self.rect = rect;
					}
                    break;
     //           case 32:
     //               {
					//	if (opt.uid == ClientBase.Instance.UID)
					//		return;
					//	var pixelRect = DeserializeObject<UnityEngine.Rect>(new Segment(opt.buffer, false));
					//	fields[32] = pixelRect;
					//	self.pixelRect = pixelRect;
					//}
     //               break;
                case 33:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var targetTexture = DeserializeObject<UnityEngine.RenderTexture>(new Segment(opt.buffer, false));
						fields[33] = targetTexture;
						self.targetTexture = targetTexture;
					}
                    break;
                case 34:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var targetDisplay = DeserializeObject<System.Int32>(new Segment(opt.buffer, false));
						fields[34] = targetDisplay;
						self.targetDisplay = targetDisplay;
					}
                    break;
                case 35:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var useJitteredProjectionMatrixForTransparentRendering = DeserializeObject<System.Boolean>(new Segment(opt.buffer, false));
						fields[35] = useJitteredProjectionMatrixForTransparentRendering;
						self.useJitteredProjectionMatrixForTransparentRendering = useJitteredProjectionMatrixForTransparentRendering;
					}
                    break;
                case 36:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var stereoSeparation = DeserializeObject<System.Single>(new Segment(opt.buffer, false));
						fields[36] = stereoSeparation;
						self.stereoSeparation = stereoSeparation;
					}
                    break;
                case 37:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var stereoConvergence = DeserializeObject<System.Single>(new Segment(opt.buffer, false));
						fields[37] = stereoConvergence;
						self.stereoConvergence = stereoConvergence;
					}
                    break;
                case 38:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var stereoTargetEye = DeserializeObject<UnityEngine.StereoTargetEyeMask>(new Segment(opt.buffer, false));
						fields[38] = stereoTargetEye;
						self.stereoTargetEye = stereoTargetEye;
					}
                    break;
                case 39:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						var clipPlane = (UnityEngine.Vector4)(fields[40] = data.Obj);
						self.CalculateObliqueMatrix(clipPlane);
					}
                    break;
            }
        }
    }
}
#endif