using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

public class AssetReference
{
    [MenuItem("Assets/Select Reference", priority = 30)]
    public static void GetReference()
    {
        //字典存放资产的依赖关系
        Dictionary<string, string[]> map = 
            new Dictionary<string, string[]>();
        //获取所有资产的路径
        string[] paths = AssetDatabase.GetAllAssetPaths();
        //遍历 建立资产间的依赖关系
        for (int i = 0; i < paths.Length; i++)
        {
            string path = paths[i];
            //根据资产路径获取该资产的依赖项
            var dependencies = AssetDatabase.GetDependencies(path).ToList();
            //获取依赖项时会包含该资产本身 将本身移除
            dependencies.Remove(path);
            //加入字典
            if (dependencies.Count > 0)
                map.Add(path, dependencies.ToArray());
            //进度条   
            EditorUtility.DisplayProgressBar("获取资产依赖关系",
                path, (float)i + 1 / paths.Length);
        }
        EditorUtility.ClearProgressBar();

        //当前所选中资产的路径
        string assetPath = AssetDatabase
            .GetAssetPath(Selection.activeObject);
        //所有引用项的资产路径
        string[] reference = map.Where(
            m => m.Value.Contains(assetPath)).Select(m => m.Key).ToArray();
        //根据路径加载引用项资产 存储到列表中
        List<Object> objects = new List<Object>();
        for (int i = 0; i < reference.Length; i++)
            objects.Add(AssetDatabase.LoadMainAssetAtPath(reference[i]));
        //选中所有的引用项
        Selection.objects = objects.ToArray();
    }

    [MenuItem("Assets/Select Reference", true)]
    public static bool GetReferencesValidate()
    {
        return Selection.activeObject != null;
    }
}