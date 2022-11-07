#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA || UNITY_WEBGL
namespace Net.Example
{
    using global::System.Threading.Tasks;
    using Net.UnityComponent;
    using UnityEngine;

    public class Spawn1 : MonoBehaviour
    {
        public GameObject spawnPrefab;
        public int spawnAmount = 1000;
        public float interleave = 1;

        async void Start()
        {
            while (!NetworkObject.IsInitIdentity)
            {
                await Task.Yield();
            }
            OnConnected();
        }

        private async void OnConnected()
        {
            await Task.Delay(1000);
            //服务器的记录uid从10000开始,所以这里uid-10000=0(transform同步组件唯一id), 这里 * 5000是每个客户端都可以实例化5000个transform同步组件
            //并且保证唯一id都是正确的,如果一个客户端实例化超过5000个, 就会和uid=10001的玩家transform同步物体唯一id碰撞, 会出现弹跳问题
            float sqrt = Mathf.Sqrt(spawnAmount);
            float offset = -sqrt / 2 * interleave;
            int spawned = 0;
            for (int spawnX = 0; spawnX < sqrt; ++spawnX)
            {
                for (int spawnZ = 0; spawnZ < sqrt; ++spawnZ)
                {
                    if (spawned < spawnAmount)
                    {
                        GameObject go = Instantiate(spawnPrefab);
                        float x = offset + spawnX * interleave;
                        float z = offset + spawnZ * interleave;
                        go.transform.position = new Vector3(x, 0, z);
                        ++spawned;
                    }
                }
            }
        }
    }
}
#endif