using System;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Mesh网格提取器
/// </summary>
public class MeshExtracter
{
    [MenuItem("Example/Mesh/Extract")]
    public static void Execute()
    {
        //非空判断
        MeshFilter meshFilter = Selection.activeGameObject
            .GetComponent<MeshFilter>();
        if (meshFilter == null) return;
        Mesh mesh = meshFilter.sharedMesh;
        if (mesh == null) return;
        try
        {
            Mesh instance = UnityEngine.Object.Instantiate(mesh); //实例化
            AssetDatabase.CreateAsset(instance,
                string.Format("Assets/{0}.asset", mesh.name)); //创建资产 
            AssetDatabase.Refresh(); //刷新
            Selection.activeObject = instance; //选中
        }
        catch (Exception error)
        {
            Debug.Log(string.Format("{0}提取Mesh网格失败: {1}",
                Selection.activeGameObject.name, error));
        }
    }

    [MenuItem("Example/Mesh/Extract", true)]
    public static bool Validate()
    {
        return Selection.activeGameObject != null;
    }
}