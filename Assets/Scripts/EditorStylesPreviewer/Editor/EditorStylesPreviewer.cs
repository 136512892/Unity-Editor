using System.Linq;
using System.Reflection;

using UnityEngine;
using UnityEditor;

public class EditorStylesPreviewer : EditorWindow
{
    [MenuItem("Example/EditorStyles Previewer")]
    public static void Open()
    {
        GetWindow<EditorStylesPreviewer>().Show();
    }

    private Vector2 scroll;
    private GUIStyle[] styles;

    private void OnEnable()
    {
        //反射方式获取EditorStyles中公开的样式
        PropertyInfo[] propertyInfos = typeof(EditorStyles)
            .GetProperties(BindingFlags.Public | BindingFlags.Static);
        propertyInfos = propertyInfos.Where(m 
            => m.PropertyType == typeof(GUIStyle)).ToArray();
        styles = new GUIStyle[propertyInfos.Length];
        for (int i = 0; i < styles.Length; i++)
        {
            styles[i] = propertyInfos[i].GetValue(null, null) as GUIStyle;
        }
    }

    private void OnGUI()
    {
        //滚动视图
        scroll = EditorGUILayout.BeginScrollView(scroll);
        for (int i = 0; i < styles.Length; i++)
        {
            var style = styles[i];
            if (GUILayout.Button(style.name, style))
            {
                //按钮点击时 复制样式的名称到粘贴板中
                EditorGUIUtility.systemCopyBuffer = style.name;
                Debug.Log(style.name);
            }
        }
        EditorGUILayout.EndScrollView();
    }
}