#if UNITY_EDITOR
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System.IO;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class DebugTools : EditorWindow
{
    [MenuItem("GameDesigner/Network/DebugTools")]
    static void ShowWindow()
    {
        var window = GetWindow<DebugTools>("Debug工具");
        window.Show();
    }

    private void OnGUI()
    {
        EditorGUILayout.HelpBox(@"此工具可以帮你在控制台打印时, 直接显示Rpc调用到的方法, 可以双击进入Rpc方法位置, 使用此工具会修改
unity编辑器的UnityEditor.CoreModule.dll,修改IL指令让控制台判断打印内容也显示高亮, !!!注意:请备份你的UnityEditor.CoreModule.dll,
以免注入IL指令编译后,unity程序集错误导致unity打不开崩溃问题!!!", MessageType.Info);
        if (GUILayout.Button("注入编辑器UnityEditor.CoreModule.dll", GUILayout.Height(30)))
        {
            var modCtx = ModuleDef.CreateModuleContext();
            var path = typeof(EditorWindow).Assembly.Location;
            var stream = new MemoryStream(File.ReadAllBytes(path));
            var module = ModuleDefMD.Load(stream, modCtx);
            foreach (var type in module.Types)
            {
                if (type.FullName == "UnityEditor.ConsoleWindow")
                {
                    foreach (var method in type.Methods)
                    {
                        if (method.Name == "StacktraceWithHyperlinks")
                        {
                            for (int i = 0; i < method.Body.Instructions.Count; i++)
                            {
                                var instruction = method.Body.Instructions[i];
                                if (instruction.OpCode.Code == Code.Ldarg_1)
                                {
                                    method.Body.Instructions[i] = Instruction.CreateLdcI4(0);
                                }
                            }
                        }
                    }
                }
            }
            module.Write(path);
            Debug.Log("注入完成!");
        }
    }
}
#endif