using UnityEngine;
using UnityEditor;

public class ExampleEditorWindow : EditorWindow
{
    [MenuItem("Example/Open Example Editor Window")]
    public static void Open()
    {
        GetWindow<ExampleEditorWindow>().Show();
    }

    private Vector2 scrollPosition;
    private const float splitterWidth = 2f;
    private float splitterPos;
    private Rect splitterRect;
    private bool isDragging;

    private void OnEnable()
    {
        splitterPos = position.width * .3f;
    }

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        {
            GUILayout.BeginVertical(GUILayout.Width(splitterPos));
            GUILayout.Box("左侧区域", GUILayout.ExpandHeight(true),
                GUILayout.ExpandWidth(true));
            GUILayout.EndVertical();

            //分割线 垂直扩展 
            GUILayout.Box(string.Empty, GUILayout.Width(splitterWidth),
                GUILayout.ExpandHeight(true));
            //该方法用于获取GUILayout最后用于控件的矩形区域
            splitterRect = GUILayoutUtility.GetLastRect();

            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            GUILayout.Box("右侧区域", GUILayout.ExpandHeight(true),
                GUILayout.ExpandWidth(true));
            GUILayout.EndVertical();
        }
        GUILayout.EndHorizontal();

        if (Event.current != null)
        {
            //该方法用于在指定区域内显示指定的鼠标光标类型
            EditorGUIUtility.AddCursorRect(splitterRect,
                MouseCursor.ResizeHorizontal);
            switch (Event.current.type)
            {
                //开始拖拽分割线
                case EventType.MouseDown:
                    isDragging = splitterRect.Contains(
                        Event.current.mousePosition);
                    break;
                case EventType.MouseDrag:
                    if (isDragging)
                    {
                        splitterPos += Event.current.delta.x;
                        //限制其最大最小值
                        splitterPos = Mathf.Clamp(splitterPos,
                            position.width * .2f, position.width * .8f);
                        Repaint();
                    }
                    break;
                //结束拖拽分割线
                case EventType.MouseUp:
                    if (isDragging)
                        isDragging = false;
                    break;
            }
        }
    }

    private void ScrollViewExample()
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        for (int i = 0; i < 50; i++)
        {
            GUILayout.Button("Button" + i);
        }
        GUILayout.EndScrollView();
    }
}