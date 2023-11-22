using System;
using UnityEditor;
using System.Reflection;

public class HierarchyWindowUtility
{
    [MenuItem("GameObject/Expand", priority = 0)]
    public static void Expand()
    {
        //展开物体层级
        SetExpandedRecursive(true);
    }
    [MenuItem("GameObject/Retract", priority = 0)]
    public static void Retract()
    {
        //收起物体层级
        SetExpandedRecursive(false);
    }
    private static void SetExpandedRecursive(bool expand)
    {
        //获取SceneHierarchyWindow类型
        Type type = typeof(EditorWindow).Assembly.GetType(
            "UnityEditor.SceneHierarchyWindow");
        //获取SetExpandedRecursive方法
        MethodInfo methodInfo = type.GetMethod("SetExpandedRecursive",
            BindingFlags.Public | BindingFlags.Instance);
        //根据类型获取窗口实例
        EditorWindow window = EditorWindow.GetWindow(type);
        object[] array = new object[2];
        array[1] = expand;
        for (int i = 0; i < Selection.transforms.Length; i++)
        {
            array[0] = Selection.transforms[i].gameObject.GetInstanceID();
            //反射方式调用SetExpandedRecursive方法
            methodInfo.Invoke(window, array);
        }
    }
}