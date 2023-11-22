using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

public class AssetBundleConfigure : EditorWindow
{
    [MenuItem("Example/Resource/AssetBundle Configure")]
    public static void Open()
    {
        GetWindow<AssetBundleConfigure>().Show();
    }

    private Vector2 lScrollPosition, rScrollPosition;
    private Vector2 abDetailScrollPosition;
    private Vector2 assetDetailScrollPosition;
    //分割线宽度
    private const float splitterWidth = 2f;
    //分割线位置
    private float splitterPos;
    private Rect splitterRect;
    //是否正在拖拽分割线
    private bool isDragging;

    //AssetBundle名称集合
    private string[] assetBundleNames;
    //<AssetBundle名称，Assets路径集合>
    private Dictionary<string, string[]> map;
    //当前选中的AssetBundle名称
    private string selectedAssetBundleName;
    //当前选中的Asset路径
    private string selectedAssetPath;

    //检索AssetBundle
    private string searchAssetBundle;
    //检索Asset路径
    private string searchAssetPath;

    private void OnEnable()
    {
        splitterPos = position.width * .5f;
        Init();
    }

    private void OnDisable()
    {
        map = null;
        searchAssetBundle = null;
        selectedAssetBundleName = null;
    }

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        lScrollPosition = GUILayout.BeginScrollView(
            lScrollPosition,
            GUILayout.Width(splitterPos),
            GUILayout.MaxWidth(splitterPos),
            GUILayout.MinWidth(splitterPos));
        OnLeftGUI();
        GUILayout.EndScrollView();

        //分割线
        GUILayout.Box(string.Empty,
            GUILayout.Width(splitterWidth),
            GUILayout.MaxWidth(splitterWidth),
            GUILayout.MinWidth(splitterWidth),
            GUILayout.ExpandHeight(true));
        splitterRect = GUILayoutUtility.GetLastRect();

        rScrollPosition = GUILayout.BeginScrollView(
            rScrollPosition, GUILayout.ExpandWidth(true));
        OnRightGUI();
        GUILayout.EndScrollView();
        GUILayout.EndHorizontal();

