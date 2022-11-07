using System.Collections.Generic;
using System;
#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA || UNITY_WEBGL
using UnityEngine;

[CreateAssetMenu(menuName = "Create InvokeHelperConfig")]
public class InvokeHelperConfigObject : ScriptableObject
{
    public InvokeHelperConfig Config = new InvokeHelperConfig();
}
#elif SERVICE
public class Header : Attribute 
{
    public Header(string text) { }
}
#endif
[Serializable]
public class InvokeHelperConfig
{
    [Header("true:���ű��������ã��������SyncVar��Rpc�Ĵ��룬false:�����ü��")]
    public bool onReloadInvoke;
    [Header("true:��unity�����ֶ�ͬ�����������ɵĴ��� false:����ʱ��̬�����ֶ�ͬ��������")]
    public bool syncVarClientEnable;
    [Header("true:��server�����ֶ�ͬ�����������ɵĴ��� false:����ʱ��̬�����ֶ�ͬ��������")]
    public bool syncVarServerEnable;
    [Header("���ɵĽű����·��(unity)")]
    public string savePath;
    [Header("�ռ�����·��(unity)")]
    public List<string> dllPaths = new List<string>();
    [Header("Rpc����")]
#if UNITY_2020_1_OR_NEWER && UNITY_EDITOR
    [NonReorderable]
#endif
    public List<InvokeHelperConfigData> rpcConfig = new List<InvokeHelperConfigData>();
    public bool foldout;
    public int rpcConfigSize;
}

[Serializable]
public class InvokeHelperConfigData 
{
    public string name;
    [Header("VS��Ŀ�ļ�·��")]
    public string csprojPath;
    [Header("���ɵĽű����·��")]
    public string savePath;
    [Header("��ȡ��������·��")]
    public string readConfigPath;
    [Header("�ռ�����·��")]
    public List<string> dllPaths = new List<string>();
    public bool foldout;
}
