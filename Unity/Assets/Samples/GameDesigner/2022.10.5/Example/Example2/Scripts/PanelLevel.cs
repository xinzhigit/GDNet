#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA || UNITY_WEBGL
using Net.Component;
using UnityEngine;

namespace Example2
{
    public class PanelLevel : SingleCase<PanelLevel>
    {
        public Transform[] levels = new Transform[3];
    }
}
#endif