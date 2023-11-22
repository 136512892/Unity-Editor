using UnityEngine;
using UnityEditor;

public class GUIStylePreviewer : EditorWindow
{
    [MenuItem("Example/GUIStyle Previewer")]
    public static void Open()
    {
        GetWindow<GUIStylePreviewer>().Show();
    }

    private Vector2 scroll;
    //检索的内容
    private string searchContent = string.Empty;

    private void OnGUI()
    {
        //搜索栏
        GUILayout.BeginHorizontal(EditorStyles.toolbar);
        GUILayout.Label("Search", GUILayout.Width(50f));
        searchContent = GUILayout.TextField(searchContent, 
            EditorStyles.toolbarSearchField);
        GUILayout.EndHorizontal();

        //滚动视图
        scroll = EditorGUILayout.BeginScrollView(scroll);
        for (int i = 0; i < GUI.skin.customStyles.Length; i++)
        {
            var style = GUI.skin.customStyles[i];
            //是否符合检索内容
            if (style.name.ToLower().Contains(searchContent.ToLower()))
            {
                if (GUILayout.Button(style.name, style))
                {
                    //按钮点击时 复制样式的名称到粘贴板中
                    EditorGUIUtility.systemCopyBuffer = style.name;
                    Debug.Log(style.name);
                }
            }
        }
        EditorGUILayout.EndScrollView();
    }
}