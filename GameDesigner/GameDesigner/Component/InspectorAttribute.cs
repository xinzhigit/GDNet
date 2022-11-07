#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA || UNITY_WEBGL
using UnityEngine;

/// <summary>
/// 只显示不能修改的属性
/// </summary>
public class DisplayOnly : PropertyAttribute 
{
    public string text;
    public DisplayOnly() { }
    public DisplayOnly(string text) 
    {
        this.text = text;
    }
}

///<summary>
///定义多选属性
///</summary>
public class EnumFlags : PropertyAttribute { }

#endif