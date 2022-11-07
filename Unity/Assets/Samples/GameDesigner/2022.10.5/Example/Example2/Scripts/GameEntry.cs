namespace Example2 
{
    using UnityEngine;

    //此类代替GameInit， 不需要进入热更新代码了
    public class GameEntry : MonoBehaviour
    {
        private static GameEntry instance;
        [Header("此类代替GameInit，不再使用热更新代码!")]
        public string text = "";

        // Start is called before the first frame update
        void Start()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            Hotfix.GameEntry.Init();
            DontDestroyOnLoad(gameObject);
        }

        // Update is called once per frame
        void Update()
        {
            Hotfix.GameEntry.Update();
        }
    }
}