        if (Event.current != null)
        {
            //光标
            EditorGUIUtility.AddCursorRect(splitterRect,
                MouseCursor.ResizeHorizontal);
            switch (Event.current.rawType)
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

    private void Init(bool reselect = true)
    {
        if (reselect)
        {
            selectedAssetBundleName = null;
            selectedAssetPath = null;
        }
        //获取所有AssetBundle名称
        assetBundleNames = AssetDatabase.GetAllAssetBundleNames();
        //初始化map字典
        map = new Dictionary<string, string[]>();
        for (int i = 0; i < assetBundleNames.Length; i++)
        {
            map.Add(assetBundleNames[i], AssetDatabase
                .GetAssetPathsFromAssetBundle(assetBundleNames[i]));
        }
    }

    private void OnLeftGUI()
    {
        //刷新 重新加载AssetBundle信息
        if (GUILayout.Button("Refresh"))
        {
            Init();
            Repaint();
        }
        //移除未使用的AssetBundle名称
        if (GUILayout.Button("RemoveUnusedNames"))
        {
            AssetDatabase.RemoveUnusedAssetBundleNames();
            Init();
            Repaint();
        }
        //检索输入框
        searchAssetBundle = GUILayout.TextField(
            searchAssetBundle, EditorStyles.toolbarSearchField);
        Rect lastRect = GUILayoutUtility.GetLastRect();
        //当点击鼠标且鼠标位置不在输入框中时 取消控件的聚焦
        if (Event.current.type == EventType.MouseDown 
            && !lastRect.Contains(Event.current.mousePosition))
        {
            GUI.FocusControl(null);
            Repaint();
        }

        //列表区域
        Rect listRect = new Rect(0f, lastRect.y + 20f, 
            lastRect.width, position.height - lastRect.y - 25f);
        //如果拖拽资产到列表区域，为资产创建AssetBundle
        if (DragObjects2RectCheck(listRect, out Object[] objects))
        {
            bool flag = false;
            for (int i = 0; i < objects.Length; i++)
            {
                if (AssetBundleUtility
                    .CreateAssetBundle4Object(objects[i]))
                    flag = true;
            }
            //创建了新的AssetBundle 刷新
            if (flag)
            {
                Init();
                Repaint();
            }
        }

        if (assetBundleNames.Length == 0) return;
        for (int i = 0; i < assetBundleNames.Length; i++)
        {
            string assetBundleName = assetBundleNames[i];
            if (!string.IsNullOrEmpty(searchAssetBundle) 
                && !assetBundleName.ToLower()
                    .Contains(searchAssetBundle.ToLower()))
                continue;
            GUILayout.BeginHorizontal(selectedAssetBundleName
                == assetBundleName ? "MeTransitionSelectHead" 
                : "ProjectBrowserHeaderBgTop", 
                GUILayout.Height(20f));
            GUILayout.Label(EditorGUIUtility.TrTextContentWithIcon(
                 assetBundleName, "GameObject Icon"), GUILayout.Height(18f));
            GUILayout.EndHorizontal();
            //鼠标点击事件
            if (Event.current.type == EventType.MouseDown 
                && GUILayoutUtility.GetLastRect()
                    .Contains(Event.current.mousePosition))
            {
                //如果是左键点击 选中该项
                if (Event.current.button == 0)
                {
                    selectedAssetBundleName = assetBundleName;
                    Repaint();
                }
                //如果是右键点击 弹出菜单
                if (Event.current.button == 1)
                {
                    GenericMenu gm = new GenericMenu();
                    //删除AssetBundle
                    gm.AddItem(new GUIContent("Delete AssetBundle"), false, () =>
                    {
                        //二次确认弹窗
                        if (EditorUtility.DisplayDialog("提醒", 
                            string.Format("是否确认删除{0}？",
                                assetBundleName), "确认", "取消"))
                        {
                            AssetBundleUtility.DeleteAssetBundleName(
                                assetBundleName);
                            Init();
                            Repaint();
                        }
                    });
                    gm.ShowAsContext();
                }
            }
        }

        GUILayout.FlexibleSpace();
        GUILayout.BeginVertical(GUI.skin.box, GUILayout.Height(100f), 
            GUILayout.ExpandWidth(true));
        string[] dependencies = AssetDatabase.GetAssetBundleDependencies(
            selectedAssetBundleName, true);
        GUILayout.Label("Dependencies:");
        abDetailScrollPosition = GUILayout.BeginScrollView(
            abDetailScrollPosition);
        for (int i = 0; i < dependencies.Length; i++)
        {
            GUILayout.Label(dependencies[i]);
        }
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    private void OnRightGUI()
    {
        //检索输入框
        searchAssetPath = GUILayout.TextField(searchAssetPath, 
            EditorStyles.toolbarSearchField);
        Rect lastRect = GUILayoutUtility.GetLastRect();
        //当点击鼠标且鼠标位置不在输入框中时 取消控件的聚焦
        if (Event.current.type == EventType.MouseDown 
            && !lastRect.Contains(Event.current.mousePosition))
        {
            GUI.FocusControl(null);
            Repaint();
        }
        if (selectedAssetBundleName == null) return;

        //资产拖拽区域
        Rect dragRect = new Rect(lastRect.x, lastRect.y + 20f,
            lastRect.width - 5f, position.height - lastRect.y - 25f);
        //如果将资产拖拽到该区域，为这些资产设置AssetBundle名
        if (DragObjects2RectCheck(dragRect, out Object[] objects))
        {
            for (int i = 0; i < objects.Length; i++)
            {
                string assetPath = AssetDatabase.GetAssetPath(objects[i]);
                AssetImporter importer = AssetImporter.GetAtPath(assetPath);
                if (importer != null)
                    importer.assetBundleName = selectedAssetBundleName;
            }
            Init(false);
            Repaint();
            return;
        }

        //该AssetBundle中的资产路径集合
        string[] assetPaths = map[selectedAssetBundleName];
        if (assetPaths.Length == 0) return;
        for (int i = 0; i < assetPaths.Length; i++)
        {
            string assetPath = assetPaths[i];
            //当前项是否符合检索内容
            if (!string.IsNullOrEmpty(searchAssetPath) 
                && !assetPath.ToLower().Contains(
                    searchAssetPath.ToLower())) 
                continue;
            GUILayout.BeginHorizontal(selectedAssetPath == assetPath 
                ? "MeTransitionSelectHead" 
                : "ProjectBrowserHeaderBgTop", 
                GUILayout.Height(20f));
            Type type = AssetDatabase.GetMainAssetTypeAtPath(assetPath);
            Texture texture = AssetPreview.GetMiniTypeThumbnail(type);
            GUILayout.Label(texture, GUILayout.Width(18f),
                GUILayout.Height(18f));
            GUILayout.Label(assetPaths[i]);
            GUILayout.EndHorizontal();
            //鼠标点击事件
            if (Event.current.type == EventType.MouseDown
                && GUILayoutUtility.GetLastRect()
                    .Contains(Event.current.mousePosition))
            {
                //如果是左键点击 选中该项
                if (Event.current.button == 0)
                {
                    selectedAssetPath = assetPath;
                    Repaint();
                    EditorGUIUtility.PingObject(
                        AssetDatabase.LoadMainAssetAtPath(
                            selectedAssetPath));
                }
                //如果是右键点击 弹出菜单
                else if (Event.current.button == 1)
                {
                    GenericMenu gm = new GenericMenu();
                    //删除
                    gm.AddItem(new GUIContent("Delete"), false, () =>
                    {
                        //根据资产路径获取其资产导入器
                        AssetImporter importer = AssetImporter
                            .GetAtPath(assetPath);
                        //清空AssetBundle名
                        if (importer != null)
                            importer.assetBundleName = null;
                        Init(false);
                        Repaint();
                    });
                    gm.ShowAsContext();
                }
            }
        }

        GUILayout.FlexibleSpace();
        GUILayout.BeginVertical(GUI.skin.box, GUILayout.Height(100f)
            , GUILayout.ExpandWidth(true));
        string[] dependencies = AssetDatabase.GetDependencies(
            selectedAssetPath, true);
        GUILayout.Label("Dependencies:");
        assetDetailScrollPosition = GUILayout.BeginScrollView(
            assetDetailScrollPosition);
        for (int i = 0; i < dependencies.Length; i++)
        {
            GUILayout.Label(dependencies[i]);
        }
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    //是否拖拽资产到矩形区域中
    private bool DragObjects2RectCheck(Rect rect, out Object[] objects)
    {
        objects = null;
        //鼠标是否在矩形区域中
        if (rect.Contains(Event.current.mousePosition))
        {
            switch (Event.current.type)
            {
                case EventType.DragUpdated:
                    //是否拖拽了资产
                    bool containsObjects = DragAndDrop
                        .objectReferences.Any();
                    DragAndDrop.visualMode = containsObjects
                        ? DragAndDropVisualMode.Copy
                        : DragAndDropVisualMode.Rejected;
                    Event.current.Use();
                    Repaint();
                    return false;
                case EventType.DragPerform:
                    //拖拽的资产
                    objects = DragAndDrop.objectReferences;
                    Event.current.Use();
                    Repaint();
                    return true;
            }
        }
        return false;
    }
}