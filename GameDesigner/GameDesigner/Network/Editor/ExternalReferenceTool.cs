#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEditor;
using UnityEngine;

public class ExternalReferenceTool : EditorWindow
{
    private List<string> paths = new List<string>();
    private List<string> csprojPaths = new List<string>();
    private Vector2 scrollPosition, scrollPosition1;

    [MenuItem("GameDesigner/Network/ExternalReference")]
    static void ShowWindow()
    {
        var window = GetWindow<ExternalReferenceTool>("多个项目外部引用工具");
        window.Show();
    }

    private void OnEnable()
    {
        LoadData();
    }

    private void OnGUI()
    {
        EditorGUI.BeginChangeCheck();
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("项目文件列表:");
        if (GUILayout.Button("选择文件", GUILayout.Width(100)))
        {
            var csprojPath = EditorUtility.OpenFilePanel("选择文件", "", "csproj");
            if (!string.IsNullOrEmpty(csprojPath))
                csprojPaths.Add(csprojPath);
            SaveData();
        }
        GUILayout.EndHorizontal();
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true, GUILayout.MaxHeight(position.height / 2));
        foreach (var path in csprojPaths)
        {
            var rect = EditorGUILayout.GetControlRect();
            EditorGUI.LabelField(new Rect(rect.position, rect.size - new Vector2(50, 0)), path);
            if (GUI.Button(new Rect(rect.position + new Vector2(position.width - 50, 0), new Vector2(20, rect.height)), "x"))
            {
                csprojPaths.Remove(path);
                SaveData();
                return;
            }
        }
        GUILayout.EndScrollView();
        EditorGUILayout.LabelField("引用文件夹列表:");
        scrollPosition1 = GUILayout.BeginScrollView(scrollPosition1, false, true, GUILayout.MaxHeight(position.height / 2));
        foreach (var path in paths)
        {
            var rect = EditorGUILayout.GetControlRect();
            EditorGUI.LabelField(new Rect(rect.position, rect.size - new Vector2(50, 0)), path);
            if (GUI.Button(new Rect(rect.position + new Vector2(position.width - 50, 0), new Vector2(20, rect.height)), "x"))
            {
                paths.Remove(path);
                SaveData();
                return;
            }
        }
        GUILayout.EndScrollView();
        if (GUILayout.Button("添加引用路径", GUILayout.Height(30)))
        {
            var path = EditorUtility.OpenFolderPanel("引用路径", "", "");
            if (!string.IsNullOrEmpty(path)) 
            {
                if(!paths.Contains(path))
                    paths.Add(path);
            }
            SaveData();
        }
        if (GUILayout.Button("执行", GUILayout.Height(30)))
        {
            foreach (var csprojPath in csprojPaths)
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(csprojPath);
                XmlNodeList node_list;
                XmlElement documentElement = xml.DocumentElement;
                var namespaceURI = xml.DocumentElement.NamespaceURI;
                if (!string.IsNullOrEmpty(namespaceURI))
                {
                    XmlNamespaceManager nsMgr = new XmlNamespaceManager(xml.NameTable); nsMgr.AddNamespace("ns", namespaceURI);
                    node_list = xml.SelectNodes("/ns:Project/ns:ItemGroup", nsMgr);
                }
                else node_list = xml.SelectNodes("/Project/ItemGroup");
                foreach (var path in paths)
                {
                    var path1 = path.Replace("/", "\\");
                    var dir = new DirectoryInfo(path);
                    var dirName = dir.Parent.FullName + "\\";
                    var files = Directory.GetFiles(path1, "*.cs", SearchOption.AllDirectories);

                    bool exist = false;
                    for (int i = 0; i < node_list.Count; i++)
                    {
                        XmlNode node = node_list.Item(i);
                        XmlNodeList node_child = node.ChildNodes;
                        foreach (XmlNode child_node in node_child)
                        {
                            if (child_node.LocalName != "Compile")
                                continue;
                            var value = child_node.Attributes["Include"].Value;
                            if (value.Contains(path1))
                            {
                                exist = true;
                                break;
                            }
                        }
                        if (exist == true)
                        {
                            node.RemoveAll();
                            foreach (var file in files)
                            {
                                XmlElement e = xml.CreateElement("Compile", namespaceURI);
                                e.SetAttribute("Include", file);
                                XmlElement e1 = xml.CreateElement("Link", namespaceURI);
                                e1.InnerText = file.Replace(dirName, "");
                                e.AppendChild(e1);
                                node.AppendChild(e);
                            }
                            break;
                        }
                    }
                    if (!exist)
                    {
                        XmlElement node = xml.CreateElement("ItemGroup", namespaceURI);
                        foreach (var file in files)
                        {
                            XmlElement e = xml.CreateElement("Compile", namespaceURI);
                            e.SetAttribute("Include", file);
                            XmlElement e1 = xml.CreateElement("Link", namespaceURI);
                            e1.InnerText = file.Replace(dirName, "");
                            e.AppendChild(e1);
                            node.AppendChild(e);
                        }
                        documentElement.AppendChild(node);
                    }
                }
                xml.Save(csprojPath);
            }
            Debug.Log("更新完成!");
        }
        if (EditorGUI.EndChangeCheck())
            SaveData();
    }

    void LoadData()
    {
        var path = Application.dataPath.Replace("Assets", "") + "data4.txt";
        if (File.Exists(path))
        {
            var jsonStr = File.ReadAllText(path);
            var data = Newtonsoft_X.Json.JsonConvert.DeserializeObject<Data>(jsonStr);
            paths = data.paths;
            csprojPaths = data.csprojPaths;
        }
    }

    void SaveData()
    {
        Data data = new Data() { paths = paths, csprojPaths = csprojPaths };
        var jsonstr = Newtonsoft_X.Json.JsonConvert.SerializeObject(data);
        var path = Application.dataPath.Replace("Assets", "") + "data4.txt";
        File.WriteAllText(path, jsonstr);
    }

    internal class Data
    {
        public List<string> csprojPaths = new List<string>();
        public List<string> paths = new List<string>();
    }
}
#endif