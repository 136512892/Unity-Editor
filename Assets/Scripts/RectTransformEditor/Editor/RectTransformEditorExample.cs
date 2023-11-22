using System.Linq;
using System.Reflection;

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RectTransform))]
public class RectTransformEditorExample : Editor
{
    private RectTransform rt;
    private Editor instance;
    private MethodInfo onSceneGUI;
    private static readonly object[] emptyArray = new object[0];

    private void OnEnable()
    {
        rt = target as RectTransform;
        //获取RectTransformEditor类型
        var editorType = Assembly.GetAssembly(typeof(Editor)).GetTypes()
            .FirstOrDefault(m => m.Name == "RectTransformEditor");
        //创建指定类型的编辑器实例
        instance = CreateEditor(targets, editorType);
        //OnSceneGUI方法是私有的 因此通过反射方式调用
        onSceneGUI = editorType.GetMethod("OnSceneGUI", 
            BindingFlags.Instance | BindingFlags.Static
            | BindingFlags.Public | BindingFlags.NonPublic);
    }
    private void OnSceneGUI()
    {
        if (instance)
            onSceneGUI.Invoke(instance, emptyArray);
    }
    private void OnDisable()
    {
        if (instance != null)
            DestroyImmediate(instance);
    }
    public override void OnInspectorGUI()
    {
        instance.OnInspectorGUI();
        OnAnchorSetHelperGUI();
    }

    //锚点设置工具
    private void OnAnchorSetHelperGUI()
    {
        EditorGUILayout.Space();
        Color color = GUI.color;
        GUI.color = Color.cyan;
        GUILayout.Label("锚点工具", EditorStyles.boldLabel);
        GUI.color = color;
        if (GUILayout.Button("Auto Anchor"))
        {
            Undo.RecordObject(rt, "Auto Anchor");
            RectTransform prt = rt.parent as RectTransform;
            Vector2 anchorMin = new Vector2(
                rt.anchorMin.x + rt.offsetMin.x / prt.rect.width,
                rt.anchorMin.y + rt.offsetMin.y / prt.rect.height);
            Vector2 anchorMax = new Vector2(
                rt.anchorMax.x + rt.offsetMax.x / prt.rect.width,
                rt.anchorMax.y + rt.offsetMax.y / prt.rect.height);
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.offsetMin = rt.offsetMax = Vector2.zero;
        }
    }
}