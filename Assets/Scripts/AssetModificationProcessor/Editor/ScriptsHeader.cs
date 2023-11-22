using System.IO;
using UnityEngine;

/// <summary>
/// 新创建的脚本自动添加模板注释(头部注释)
/// </summary>
public class ScriptsHeader : UnityEditor.AssetModificationProcessor
{
    private const string author = "张寿昆";
    private const string email = "136512892@qq.com";
    private const string firstVersion = "1.0.0";

    /// <summary>
    /// 资产创建时调用
    /// </summary>
    /// <param name="path">资产路径</param>
    public static void OnWillCreateAsset(string path)
    {
        path = path.Replace(".meta", "");
        if (!path.EndsWith(".cs")) return;
        string scriptPath = Application.dataPath
            .Replace("Assets", "") + path;
        string header = string.Format(
                         "/* =======================================================\r\n"
                       + " *  Unity版本：{0}\r\n"
                       + " *  作 者：{1}\r\n"
                       + " *  邮 箱：{2}\r\n"
                       + " *  创建时间：{3}\r\n"
                       + " *  当前版本：{4}\r\n"
                       + " *  主要功能：\r\n"
                       + " *  详细描述：\r\n"
                       + " *  修改记录：\r\n"
                       + " * =======================================================*/\r\n\r\n",
                         Application.unityVersion, author, email,
                        System.DateTime.Now.ToString(
                            "yyyy-MM-dd HH:mm:ss"), firstVersion);
        File.WriteAllText(scriptPath, header 
            + File.ReadAllText(scriptPath));
    }
}