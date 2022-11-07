﻿#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA || UNITY_WEBGL
namespace Net.Component
{
    public enum SerializeAdapterType
    {
        Default,//默认序列化, protobuff + json
        PB_JSON_FAST,//快速序列化 protobuff + json
        Binary,//快速序列化 需要注册远程类型
        Binary2,//极速序列化 Binary + Binary2 需要生成序列化类型, 菜单GameDesigner/Netowrk/Fast2BuildTools
        Binary3//极速序列化 需要生成序列化类型, 菜单GameDesigner/Netowrk/Fast2BuildTools
    }

    public class SerializeAdapter : SingleCase<SerializeAdapter>
    {
        public SerializeAdapterType type;
        public bool isEncrypt = false;//数据加密?

        void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            Init();
        }

        public void Init()
        {
            var cm = GetComponent<ClientManager>();
            switch (type) {
                case SerializeAdapterType.Default:
                    break;
                case SerializeAdapterType.PB_JSON_FAST:
                    cm.client.AddAdapter(new Adapter.SerializeFastAdapter() { isEncrypt = isEncrypt });
                    break;
                case SerializeAdapterType.Binary:
                    cm.client.AddAdapter(new Adapter.SerializeAdapter() { isEncrypt = isEncrypt });
                    break;
                case SerializeAdapterType.Binary2:
                    cm.client.AddAdapter(new Adapter.SerializeAdapter2() { isEncrypt = isEncrypt });
                    break;
                case SerializeAdapterType.Binary3:
                    cm.client.AddAdapter(new Adapter.SerializeAdapter3() { isEncrypt = isEncrypt });
                    break;
            }
        }
    }
}
#endif