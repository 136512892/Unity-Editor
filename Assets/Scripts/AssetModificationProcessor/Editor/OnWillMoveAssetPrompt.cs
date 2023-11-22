using UnityEditor;
using UnityEngine;

public class OnWillMoveAssetPrompt : UnityEditor.AssetModificationProcessor
{
    /// <summary>
    /// 即将移动资产时调用
    /// </summary>
    /// <param name="sourcePath">资产当前路径</param>
    /// <param name="destinationPath">资产要移动到的路径</param>
    /// <returns></returns>
    public static AssetMoveResult OnWillMoveAsset(
        string sourcePath, string destinationPath)
    {
        //Debug.Log(string.Format("SourcePath：{0}  " +
        //    "DestinationPath：{1}", sourcePath, destinationPath));
        if (AssetDatabase.IsValidFolder(sourcePath))
        {
            //在sourcePath文件夹中查找是否包含脚本文件
            if (AssetDatabase.FindAssets("t:Script", 
                new string[] { sourcePath }).Length != 0)
            {
                //弹出确认弹窗
                if (!EditorUtility.DisplayDialog("提示", string.Format(
                    "是否确认将文件夹{0}移动至{1}", 
                    sourcePath, destinationPath), "确认", "取消"))
                    return AssetMoveResult.FailedMove;
            }
        }
        return AssetMoveResult.DidNotMove;
    }
}