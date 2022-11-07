#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA || UNITY_WEBGL
namespace Net.Example
{
    using global::System.Threading.Tasks;
    using Net.UnityComponent;
    using UnityEngine;

    public class Spawn : MonoBehaviour
    {
        public int num = 1000;
        public int currnum;
        public GameObject @object;

        // Start is called before the first frame update
        async void Start()
        {
            while (!NetworkObject.IsInitIdentity)
            {
                await Task.Yield();
            }
            OnConnected();
        }

        private void OnConnected()
        {
            InvokeRepeating(nameof(Ins), 0.1f, 0.1f);
        }

        void Ins()
        {
            if (currnum >= num)
                return;
            currnum++;
            Instantiate(@object, transform.position, transform.rotation);
            transform.Rotate(transform.forward, 30f);
        }
    }
}
#endif