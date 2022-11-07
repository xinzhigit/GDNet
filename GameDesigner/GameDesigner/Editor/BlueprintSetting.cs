#if UNITY_EDITOR
using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEditor;

public class BlueprintSetting : ScriptableObject
{
    static private BlueprintSetting _instance = null;
    static public BlueprintSetting Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<BlueprintSetting>("BlueprintSetting");
                if (_instance == null)
                    throw new Exception("BlueprintSetting预制体脚本丢失!");
                PluginSettings.InitPlugin();
            }
            return _instance;
        }
    }

    static public bool IsSubclassOf(Type type, Type Typeof)
    {
        if (type == null | Typeof == null)
            return false;
        if (type.IsSubclassOf(Typeof) | type == Typeof)
            return true;
        return false;
    }

    static void SetPropertyValue(GUIStyle style)
    {
        style.normal.background = Resources.Load<Texture2D>("RadioButton_Off");
        style.normal.textColor = Color.white;
        style.fontStyle = FontStyle.Bold;
        style.alignment = TextAnchor.MiddleCenter;
    }

    static void PropertyFor(object target, object setValue)
    {
        if (target == null)
            return;

        foreach (PropertyInfo property in target.GetType().GetProperties())
        {
            if (!property.CanWrite)
                continue;
            if (IsSubclassOf(property.PropertyType, typeof(UnityEngine.Object)) | property.PropertyType == typeof(string) | property.PropertyType == typeof(object) | property.PropertyType.IsValueType | property.PropertyType.IsEnum)
            {
                property.SetValue(target, property.GetValue(setValue, null), null);
            }
            else
            {
                PropertyFor(property.GetValue(target, null), property.GetValue(setValue, null));
            }
        }
    }

#if UNITY_EDITOR || DEBUG
    private GUIStyle GetNodeStyle(ref GUIStyle style, ref GUIStyle style1, string styleName, Action<GUIStyle> action = null)
    {
        if (style1 == null)
            goto JUMP;
        if (style1.normal.background != null)
            return style1;
        if (style == null)
            goto JUMP;
        if (style.normal.background == null)
            goto JUMP;
        goto RET;
    JUMP: style = new GUIStyle(GUI.skin.GetStyle(styleName));
        style.normal.textColor = Color.white;
        style.alignment = TextAnchor.UpperCenter;
        action?.Invoke(style);
    RET: return style1 = style;
    }

    [Header("状态机皮肤")]
    [SerializeField]
    private Texture _stateMachineImage = null;
    public Texture stateMachineImage
    {
        get
        {
            if (_stateMachineImage == null)
            {
                return _stateMachineImage = Resources.Load<Texture>("stateStyle");
            }
            return _stateMachineImage;
        }
    }

    [Header("横向间隔条皮肤")]
    public string horSpaceStyleNames = "ButtonMid";
    [SerializeField]
    private GUIStyle horSpaceStyle = null; private GUIStyle horSpaceStyle1 = null;
    public GUIStyle HorSpaceStyle
    {
        get
        {
            return GetNodeStyle(ref horSpaceStyle, ref horSpaceStyle1, horSpaceStyleNames);
        }
    }

#endif
    public PluginLanguage language = PluginLanguage.Chinese;
    public string[] LANGUAGE = new string[120];
}

/// <summary>
/// 插件语言
/// </summary>
public enum PluginLanguage
{
    /// <summary>
    /// 英文
    /// </summary>
    English,
    /// <summary>
    /// 中文
    /// </summary>
    Chinese
}
#endif