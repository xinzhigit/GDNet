#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA || UNITY_WEBGL
namespace Example2
{
    using UnityEngine;
    using UnityEngine.UI;

    public class InputFire : MonoBehaviour
    {
        internal PlayerControl pc;

        // Start is called before the first frame update
        void Start()
        {
            GetComponent<Button>().onClick.AddListener(() => {
                pc.fire = true;
            });
        }
    }
}
#endif