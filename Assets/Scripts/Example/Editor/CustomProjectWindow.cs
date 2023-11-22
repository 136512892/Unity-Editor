//using UnityEngine;
//using UnityEditor;

//public class CustomProjectWindow
//{
//    [InitializeOnLoadMethod]
//    static void InitializeOnLoad()
//    {
//        EditorApplication.projectWindowItemOnGUI
//            -= OnProjectWindowItemGUI;
//        EditorApplication.projectWindowItemOnGUI
//            += OnProjectWindowItemGUI;
//    }

//    private static void OnProjectWindowItemGUI(
//        string guid, Rect selectionRect)
//    {
//        string assetPath = AssetDatabase.GUIDToAssetPath(guid);
//        string[] dependencies = AssetDatabase.GetDependencies(assetPath);
//        if (dependencies.Length == 0) return;
//        Rect rect = selectionRect;
//        rect.x += selectionRect.width - 20f;
//        rect.width = 20f;
//        GUI.Label(rect, dependencies.Length.ToString());
//    }
//}