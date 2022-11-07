#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA || UNITY_WEBGL
using Net.Client;
using Net.Component;
using Net.Share;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UITest : SingleCase<UITest>
{
    public InputField cri;
    public Button cr, jr, er, rb, eb;
    public Text text;

    // Start is called before the first frame update
    async void Start()
    {
        cr.onClick.AddListener(()=> {
            ClientBase.Instance.SendRT("CreateRoom", cri.text);
        });
        jr.onClick.AddListener(() => {
            ClientBase.Instance.SendRT("JoinRoom", cri.text);
        });
        er.onClick.AddListener(() => {
            ClientBase.Instance.SendRT("ExitRoom");
        });
        rb.onClick.AddListener(() => {
            ClientBase.Instance.SendRT("StartBattle");
        });
        eb.onClick.AddListener(() => {
            ClientBase.Instance.SendRT("ExitBattle");
        });
        while (ClientBase.Instance == null)
        {
            await Task.Yield();
        }
        while (!ClientBase.Instance.Connected)
        {
            await Task.Yield();
        }
        ClientBase.Instance.AddRpcHandle(this);
    }

    private void Update()
    {
        var i = LockStep.Client.Game.I;
        text.text = $"网络帧:{i.frame2}/秒 延迟:{i.delay}/秒";
    }

    [Rpc]
    void CreateRoomCallback(string str) 
    {
        Debug.Log(str);
    }

    [Rpc]
    void JoinRoomCallback(string str)
    {
        Debug.Log(str);
    }

    [Rpc]
    void ExitRoomCallback(string str)
    {
        Debug.Log(str);
    }

    [Rpc]
    void StartGameSync()
    {
        Debug.Log("开始帧同步!");
    }

    private void OnDestroy()
    {
        ClientManager.Instance.client.RemoveRpc(this);
    }
}
#endif