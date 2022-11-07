namespace Example2 
{
    using UnityEngine;

    //�������GameInit�� ����Ҫ�����ȸ��´�����
    public class GameEntry : MonoBehaviour
    {
        private static GameEntry instance;
        [Header("�������GameInit������ʹ���ȸ��´���!")]
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