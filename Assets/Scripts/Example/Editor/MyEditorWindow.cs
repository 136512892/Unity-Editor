using UnityEngine;
using UnityEditor;

public class MyEditorWindow : EditorWindow
{
    [MenuItem("Example/My Editor Window")]
    public static void Open()
    {
        MyEditorWindow window = GetWindow<MyEditorWindow>();
        window.titleContent = new GUIContent("窗口标题");
        window.minSize = new Vector2(300f, 300f);
        window.maxSize = new Vector2(1920f, 1080f);
        window.Show();
    }
    private Rect examplePupupWindowRect;

    private GUIStyle m_Style;
    
    //private void OnEnable()
    //{
    //    Debug.Log("OnEnable");
    //}
    private void OnGUI()
    {
        if (GUILayout.Button("PopupWindow Example"))
            PopupWindow.Show(examplePupupWindowRect,
                new ExamplePopupWindowContent(
                    new Vector2(position.width - 6f, 100f)));
        if (Event.current.type == EventType.Repaint)
            examplePupupWindowRect = GUILayoutUtility.GetLastRect();

        if (GUILayout.Button("Button"))
        {
            GenericMenu gm = new GenericMenu();
            //添加菜单项
            gm.AddItem(new GUIContent("Memu1"), true,
                () => Debug.Log("Select Menu1"));
            //添加分隔符 参数传空字符串表示在一级菜单中添加分隔符
            gm.AddSeparator(string.Empty);
            //添加不可交互菜单项
            gm.AddDisabledItem(new GUIContent("Memu2"));
            //通过'/'可添加子菜单项
            gm.AddItem(new GUIContent("Menu3/SubMenu1"), false,
                () => Debug.Log("Select SubMenu1"));
            //在子菜单中添加分隔符
            gm.AddSeparator("Menu3/");
            gm.AddItem(new GUIContent("Menu3/SubMenu2"), false,
                () => Debug.Log("Select SubMenu2"));
            //显示菜单
            gm.ShowAsContext();
        }

        if (m_Style == null)
        {
            GUISkin skin = Resources.Load<GUISkin>("New GUISkin");
            m_Style = skin.label;
        }
        GUILayout.Label("Hello World.", m_Style);
    }
    //private void OnFocus()
    //{
    //    Debug.Log("OnFocus");
    //}
    //private void OnLostFocus()
    //{
    //    Debug.Log("OnLostFocus");
    //}
    //private void OnHierarchyChange()
    //{
    //    Debug.Log("OnHierarchyChang");
    //}
    //private void OnInspectorUpdate()
    //{
    //    Debug.Log("OnInspecotrUpdate");
    //}
    //private void OnProjectChange()
    //{
    //    Debug.Log("OnProjectChang");
    //}
    //private void OnSelectionChange()
    //{
    //    Debug.Log("OnSelectionChang");
    //}
    //private void OnValidate()
    //{
    //    Debug.Log("OnValidate");
    //}
    //private void OnDisable()
    //{
    //    Debug.Log("OnDisable");
    //}
    //private void OnDestroy()
    //{
    //    Debug.Log("OnDestroy");
    //}
}

public class ExamplePopupWindowContent : PopupWindowContent
{
    //窗口尺寸
    private Vector2 windowSize;
    //滚动值
    private Vector2 scroll;

    public ExamplePopupWindowContent(Vector2 windowSize)
    {
        this.windowSize = windowSize;
    }
    public override Vector2 GetWindowSize()
    {
        return windowSize;
    }
    public override void OnOpen()
    {
        Debug.Log("打开示例弹出窗口");
    }
    public override void OnClose()
    {
        Debug.Log("关闭示例弹出窗口");
    }
    public override void OnGUI(Rect rect)
    {
        scroll = EditorGUILayout.BeginScrollView(scroll);
        GUILayout.BeginHorizontal();
        GUILayout.Toggle(false, "Toggle1");
        GUILayout.Toggle(true, "Toggle2");
        GUILayout.Toggle(true, "Toggle3");
        GUILayout.EndHorizontal();
        GUILayout.Button("Button1");
        GUILayout.Button("Button2");
        GUILayout.Button("Button3");
        EditorGUILayout.EndScrollView();
    }
}