using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// Mesh网格合并
/// </summary>
public class MeshCombiner
{
    [MenuItem("Example/Mesh/Combine %M")]
    public static void Execute()
    {
        //弹出对话框 让用户选择是否合并到单个子网格中
        bool mergeSubMeshes = EditorUtility.DisplayDialog(
            "网格合并", "是否合并到单个子网格中", "是", "否");
        //列表存储选中的所有网格实例
        List<CombineInstance> instances = new List<CombineInstance>();
        //遍历当前所选中的物体
        for (int i = 0; i < Selection.gameObjects.Length; i++)
        {
            //非空判断
            GameObject go = Selection.gameObjects[i];
            if (go == null) continue;
            MeshFilter meshFilter = go.GetComponent<MeshFilter>();
            if (meshFilter == null) continue;
            Mesh target = meshFilter.sharedMesh;
            if (target == null) continue;
            //添加到列表中
            instances.Add(new CombineInstance 
            {
                mesh = target, 
                transform = meshFilter.transform.localToWorldMatrix 
            });
        }
        //获取所有选中的MeshRenderer组件
        var mrs = Selection.gameObjects.Select(
            m => m.GetComponent<MeshRenderer>()).ToArray();
        //如果合并到单个子网格中 获取第一个MeshRenderer组件的材质即可
        //否则获取所有MeshRenderer组件的材质
        var materials = mergeSubMeshes 
            ? mrs.First().sharedMaterials.ToArray() 
            : mrs.SelectMany(m => m.sharedMaterials).ToArray();
        //创建一个新的物体
        GameObject instance = new GameObject("New Mesh Combined");
        //为其添加MeshFilter组件
        MeshFilter filter = instance.AddComponent<MeshFilter>();
        //创建新的网格
        filter.mesh = new Mesh { name = instance.name };
        //网格合并
        filter.sharedMesh.CombineMeshes(instances.ToArray(),
            mergeSubMeshes);
        //为新物体添加MeshRenderer组件
        MeshRenderer renderer = instance.AddComponent<MeshRenderer>();
        //为MeshRenderer组件设置材质
        renderer.sharedMaterials = materials.ToArray();
    }

    [MenuItem("Example/Mesh/Combine %M", true)]
    public static bool Validate()
    {
        //当前选中的游戏物体至少两个 才可以进行网格合并
        return Selection.gameObjects.Length >= 2;
    }
}