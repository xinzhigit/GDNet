#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA || UNITY_WEBGL
using ECS;
using Net.Share;
using System;
using UnityEngine;

namespace LockStep.Client
{
    [Serializable]
    public class Player : Actor, IUpdate
    {
        public ObjectView objectView;
        public float moveSpeed = 6f;
        internal Operation opt;
        
        public void OnUpdate()
        {
            var dir = opt.direction;
            if (dir == Net.Vector3.zero)
                objectView.anim.Play("soldierIdle");
            else
            {
                objectView.anim.Play("soldierRun");
                gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, Quaternion.LookRotation(dir, Vector3.up), 0.5f);
                gameObject.transform.Translate(0, 0, moveSpeed * LSTime.deltaTime);
            }
        }

        public override void OnDestroy()
        {
            UnityEngine.Object.DestroyImmediate(objectView.gameObject);
        }
    }
}
#endif