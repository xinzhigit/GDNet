using Example2;
using System.IO;
using UnityEditor;
using UnityEngine;
using SceneManager = UnityEngine.SceneManagement.SceneManager;

public class BuildPathTools
{
    [MenuItem("GameDesigner/Example/Example2/BuildSceneData")]
    static void Init()
    {
        var dirs = Directory.GetDirectories(Application.dataPath, "ExampleServer~", SearchOption.AllDirectories);
        if (dirs.Length == 0)
            throw new System.Exception("找不到目录!");
        var scene = SceneManager.GetActiveScene();
        var path = dirs[0] + "/bin/Debug/Data/" + scene.name + ".sceneData";
        var roamingPaths = Object.FindObjectsOfType<RoamingPath>();
        SceneData sceneData = new SceneData();
        sceneData.name = scene.name;
        foreach (var item in roamingPaths)
        {
            var monsterPoint = item.GetComponent<MonsterPoint>();
            sceneData.monsterPoints.Add(new MonsterPoint1()
            {
                roamingPath = new RoamingPath1() { waypointsList = item.waypointsList.ConvertAll(x => (Net.Vector3)x) },
                monsters = monsterPoint.monsters,
            });
        }
        var jsonStr = Newtonsoft_X.Json.JsonConvert.SerializeObject(sceneData);
        File.WriteAllText(path, jsonStr);
        Debug.Log($"场景数据生成成功!--{path}");
    }
}