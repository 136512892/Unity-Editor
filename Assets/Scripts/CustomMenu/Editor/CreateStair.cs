using UnityEngine;
using UnityEditor;

public class CreateStair 
{
    /// <summary>
    /// 创建3D物体 阶梯
    /// </summary>
    [MenuItem("GameObject/3D Object/Stair")]
    public static void Execute()
    {
        GameObject go = new GameObject("Stair");
        MeshFilter mf = go.AddComponent<MeshFilter>();
        MeshRenderer mr = go.AddComponent<MeshRenderer>();
        //加载阶梯网格资产
        Mesh stairMesh = AssetDatabase.LoadAssetAtPath<Mesh>(
            "Assets/Mesh/Stair.asset");
        mf.sharedMesh = stairMesh;
        mr.sharedMaterial = new Material(Shader.Find("Standard"));
        //如果当前在Hierarchy窗口中已经选中了某个游戏物体
        //将新创建的阶梯物体设为它的子级
        if (Selection.activeTransform != null)
        {
            go.transform.SetParent(Selection.activeTransform);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;
        }
        Selection.activeTransform = go.transform;
    }
}