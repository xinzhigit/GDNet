#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

public class PluginSettings : EditorWindow
{
    private PluginLanguage language;

    private void Awake()
    {
        language = BlueprintSetting.Instance.language;
        InitPlugin();
    }

    public static void InitPlugin()
    {
        if (BlueprintSetting.Instance.language == PluginLanguage.Chinese)
        {
            var obj = Resources.Load<TextAsset>("ChineseLanguage");
            var text = System.Text.Encoding.UTF8.GetString(obj.bytes);
            BlueprintSetting.Instance.LANGUAGE = text.Split(new string[] { "\r\n" }, 0);
        }
        else
        {
            var obj = Resources.Load<TextAsset>("EnglishLanguage");
            var text = System.Text.Encoding.UTF8.GetString(obj.bytes);
            BlueprintSetting.Instance.LANGUAGE = text.Split(new string[] { "\r\n" }, 0);
        }
    }

    [MenuItem("GameDesigner/PluginSettings")]
    static void Init()
    {
        var setting = GetWindow<PluginSettings>();
        setting.maxSize = new Vector2(300, 100);
        setting.Show();
    }

    void OnGUI()
    {
        if (language != BlueprintSetting.Instance.language)
        {
            language = BlueprintSetting.Instance.language;
            InitPlugin();
        }
        BlueprintSetting.Instance.language = (PluginLanguage)EditorGUILayout.EnumPopup(BlueprintSetting.Instance.LANGUAGE[83], BlueprintSetting.Instance.language);
    }
}
#endif