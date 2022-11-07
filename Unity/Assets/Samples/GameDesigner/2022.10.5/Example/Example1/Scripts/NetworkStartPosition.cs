#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA || UNITY_WEBGL
using Net.Client;
using Net.Component;
using Net.Share;
using Net.System;
using Net.UnityComponent;
using System.Threading.Tasks;
using UnityEngine;
namespace Example1
{
    public class NetworkStartPosition : MonoBehaviour
    {
        public GameObject playerPrefab;
        public Vector2 offsetX = new Vector2(-20, 20);
        public Vector2 offsetZ = new Vector2(-20, 20);
        private uint delay, frame;

        // Start is called before the first frame update
        async void Start()
        {
            while (!NetworkObject.IsInitIdentity)
            {
                await Task.Yield();
            }
            OnConnectedHandle();
            ClientBase.Instance.OnPingCallback += PingCall;
            ThreadManager.Invoke(1f, ()=> {
                ClientBase.Instance.Ping();
                return true;
            });
            ClientBase.Instance.OnOperationSync += OnOperationSync;
        }

        private void PingCall(uint ms) 
        {
            delay = ms;
        }

        private void OnOperationSync(OperationList list) 
        {
            frame = list.frame;
        }

        private void OnConnectedHandle()
        {
            var offset = new Vector3(Random.Range(offsetX.x, offsetX.y), 0, Random.Range(offsetZ.x, offsetZ.y));
            var player1 = Instantiate(playerPrefab, transform.position + offset, transform.rotation);
            player1.GetComponent<NetworkObject>().Identity = ClientBase.Instance.UID;
            player1.GetComponent<PlayerController>().isLocalPlayer = true;
            Camera.main.GetComponent<ARPGcamera>().target = player1.transform;
        }

        private void OnGUI()
        {
            var color = GUI.color;
            GUI.color = Color.green;
            GUI.Label(new Rect(10, 10, 200, 20), $"同步帧:{frame} 延迟:{delay}ms");
            GUI.color = color;
        }

        private void OnDestroy()
        {
            if (ClientBase.Instance == null)
                return;
            ClientBase.Instance.OnOperationSync -= OnOperationSync;
            ClientBase.Instance.OnPingCallback -= PingCall;
        }
    }
}
#endif