#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA || UNITY_WEBGL
namespace AOIExample 
{
    using Net.Client;
    using Net.Component;
    using Net.Share;
    using Net.UnityComponent;
    using UnityEngine;

    public class SceneManager : NetworkSceneManager
    {
        public GameObject player;

        public override void OnConnected()
        {
            base.OnConnected();
            var player1 = Instantiate(player, new Vector3(Random.Range(-20, 20), 1, Random.Range(-20, 20)), Quaternion.identity);
            player1.AddComponent<PlayerControl>();
            player1.name = ClientBase.Instance.Identify;
            player1.GetComponent<AOIBody>().IsLocal = true;
            player1.GetComponent<PlayerControl>().moveSpeed = 20f;
            FindObjectOfType<ARPGcamera>().target = player1.transform;
        }
        public override void OnNetworkObjectCreate(Operation opt, NetworkObject identity)
        {
            var rigidbody = identity.GetComponent<Rigidbody>();
            Destroy(rigidbody);
        }
        public override void OnPlayerExit(Operation opt)
        {
            if (opt.identity == ClientBase.Instance.UID)//服务器延迟检测连接断开时,网络场景会将移除cmd插入同步队列, 当你再次进入如果uid是上次的uid, 则会发送下来,会删除刚生成的玩家对象
                return;
            base.OnPlayerExit(opt);
        }
    }
}
#endif