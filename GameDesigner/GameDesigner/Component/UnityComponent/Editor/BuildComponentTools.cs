#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using static Fast2BuildTools2;
using Object = UnityEngine.Object;

public class BuildComponentTools : EditorWindow
{
    private string savePath, savePath1;
    private Object component;
    private Object oldComponent;
    private FoldoutData foldout;
    private Vector2 scrollPosition1;

    [MenuItem("GameDesigner/Network/BuildComponentTools")]
    public static void Init()
    {
        GetWindow<BuildComponentTools>("BuildComponentTools", true);
    }
    private void OnEnable()
    {
        LoadData();
    }
    void OnGUI()
    {
        component = EditorGUILayout.ObjectField("组件", component, typeof(Object), true);
        if (component != oldComponent) 
        {
            oldComponent = component;
            if (component != null)
            {
                var type = component.GetType();
                //var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
                var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public);
                var fields1 = new List<FieldData>();
                //foreach (var item in fields)
                //{
                //    fields1.Add(new FieldData() { name = item.Name, serialize = true });
                //}
                foreach (var item in properties)
                {
                    if (!item.CanRead | !item.CanWrite)
                        continue;
                    if (item.GetIndexParameters().Length > 0)
                        continue;
                    if (item.GetCustomAttribute<ObsoleteAttribute>() != null)
                        continue;
                    var ptype = item.PropertyType;
                    var code = Type.GetTypeCode(ptype);
                    if (code == TypeCode.Object & ptype != typeof(Vector2) & ptype != typeof(Vector3) & ptype != typeof(Vector4) &
                        ptype != typeof(Rect) & ptype != typeof(Quaternion) & ptype != typeof(Color)
                        & ptype != typeof(Color32) & ptype != typeof(Net.Vector2) & ptype != typeof(Net.Vector3)
                        & ptype != typeof(Net.Vector4) & ptype != typeof(Net.Rect) & ptype != typeof(Net.Quaternion)
                        & ptype != typeof(Net.Color) & ptype != typeof(Net.Color32) & ptype != typeof(Object) & !ptype.IsSubclassOf(typeof(Object)))
                        continue;
                    fields1.Add(new FieldData() { name = item.Name });
                }
                for (int i = 0; i < methods.Length; i++)
                {
                    var met = methods[i];
                    if (met.MethodImplementationFlags == MethodImplAttributes.InternalCall | met.Name.Contains("get_") | met.Name.Contains("set_") |
                        met.GetCustomAttribute<ObsoleteAttribute>() != null | met.IsGenericMethod)
                        continue;
                    var pars = met.GetParameters();
                    bool not = false;
                    foreach (var item in pars)
                    {
                        var ptype = item.ParameterType;
                        var code = Type.GetTypeCode(ptype);
                        if (code == TypeCode.Object & ptype != typeof(Vector2) & ptype != typeof(Vector3) & ptype != typeof(Vector4) &
                            ptype != typeof(Rect) & ptype != typeof(Quaternion) & ptype != typeof(Color)
                            & ptype != typeof(Color32) & ptype != typeof(Net.Vector2) & ptype != typeof(Net.Vector3)
                            & ptype != typeof(Net.Vector4) & ptype != typeof(Net.Rect) & ptype != typeof(Net.Quaternion)
                            & ptype != typeof(Net.Color) & ptype != typeof(Net.Color32) //& ptype != typeof(Object) & !ptype.IsSubclassOf(typeof(Object))
                            )
                        {
                            not = true;
                            break;
                        }
                    }
                    if (not)
                        continue;
                    fields1.Add(new FieldData() { name = met.ToString() });
                }
                foldout = new FoldoutData() { name = type.Name, fields = fields1, foldout = true };
            }
        }
        if (foldout != null)
        {
            scrollPosition1 = GUILayout.BeginScrollView(scrollPosition1, false, true);
            var rect = EditorGUILayout.GetControlRect();
            foldout.foldout = EditorGUI.Foldout(rect, foldout.foldout, foldout.name + "", true);
            if (foldout.foldout)
            {
                EditorGUI.indentLevel = 1;
                for (int i = 0; i < foldout.fields.Count; i++)
                {
                    var rect1 = EditorGUILayout.GetControlRect();
                    rect1.x += 20;
                    var width = rect1.width;
                    rect1.width = 180;
                    foldout.fields[i].select = GUI.Toolbar(rect1, foldout.fields[i].select, new string[] { "同步调用", "本地调用", "忽略" });
                    rect1.x += 200;
                    rect1.width = width - 200;
                    GUIStyle titleStyle2 = new GUIStyle();
                    switch (foldout.fields[i].select)
                    {
                        case 0:
                            titleStyle2.normal.textColor = Color.white;
                            break;
                        case 1:
                            titleStyle2.normal.textColor = Color.green;
                            break;
                        case 2:
                            titleStyle2.normal.textColor = Color.red;
                            break;
                    }
                    GUI.Label(rect1, foldout.fields[i].name, titleStyle2);
                    
                }
                EditorGUI.indentLevel = 0;
            }
            if (rect.Contains(Event.current.mousePosition) & Event.current.button == 1)
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("全部同步调用勾上"), false, () =>
                {
                    foldout.fields.ForEach(item => item.select = 0);
                }); 
                menu.AddItem(new GUIContent("全部本地调用勾上"), false, () =>
                {
                    foldout.fields.ForEach(item => item.select = 1);
                });
                menu.AddItem(new GUIContent("智能本地调用勾上"), false, () =>
                {
                    foldout.fields.ForEach(item => {
                        if (item.name.Contains("Get") | item.name.Contains("Is"))
                            item.select = 1;
                    });
                });
                menu.AddItem(new GUIContent("全部取消"), false, () =>
                {
                    foldout.fields.ForEach(item => item.select = 2);
                });
                menu.ShowAsContext();
            }
            GUILayout.EndScrollView();
        }
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("保存路径:", savePath);
        if (GUILayout.Button("选择路径", GUILayout.Width(100)))
        {
            savePath = EditorUtility.OpenFolderPanel("保存路径", "", "");
            SaveData();
        }
        GUILayout.EndHorizontal();
        if (GUILayout.Button("生成同步组件脚本", GUILayout.Height(40)))
        {
            if (string.IsNullOrEmpty(savePath))
            {
                EditorUtility.DisplayDialog("提示", "请选择生成脚本路径!", "确定");
                return;
            }
            if (component == null)
            {
                EditorUtility.DisplayDialog("提示", "请选择unity组件!", "确定");
                return;
            }
            var type = component.GetType();
            var str = BuildNew(type, foldout.fields.ConvertAll((item) => item.select == 2 ? item.name : ""), foldout.fields.ConvertAll((item) => item.select == 1 ? item.name : ""));
            File.WriteAllText(savePath + $"//Network{type.Name}.cs", str.ToString());
            Debug.Log("生成脚本成功!"); 
            AssetDatabase.Refresh();
        }
    }

    static StringBuilder BuildNew(Type type, List<string> ignores, List<string> immediatelys) 
    {
        var templateCode = @"#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA || UNITY_WEBGL
using Net.Client;
using Net.Share;
using Net.Component;
using Net.UnityComponent;
using UnityEngine;
using Net.System;
using static Net.Serialize.NetConvertFast2;

namespace BuildComponent
{
    /// <summary>
    /// {TypeName1}同步组件, 此代码由BuildComponentTools工具生成, 如果同步发生相互影响的字段或属性, 请自行检查处理一下!
    /// </summary>
    [RequireComponent(typeof({TypeName}))]
    public class Network{TypeName1} : NetworkBehaviour
    {
        private {TypeName} self;
        public bool autoCheck;
        private object[] fields;
		private int[] eventsId;
		
        public void Awake()
        {
            self = GetComponent<{TypeName}>();
[Split]
			fields = new object[{FieldSize}];
			eventsId = new int[{FieldSize}];
[Split]
            fields[{FieldIndex}] = self.{TypeFieldName};
[Split]
        }

[Split]
        public {PropertyType} {TypeFieldName}
        {
            get
            {
                return self.{TypeFieldName};
            }
            set
            {
                if (value.Equals({FieldName}))
                    return;
                {FieldName} = value;
                self.{TypeFieldName} = value;
                ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = Index,
                    index2 = {FieldIndex},
                    buffer = SerializeObject(value).ToArray(true),
                    uid = ClientBase.Instance.UID
                });
            }
        }
[Split]
        public override void OnPropertyAutoCheck()
        {
            if (!autoCheck)
                return;
[Split]
            {TypeFieldName} = {TypeFieldName};
[Split]
        }
[Split]
        public void {FuncName}({ParsString} bool always = false, int executeNumber = 0, float time = 0)
        {
            if ({Condition} !always) return;
			{SetPars}
            var buffer = SerializeModel(new RPCModel() { pars = new object[] { {Params} } });
            ClientBase.Instance.AddOperation(new Operation(Command.BuildComponent, netObj.Identity)
            {
                index = netObj.registerObjectIndex,
                index1 = Index,
                index2 = {FieldIndex},
                buffer = buffer
            });
            if (executeNumber > 0)
            {
                ThreadManager.Event.RemoveEvent(eventsId[{EIDIndex}]);
                eventsId[{EIDIndex}] = ThreadManager.Event.AddEvent(time, executeNumber, (obj)=> {
                    {FuncName}({Params} true, 0, 0);
                }, null);
            }
        }
[Split]
		public {ReturnType} {FuncName}({ParsString})
        {
            {Return}self.{FuncName}({Params});
        }
[Split]
        public override void OnNetworkOperationHandler(Operation opt)
        {
            switch (opt.index2)
            {
[Split]
                case {FieldIndex1}:
                    {
						if (opt.uid == ClientBase.Instance.UID)
							return;
						var {TypeFieldName} = DeserializeObject<{FieldType1}>(new Segment(opt.buffer, false));
						{FieldName} = {TypeFieldName};
						self.{TypeFieldName} = {TypeFieldName};
					}
                    break;
[Split]
                case {FieldIndex1}:
                    {
						self.{FuncName}();
					}
                    break;
[Split]
                case {FieldIndex1}:
                    {
						var segment = new Segment(opt.buffer, false);
						var data = DeserializeModel(segment);
						{SetPars}
						self.{FuncName}({Params});
					}
                    break;
[Split]
            }
        }
    }
}
" + @"#endif";

        var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
        var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public);
        StringBuilder sb = new StringBuilder();
        StringBuilder sb1 = new StringBuilder();
        StringBuilder sb2 = new StringBuilder();
        StringBuilder sb3 = new StringBuilder();
        StringBuilder sb4 = new StringBuilder();
        StringBuilder sb5 = new StringBuilder();

        templateCode = templateCode.Replace("{TypeName1}", $"{type.Name}");
        templateCode = templateCode.Replace("{TypeName}", $"{type.FullName}");

        var blockCodes = templateCode.Split(new string[] { "[Split]" }, 0);
        blockCodes[0] = blockCodes[0].Remove(blockCodes[0].Length - 2, 2);
        sb.Append(blockCodes[0]);

        int parNum = 0;
        for (int i = 0; i < properties.Length; i++)
        {
            var item = properties[i];
            if (ignores.Contains(item.Name))
                continue;
            if (!item.CanRead | !item.CanWrite | item.GetCustomAttribute<ObsoleteAttribute>() != null)
                continue;
            var ptype = item.PropertyType;
            var code = Type.GetTypeCode(ptype);
            if (code == TypeCode.Object & ptype != typeof(Vector2) & ptype != typeof(Vector3) & ptype != typeof(Vector4) &
                ptype != typeof(Rect) & ptype != typeof(Quaternion) & ptype != typeof(Color)
                & ptype != typeof(Color32) & ptype != typeof(Net.Vector2) & ptype != typeof(Net.Vector3)
                & ptype != typeof(Net.Vector4) & ptype != typeof(Net.Rect) & ptype != typeof(Net.Quaternion)
                & ptype != typeof(Net.Color) & ptype != typeof(Net.Color32) & ptype != typeof(UnityEngine.Object) & !ptype.IsSubclassOf(typeof(UnityEngine.Object))
                )
                continue;
            parNum++;
            var blockCode = blockCodes[2];
            blockCode = blockCode.Replace("{FieldIndex}", $"{parNum}");
            blockCode = blockCode.Replace("{TypeFieldName}", $"{item.Name}");
            blockCode = blockCode.Remove(blockCode.Length - 2, 2);
            sb1.Append(blockCode);

            blockCode = blockCodes[4];
            blockCode = blockCode.Replace("{PropertyType}", $"{item.PropertyType.FullName}");
            blockCode = blockCode.Replace("{TypeFieldName}", $"{item.Name}");
            blockCode = blockCode.Replace("{FieldName}", $"fields[{parNum}]");
            blockCode = blockCode.Replace("{FieldIndex}", $"{parNum}");
            blockCode = blockCode.Remove(blockCode.Length - 2, 2);
            sb2.Append(blockCode);

            blockCode = blockCodes[6];
            blockCode = blockCode.Replace("{TypeFieldName}", $"{item.Name}");
            blockCode = blockCode.Remove(blockCode.Length - 2, 2);
            sb3.Append(blockCode);

            blockCode = blockCodes[11];
            blockCode = blockCode.Replace("{FieldIndex1}", $"{parNum}");
            blockCode = blockCode.Replace("{FieldName}", $"fields[{parNum}]");
            blockCode = blockCode.Replace("{FieldType1}", $"{item.PropertyType.FullName}");
            blockCode = blockCode.Replace("{TypeFieldName}", $"{item.Name}");
            blockCode = blockCode.Remove(blockCode.Length - 2, 2);
            sb5.Append(blockCode);
        }
        for (int i = 0; i < methods.Length; i++)
        {
            var met = methods[i];
            var metName = met.ToString();
            if (ignores.Contains(metName))
                continue;
            if (met.MethodImplementationFlags == MethodImplAttributes.InternalCall | met.Name.Contains("get_") | met.Name.Contains("set_") |
                met.GetCustomAttribute<ObsoleteAttribute>() != null | met.IsGenericMethod)
                continue;
            var pars = met.GetParameters();
            bool not = false;
            foreach (var item in pars)
            {
                var ptype = item.ParameterType;
                var code = Type.GetTypeCode(ptype);
                if (code == TypeCode.Object & ptype != typeof(Vector2) & ptype != typeof(Vector3) & ptype != typeof(Vector4) &
                    ptype != typeof(Rect) & ptype != typeof(Quaternion) & ptype != typeof(Color)
                    & ptype != typeof(Color32) & ptype != typeof(Net.Vector2) & ptype != typeof(Net.Vector3)
                    & ptype != typeof(Net.Vector4) & ptype != typeof(Net.Rect) & ptype != typeof(Net.Quaternion)
                    & ptype != typeof(Net.Color) & ptype != typeof(Net.Color32) //& ptype != typeof(Object) & !ptype.IsSubclassOf(typeof(Object))
                    )
                {
                    not = true;
                    break;
                }
            }
            if (not)
                continue;
            string blockCode;
            var parsStr = "";
            var conditionStr = "";
            var setValueStr = "";
            var paramsStr = "";
            var setValueStr1 = "";
            parNum++;
            var metIndex = parNum;
            foreach (var item in pars)
            {
                parNum++;
                parsStr += $"{item.ParameterType.FullName} {item.Name},";
                conditionStr += $"{item.Name}.Equals(fields[{parNum}]) & ";
                setValueStr += $"fields[{parNum}] = {item.Name};\r\n\t\t\t";
                paramsStr += $"{item.Name},";
                setValueStr1 += $"var {item.Name} = ({item.ParameterType.FullName})(fields[{parNum}] = data.Obj);\r\n\t\t\t\t\t\t";
            }
            if (setValueStr.Length > 0)
                setValueStr = setValueStr.Remove(setValueStr.Length - 5, 5);
            if (setValueStr1.Length > 0)
                setValueStr1 = setValueStr1.Remove(setValueStr1.Length - 8, 8);

            if (immediatelys.Contains(metName))
            {
                parsStr = parsStr.TrimEnd(',');
                paramsStr = paramsStr.TrimEnd(',');
                blockCode = blockCodes[9];
                blockCode = blockCode.Replace("{FuncName}", $"{met.Name}");
                blockCode = blockCode.Replace("{ParsString}", $"{parsStr}");
                blockCode = blockCode.Replace("{ReturnType}", $"{(met.ReturnType == typeof(void) ? "void" : met.ReturnType.FullName)}");
                blockCode = blockCode.Replace("{Return}", $"{(met.ReturnType == typeof(void) ? "" : "return ")}");
                blockCode = blockCode.Replace("{Params}", $"{paramsStr}");
                blockCode = blockCode.Remove(blockCode.Length - 2, 2);
                sb4.Append(blockCode);
                parNum = metIndex - 1;
                continue;
            }

            blockCode = blockCodes[8];
            blockCode = blockCode.Replace("{FuncName}", $"{met.Name}");
            blockCode = blockCode.Replace("{ParsString}", $"{parsStr}");
            blockCode = blockCode.Replace("{Condition}", $"{conditionStr}");
            blockCode = blockCode.Replace("{SetPars}", $"{setValueStr}");
            blockCode = blockCode.Replace("{Params}", $"{paramsStr}");
            blockCode = blockCode.Replace("{FieldIndex}", $"{metIndex}");

            blockCode = blockCode.Replace("{EIDIndex}", $"{metIndex}");

            blockCode = blockCode.Remove(blockCode.Length - 2, 2);
            sb4.Append(blockCode);

            if (pars.Length == 0)
            {
                blockCode = blockCodes[12];
                blockCode = blockCode.Replace("{FieldIndex1}", $"{metIndex}");
                blockCode = blockCode.Replace("{FuncName}", $"{met.Name}");
                blockCode = blockCode.Remove(blockCode.Length - 2, 2);
                sb5.Append(blockCode);
            }
            else
            {
                paramsStr = paramsStr.TrimEnd(',');
                blockCode = blockCodes[13];
                blockCode = blockCode.Replace("{FieldIndex1}", $"{metIndex}");
                blockCode = blockCode.Replace("{FuncName}", $"{met.Name}");
                blockCode = blockCode.Replace("{Params}", $"{paramsStr}");
                blockCode = blockCode.Replace("{SetPars}", $"{setValueStr1}");
                blockCode = blockCode.Remove(blockCode.Length - 2, 2);
                sb5.Append(blockCode);
            }
        }

        var blockCodeX = blockCodes[1];
        blockCodeX = blockCodeX.Replace("{FieldSize}", $"{parNum}");
        blockCodeX = blockCodeX.Remove(blockCodeX.Length - 2, 2);
        sb.Append(blockCodeX);
        sb.Append(sb1.ToString());
        sb.Append(blockCodes[3]);

        sb.Append(sb2.ToString());

        sb.Append(blockCodes[5]);
        sb.Append(sb3.ToString());
        sb.Append(blockCodes[7]);

        sb.Append(sb4.ToString());

        sb.Append(blockCodes[10]);
        sb.Append(sb5.ToString());
        sb.Append(blockCodes[14]);

        return sb;
    }

    void LoadData()
    {
        var path = Application.dataPath.Replace("Assets", "") + "data3.txt";
        if (File.Exists(path))
        {
            var jsonStr = File.ReadAllText(path);
            var data = Newtonsoft_X.Json.JsonConvert.DeserializeObject<Data>(jsonStr);
            savePath = data.savepath;
            savePath1 = data.savepath1;
        }
    }
    void SaveData()
    {
        Data data = new Data()
        {
            savepath = savePath,
            savepath1 = savePath1,
        };
        var jsonstr = Newtonsoft_X.Json.JsonConvert.SerializeObject(data);
        var path = Application.dataPath.Replace("Assets", "") + "data3.txt";
        File.WriteAllText(path, jsonstr);
    }
    internal class Data
    {
        public string savepath, savepath1;
    }
}
#endif