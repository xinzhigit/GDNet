#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA || UNITY_WEBGL
using System;
using UnityEngine;

namespace LockStep.Client
{
    [Serializable]
    public class Actor : ECS.Component
    {
        public string name;
        public GameObject gameObject;
        //public TSTransform transform;
        public Rigidbody rigidBody;
    }
}
#endif