using System;
using System.Text.RegularExpressions;

using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

public class AssetBundleUtility
{
    /// <summary>
    /// 递归方式获取一个唯一的AssetBundle名称
    /// （检查是否存在具有相同名称的AssetBundle，
    /// 如果有，方法会将数字1附加到名称后再次检查，
    /// 持续递增数字再次检查直到名称是唯一的）
    /// </summary>
    /// <param name="assetBundleName">AssetBundle名称</param>
    /// <returns></returns>
    public static string GetUniqueAssetBundleNameRecursive(
        string assetBundleName)
    {
        //获取当前所有的AssetBundle名称
        string[] assetBundleNames = AssetDatabase.GetAllAssetBundleNames();
        //查找当前是否已经包含该名称
        int index = Array.FindIndex(assetBundleNames,
            m => m == assetBundleName.ToLower());
        //已经包含
        if (index != -1)
        {
            string target = assetBundleNames[index];
            //匹配字符串末尾的数字内容
            string numberStr = Regex.Match(target, @"\d+$").Value;
            //数字内容不为空
            if (!string.IsNullOrEmpty(numberStr))
            {
                //尝试类型转换
                int.TryParse(numberStr, out int number);
                //截掉末尾数字内容，字符截取长度
                int subLength = target.Length - numberStr.Length;
                //拼接自增后的数字形成新的名称
                string newName = target.Substring(target.Length - 
                    subLength - 1, subLength) + (++number);
                //递归获取，直到名称是当前唯一
                return GetUniqueAssetBundleNameRecursive(newName);
            }
            //数字内容为空
            else
                return GetUniqueAssetBundleNameRecursive(
                    assetBundleName + 1);
        }
        //当前不包含该名称，已经是唯一的名称
        else return assetBundleName;
    }

    /// <summary>
    /// 为资产创建AssetBundle
    /// </summary>
    /// <param name="obj">资产</param>
    /// <returns>创建成功返回true 否则返回false</returns>
    public static bool CreateAssetBundle4Object(Object obj)
    {
        //根据资产获取资产的路径
        string assetPath = AssetDatabase.GetAssetPath(obj);
        //根据资产路径获取资产导入器
        AssetImporter importer = AssetImporter.GetAtPath(assetPath);
        if (importer == null) return false;
        //为资产设置AssetBundle名称
        importer.assetBundleName =
            GetUniqueAssetBundleNameRecursive(obj.name);
        return true;
    }

    /// <summary>
    /// 删除AssetBundle名称
    /// </summary>
    /// <param name="assetBundleName">AssetBundle名称</param>
    public static void DeleteAssetBundleName(string assetBundleName)
    {
        //根据AssetBundle名称获取其中的资产路径集合
        string[] assetPaths = AssetDatabase
            .GetAssetPathsFromAssetBundle(assetBundleName);
        for (int i = 0; i < assetPaths.Length; i++)
        {
            //根据资产路径获取资产导入器
            AssetImporter importer = AssetImporter.GetAtPath(assetPaths[i]);
            if (importer == null) continue;
            //清空资产的AssetBundle名
            importer.assetBundleName = null;
        }
        //最终移除AssetBundle名称
        AssetDatabase.RemoveAssetBundleName(assetBundleName, true);
    }
}