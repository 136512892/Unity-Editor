//using UnityEngine;
//using UnityEditor;

//public class CustomHierarchyWindow
//{
//    [InitializeOnLoadMethod]
//    static void InitializeOnLoad()
//    {
//        EditorApplication.hierarchyWindowItemOnGUI
//            -= OnHierarchyWindowItemGUI;
//        EditorApplication.hierarchyWindowItemOnGUI 
//            += OnHierarchyWindowItemGUI;
//    }

//    private static void OnHierarchyWindowItemGUI(
//        int instanceID, Rect selectionRect)
//    {
//        GameObject go = EditorUtility
//            .InstanceIDToObject(instanceID) as GameObject;
//        if (go == null) return;
//        Component[] components = go.GetComponents<Component>();
//        for (int i = 0; i < components.Length; i++)
//        {
//            Component component = components[i];
//            if (component == null) continue;
//            Texture texture = AssetPreview.GetMiniTypeThumbnail(
//                component.GetType()) ?? 
//                AssetPreview.GetMiniThumbnail(component);
//            if (texture == null) continue;
//            Rect rect = selectionRect;
//            rect.x += selectionRect.width - (i + 1) * 20f;
//            rect.width = 20f;
//            GUI.Label(rect, new GUIContent(texture,
//                component.GetType().Name));
//        }
//    }
//}