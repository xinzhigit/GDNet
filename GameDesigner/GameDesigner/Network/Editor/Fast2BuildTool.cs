#if UNITY_EDITOR
using System.IO;
using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class Fast2BuildTools2 : EditorWindow
{
    private List<FoldoutData> typeNames = new List<FoldoutData>();
    private string search = "", search1 = "";
    private string searchBind = "", searchBind1 = "";
    private DateTime searchTime;
    private TypeData[] types;
    private Vector2 scrollPosition;
    private Vector2 scrollPosition1;
    private string savePath, savePath1;
    private bool serField = true;
    private bool serProperty = true;
    private string typeEntry;
    private string typeEntry1;
    private string methodEntry;
    private string methodEntry1;
    private string selectType;

    [MenuItem("GameDesigner/Network/Fast2BuildTool-2")]
    static void ShowWindow()
    {
        var window = GetWindow<Fast2BuildTools2>("快速序列化2生成工具");
        window.Show();
    }

    private void OnEnable()
    {
        var types1 = new List<TypeData>();
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        Assembly assembly = null;
        foreach (var assemblie in assemblies)
        {
            if (assemblie.GetName().Name == "Assembly-CSharp")
            {
                assembly = assemblie;
                break;
            }
        }
        var types2 = assembly.GetTypes().Where(t => !t.IsAbstract & !t.IsInterface & !t.IsGenericType & !t.IsGenericType & !t.IsGenericTypeDefinition).ToArray();
        var types3 = typeof(Vector2).Assembly.GetTypes().Where(t => !t.IsAbstract & !t.IsInterface & !t.IsGenericType & !t.IsGenericType & !t.IsGenericTypeDefinition).ToArray();
        var typeslist = new List<Type>(types2);
        typeslist.AddRange(types3);
        foreach (var obj in typeslist)
        {
            var str = obj.FullName;
            types1.Add(new TypeData() { name = str, type = obj });
        }
        types = types1.ToArray();
        LoadData();
    }

    private void OnGUI()
    {
        search = EditorGUILayout.TextField("搜索绑定类型", search);
        searchBind = EditorGUILayout.TextField("搜索已绑定类型", searchBind);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("全部展开")) 
            foreach (var type1 in typeNames)
                type1.foldout = true;
        if (GUILayout.Button("全部收起"))
            foreach (var type1 in typeNames)
                type1.foldout = false;
        if (GUILayout.Button("全部字段更新"))
        {
            UpdateFields();
            SaveData();
            Debug.Log("全部字段已更新完成!");
        }
        if (GUILayout.Button("引用文件夹"))
        {
            var path = EditorUtility.OpenFolderPanel("选择文件夹路径", "", "");
            var files = Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var texts = File.ReadLines(file);
                var nameSpace = "";
                var typeName = "";
                foreach (var text in texts)
                {
                    if (text.Contains("namespace"))
                    {
                        nameSpace = text.Replace("namespace", "").Trim();
                        continue;
                    }
                    var index = 0;
                    var has = false;
                    if (text.Contains("class"))
                    {
                        index = text.IndexOf("class") + 6;
                        has = true;
                    }
                    if (text.Contains("struct"))
                    {
                        index = text.IndexOf("struct") + 7;
                        has = true;
                    }
                    if (has)
                    {
                        var end = text.Length - index;
                        var typeName1 = text.Substring(index, end);
                        var typeName2 = typeName1.Split(':');
                        typeName = typeName2[0].Trim();
                        string typeFull;
                        if (nameSpace == "")
                            typeFull = typeName;
                        else
                            typeFull = $"{nameSpace}.{typeName}";
                        foreach (var type1 in types)
                        {
                            if (type1.name != typeFull)
                                continue;
                            AddSerType(type1);
                            break;
                        }
                        typeName = "";
                    }
                }
                if (typeName.Length > 0) 
                {
                    var typeFull = $"{nameSpace}.{typeName}";
                    foreach (var type1 in types)
                    {
                        if (type1.name != typeFull)
                            continue;
                        AddSerType(type1);
                        break;
                    }
                }
            }
        }
        EditorGUILayout.EndHorizontal();
        if (typeNames.Count != 0)
        {
            scrollPosition1 = GUILayout.BeginScrollView(scrollPosition1, false, true, GUILayout.MaxHeight(position.height / 2));
            EditorGUI.BeginChangeCheck();
            foreach (var type1 in typeNames)
            {
                var rect = EditorGUILayout.GetControlRect();
                var color = GUI.color;
                if (type1.name == selectType)
                    GUI.color = Color.green;
                type1.foldout = EditorGUI.Foldout(new Rect(rect.position, rect.size - new Vector2(50, 0)), type1.foldout, type1.name, true);
                GUI.color = color;
                if (type1.foldout)
                {
                    EditorGUI.indentLevel = 1;
                    for (int i = 0; i < type1.fields.Count; i++)
                    {
                        type1.fields[i].serialize = EditorGUILayout.Toggle(type1.fields[i].name, type1.fields[i].serialize);
                    }
                    EditorGUI.indentLevel = 0;
                }
                if (GUI.Button(new Rect(rect.position + new Vector2(position.width - 50, 0), new Vector2(20, rect.height)), "x"))
                {
                    typeNames.Remove(type1);
                    SaveData();
                    return;
                }
                if (rect.Contains(Event.current.mousePosition) & Event.current.button == 1)
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("全部勾上"), false, ()=>
                    {
                        type1.fields.ForEach(item => item.serialize = true);
                    }); 
                    menu.AddItem(new GUIContent("全部取消"), false, () =>
                    {
                        type1.fields.ForEach(item => item.serialize = false);
                    });
                    menu.AddItem(new GUIContent("更新字段"), false, () =>
                    {
                        UpdateField(type1);
                        SaveData();
                    });
                    menu.AddItem(new GUIContent("移除"), false, () =>
                    {
                        typeNames.Remove(type1);
                        SaveData();
                    });
                    menu.ShowAsContext();
                }
            }
            if (EditorGUI.EndChangeCheck())
                SaveData();
            GUILayout.EndScrollView();
        }
        if (search != search1)
        {
            search1 = search;
            searchTime = DateTime.Now.AddMilliseconds(20);
        }
        if (DateTime.Now > searchTime & search.Length > 0)
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true, GUILayout.MaxHeight(position.height / 2));
            foreach (var type1 in types)
            {
                if (!type1.name.ToLower().Contains(search.ToLower()))
                    continue;
                if (GUILayout.Button(type1.name))
                {
                    AddSerType(type1);
                    return;
                }
            }
            GUILayout.EndScrollView();
        }

        if (searchBind != searchBind1)
        {
            searchBind1 = searchBind;
            searchTime = DateTime.Now.AddMilliseconds(20);
            if (searchBind.Length == 0)
                selectType = "";
        }
        if (DateTime.Now > searchTime & searchBind.Length > 0)
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true, GUILayout.MaxHeight(position.height / 6));
            foreach (var type1 in typeNames)
            {
                if (!type1.name.ToLower().Contains(searchBind.ToLower()))
                    continue;
                if (GUILayout.Button(type1.name))
                {
                    var scrollPosition2 = new Vector2();
                    for (int i = 0; i < typeNames.Count; i++)
                    {
                        if (typeNames[i].name == type1.name) 
                        {
                            scrollPosition1 = scrollPosition2;
                            selectType = type1.name;
                            break;
                        }
                        scrollPosition2.y += 20f;//20是foldout标签
                        if (typeNames[i].foldout)
                            scrollPosition2.y += typeNames[i].fields.Count * 20f;
                    }
                    break;
                }
            }
            GUILayout.EndScrollView();
        }

        serField = EditorGUILayout.Toggle("序列化字段:", serField);
        serProperty = EditorGUILayout.Toggle("序列化属性:", serProperty);
        GUILayout.BeginHorizontal();
        typeEntry = EditorGUILayout.TextField("收集类名:", typeEntry);
        methodEntry = EditorGUILayout.TextField("收集方法:", methodEntry);
        if (typeEntry != typeEntry1)
        {
            typeEntry1 = typeEntry;
            SaveData();
        }
        if (methodEntry != methodEntry1)
        {
            methodEntry1 = methodEntry;
            SaveData();
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("保存路径:", savePath);
        if (GUILayout.Button("选择路径", GUILayout.Width(100)))
        {
            savePath = EditorUtility.OpenFolderPanel("保存路径", "", "");
            SaveData();
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("保存路径1:", savePath1);
        if (GUILayout.Button("选择路径1", GUILayout.Width(100)))
        {
            savePath1 = EditorUtility.OpenFolderPanel("保存路径", "", "");
            SaveData();
        }
        GUILayout.EndHorizontal();
        if (GUILayout.Button("生成绑定代码", GUILayout.Height(30)))
        {
            if (string.IsNullOrEmpty(savePath))
            {
                EditorUtility.DisplayDialog("提示", "请选择生成脚本路径!", "确定");
                return;
            }
            List<Type> types = new List<Type>();
            foreach (var type1 in typeNames)
            {
                Type type = Net.Serialize.NetConvertOld.GetType(type1.name);
                if (type == null)
                {
                    Debug.Log($"类型:{type1.name}已不存在!");
                    continue;
                }
                Fast2BuildMethod.Build(type, savePath, serField, serProperty, type1.fields.ConvertAll((item)=> !item.serialize ? item.name : ""));
                Fast2BuildMethod.BuildArray(type, savePath);
                Fast2BuildMethod.BuildGeneric(type, savePath);
                if (!string.IsNullOrEmpty(savePath1)) 
                {
                    Fast2BuildMethod.Build(type, savePath1, serField, serProperty, type1.fields.ConvertAll((item) => !item.serialize ? item.name : ""));
                    Fast2BuildMethod.BuildArray(type, savePath1);
                    Fast2BuildMethod.BuildGeneric(type, savePath1);
                }
                types.Add(type);
            }
            if (!string.IsNullOrEmpty(typeEntry)) 
            {
                var types1 = (Type[])Net.Serialize.NetConvertOld.GetType(typeEntry).GetMethod(methodEntry).Invoke(null, null);
                foreach (var type in types1)
                {
                    if (type.IsGenericType)
                    {
                        var args = type.GenericTypeArguments;
                        if (args.Length == 1)
                        {
                            //TODO
                        }
                        else if (args.Length == 2)
                        {
                            var text = Fast2BuildMethod.BuildDictionary(type, out var className1);
                            File.WriteAllText(savePath + $"//{className1}.cs", text);
                        }
                    }
                    else 
                    {
                        Fast2BuildMethod.Build(type, savePath, serField, serProperty, new List<string>());
                        Fast2BuildMethod.BuildArray(type, savePath);
                        Fast2BuildMethod.BuildGeneric(type, savePath);
                        if (!string.IsNullOrEmpty(savePath1))
                        {
                            Fast2BuildMethod.Build(type, savePath1, serField, serProperty, new List<string>());
                            Fast2BuildMethod.BuildArray(type, savePath1);
                            Fast2BuildMethod.BuildGeneric(type, savePath1);
                        }
                    }
                    types.Add(type);
                }
            }
            Fast2BuildMethod.BuildBindingType(types, savePath);
            Debug.Log("生成完成.");
            AssetDatabase.Refresh();
        }
        EditorGUILayout.HelpBox("使用时在Start方法初始化: Net.Serialize.NetConvertFast2.AddSerializeType3s(Binding.BindingType.TYPES);", MessageType.Info);
    }

    private void AddSerType(TypeData type1)
    {
        if (typeNames.Find(item => item.name == type1.name) == null)
        {
            var fields = type1.type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            var properties = type1.type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var fields1 = new List<FieldData>();
            foreach (var item in fields)
            {
                if (item.GetCustomAttribute<Net.Serialize.NonSerialized>() != null)
                    continue;
                fields1.Add(new FieldData() { name = item.Name, serialize = true });
            }
            foreach (var item in properties)
            {
                if (!item.CanRead | !item.CanWrite)
                    continue;
                if (item.GetIndexParameters().Length > 0)
                    continue;
                if (item.GetCustomAttribute<Net.Serialize.NonSerialized>() != null)
                    continue;
                fields1.Add(new FieldData() { name = item.Name, serialize = true });
            }
            typeNames.Add(new FoldoutData() { name = type1.name, fields = fields1, foldout = false });
        }
        SaveData();
    }

    private void UpdateFields() 
    {
        foreach (var fd in typeNames) 
        {
            UpdateField(fd);
        }
        SaveData();
    }

    private void UpdateField(FoldoutData fd)
    {
        Type type = null;
        foreach (var type2 in types)
        {
            if (fd.name == type2.name)
            {
                type = type2.type;
                break;
            }
        }
        if (type == null)
        {
            Debug.Log(fd.name + "类型为空!");
            return;
        }
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var fields1 = new List<FieldData>();
        foreach (var item in fields)
        {
            if (item.GetCustomAttribute<Net.Serialize.NonSerialized>() != null)
                continue;
            fields1.Add(new FieldData() { name = item.Name, serialize = true });
        }
        foreach (var item in properties)
        {
            if (!item.CanRead | !item.CanWrite)
                continue;
            if (item.GetIndexParameters().Length > 0)
                continue;
            if (item.GetCustomAttribute<Net.Serialize.NonSerialized>() != null)
                continue;
            fields1.Add(new FieldData() { name = item.Name, serialize = true });
        }
        foreach (var item in fields1)
        {
            if (fd.fields.Find(item1 => item1.name == item.name, out var fd1))
            {
                item.serialize = fd1.serialize;
            }
        }
        fd.fields = fields1;
    }

    void LoadData() 
    {
        var path = Application.dataPath.Replace("Assets", "") + "data2.txt";
        if (File.Exists(path))
        {
            var jsonStr = File.ReadAllText(path);
            var data = Newtonsoft_X.Json.JsonConvert.DeserializeObject<Data>(jsonStr);
            typeNames = data.typeNames;
            savePath = data.savepath; 
            savePath1 = data.savepath1;
            typeEntry = data.typeEntry;
            methodEntry = data.methodEntry;
        }
    }

    void SaveData()
    {
        Data data = new Data() { 
            typeNames = typeNames,
            savepath = savePath,
            savepath1 = savePath1,
            typeEntry = typeEntry,
            methodEntry = methodEntry,
        };
        var jsonstr = Newtonsoft_X.Json.JsonConvert.SerializeObject(data);
        var path = Application.dataPath.Replace("Assets", "") + "data2.txt";
        File.WriteAllText(path, jsonstr);
    }

    internal class FoldoutData 
    {
        public string name;
        public bool foldout;
        public List<FieldData> fields = new List<FieldData>();
    }

    internal class FieldData 
    {
        public string name;
        public bool serialize;
        public int select;
    }

    internal class TypeData 
    {
        public string name;
        public Type type;
    }

    internal class Data
    {
        public string savepath, savepath1;
        public List<FoldoutData> typeNames;
        public string typeEntry;
        public string methodEntry;
    }
}
#endif