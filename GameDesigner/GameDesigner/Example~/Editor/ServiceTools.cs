#if UNITY_EDITOR
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ServiceTools
{
    [MenuItem("GameDesigner/Example/Example1_Service")]
    static void Init() 
    {
        var files = Directory.GetFiles(Application.dataPath, "ExampleServer.exe", SearchOption.AllDirectories);
        if (files.Length == 0)
            return;
        var exe = files[0];
        Process p = new Process();
        p.StartInfo.FileName = exe;
        p.StartInfo.Arguments = "Example1";
        p.Start();
    }
    [MenuItem("GameDesigner/Example/Example2_Service")]
    static void Init1()
    {
        var files = Directory.GetFiles(Application.dataPath, "ExampleServer.exe", SearchOption.AllDirectories);
        if (files.Length == 0)
            return;
        var exe = files[0];
        Process p = new Process();
        p.StartInfo.FileName = exe;
        p.StartInfo.Arguments = "Example2";
        p.Start();
    }
    [MenuItem("GameDesigner/Example/LockStepService")]
    static void Init2()
    {
        var files = Directory.GetFiles(Application.dataPath, "ExampleServer.exe", SearchOption.AllDirectories);
        if (files.Length == 0)
            return;
        var exe = files[0];
        Process p = new Process();
        p.StartInfo.FileName = exe;
        p.StartInfo.Arguments = "Example3";
        p.Start();
    }
    [MenuItem("GameDesigner/Example/AOIService")]
    static void Init3()
    {
        var files = Directory.GetFiles(Application.dataPath, "ExampleServer.exe", SearchOption.AllDirectories);
        if (files.Length == 0)
            return;
        var exe = files[0];
        Process p = new Process();
        p.StartInfo.FileName = exe;
        p.StartInfo.Arguments = "Example4";
        p.Start();
    }
}
#endif