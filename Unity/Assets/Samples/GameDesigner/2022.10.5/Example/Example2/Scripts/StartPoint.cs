﻿#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA || UNITY_WEBGL
using Net.Client;
using Net.Component;
using Net.UnityComponent;
using UnityEngine;
namespace Example2
{
    public class StartPoint : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            var prefab = (GameObject)Resources.Load("Prefabs/player");
            var offset = new Vector3(Random.Range(-20, 20), 0, Random.Range(-20, 20));
            var player1 = Instantiate(prefab, transform.position + offset, transform.rotation);
            var pc = player1.AddComponent<PlayerControl>();
            FindObjectOfType<InputJump>().pc = pc;
            FindObjectOfType<InputFire>().pc = pc;
            Camera.main.GetComponent<ARPGcamera>().target = player1.transform;
            var p = player1.GetComponent<Player>();
            GameManager.I.players.Add(p);
            p.id = ClientBase.Instance.UID;
            p.IsLocal = true;
            p.GetComponent<NetworkObject>().Identity = ClientBase.Instance.UID;
            InputJoystick.OnJoystickMoving += (dir) =>
            {
                pc.moveDirection = new Vector3(dir.x, 0, dir.y);
            };
            InputJoystick.OnJoystickMoveEnd += () =>
            {
                pc.moveDirection = Vector3.zero;
            };
        }
    }
}
#endif