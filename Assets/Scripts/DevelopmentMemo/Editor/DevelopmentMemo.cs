using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;
using UnityEditor;

public class DevelopmentMemo : EditorWindow
{
    [MenuItem("Example/Development Memo")]
    public static void Open()
    {
        GetWindow<DevelopmentMemo>().Show();
    }

    //缓存文件的路径
    private string filePath;
    //数据类
    [SerializeField] private DevelopmentMemoData data;
    //搜索的内容
    private string searchContent = string.Empty;
    //列表的宽度
    private float listRectWidth = 280f;
    //左右两侧分割线区域
    private Rect splitterRect;
    //是否正在拖拽分割线
    private bool isDragging;
    //列表滚动值
    private Vector2 listScroll;
    //当前选中项
    private DevelopmentMemoItem currentItem;
    //详情滚动值
    private Vector2 detailScroll;
    private Rect dateRect;

    private void OnEnable()
    {
        //缓存文件的路径
        filePath = Path.GetFullPath(".").Replace("\\", "/")
            + "/Library/DevelopmentMemo.dat";
        //判断是否有缓存文件
        if (File.Exists(filePath))
        {
            //文件流打开缓存文件
            using (FileStream fs = File.Open(filePath, FileMode.Open))
            {
                //二进制反序列化
                BinaryFormatter bf = new BinaryFormatter();
                var deserialize = bf.Deserialize(fs);
                if (deserialize != null)
                    data = deserialize as DevelopmentMemoData;
                //反序列化失败
                if (data == null)
                {
                    //删除无效数据文件
                    File.Delete(filePath);
                    data = new DevelopmentMemoData();
                }
                //默认按标题排序
                else data.list = data.list.OrderBy(m => m.title).ToList();
            }
        }
        //当前没有缓存 
        else data = new DevelopmentMemoData();
    }

    private void OnDisable()
    {
        try
        {
            //数据写入缓存
            using (FileStream fs = File.Create(filePath))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, data);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    private void OnGUI()
    {
        OnTopGUI();
        OnBodyGUI();
    }

    //顶部GUI
    private void OnTopGUI()
    {
        GUILayout.BeginHorizontal(EditorStyles.toolbar);
        //排序按钮
        GUI.enabled = data != null && data.list.Count > 0;
        if (GUILayout.Button("Sort", EditorStyles.toolbarDropDown, 
            GUILayout.Width(50f)))
        {
            GenericMenu gm = new GenericMenu();
            gm.AddItem(new GUIContent("Title ↓"), false, 
                () => data.list = data.list
                    .OrderBy(m => m.title).ToList());
            gm.AddItem(new GUIContent("Title ↑"), false, 
                () => data.list = data.list
                    .OrderByDescending(m => m.title).ToList());
            gm.AddItem(new GUIContent("Created Time ↓"), false,
                () => data.list = data.list
                    .OrderBy(m => m.createdTime).ToList());
            gm.AddItem(new GUIContent("Created Time ↑"), false, 
                () => data.list = data.list
                    .OrderByDescending(m => m.createdTime).ToList());
            gm.ShowAsContext();
        }
        GUI.enabled = true;
        GUILayout.Space(5f);
        //搜索框
        searchContent = GUILayout.TextField(searchContent, 
            EditorStyles.toolbarSearchField);
        //当点击鼠标且鼠标位置不在输入框中时 取消控件的聚焦
        if (Event.current.type == EventType.MouseDown 
            && !GUILayoutUtility.GetLastRect().Contains(
                Event.current.mousePosition))
        {
            GUI.FocusControl(null);
            Repaint();
        }
        GUILayout.EndHorizontal();
    }

    private void OnBodyGUI()
    {
        //左侧列表
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical(GUILayout.Width(listRectWidth));
        OnLeftGUI();
        GUILayout.EndVertical();
        //分割线
        GUILayout.BeginVertical(GUILayout.ExpandHeight(true), 
            GUILayout.MaxWidth(5f));
        GUILayout.Box(GUIContent.none, "EyeDropperVerticalLine", 
            GUILayout.ExpandHeight(true));
        GUILayout.EndVertical();
        splitterRect = GUILayoutUtility.GetLastRect();
        //右侧详情
        GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
        OnRightGUI();
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

        if (Event.current != null)
        {
            //光标
            EditorGUIUtility.AddCursorRect(splitterRect,
                MouseCursor.ResizeHorizontal);
            switch (Event.current.rawType)
            {
                //鼠标按下时判断是否为分割线的区域
                case EventType.MouseDown:
                    isDragging = splitterRect.Contains(
                        Event.current.mousePosition);
                    break;
                //拖拽分割线的过程中根据拖拽偏移量调整左侧列表宽度
                case EventType.MouseDrag:
                    if (isDragging)
                    {
                        listRectWidth += Event.current.delta.x;
                        listRectWidth = Mathf.Clamp(listRectWidth,
                            position.width * .3f, position.width * .8f);
                        Repaint();
                    }
                    break;
                //鼠标抬起结束拖拽
                case EventType.MouseUp:
                    if (isDragging)
                        isDragging = false;
                    break;
            }
        }
    }
    //左侧列表GUI
    private void OnLeftGUI()
    {
        //列表滚动试图
        listScroll = EditorGUILayout.BeginScrollView(listScroll);
        for (int i = 0; i < data.list.Count; i++)
        {
            var item = data.list[i];
            //判断当前项是否符合检索的内容
            if (!item.title.ToLower().Contains(searchContent.ToLower()))
                continue;
            //当前选中项与其它项使用不同样式
            GUILayout.BeginHorizontal(currentItem == item 
                ? "MeTransitionSelectHead"
                : "ProjectBrowserHeaderBgTop");
            GUILayout.Label(item.title); 
            if (item.isOverdue)
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label(EditorGUIUtility.IconContent(
                    "console.warnicon.sml"));
            }
            GUILayout.EndHorizontal();
            //鼠标点击当前项 进行选中
            if (Event.current.type == EventType.MouseDown 
                && GUILayoutUtility.GetLastRect().Contains(
                    Event.current.mousePosition))
            {
                if (currentItem != item)
                {
                    GUI.FocusControl(null);
                    currentItem = item;
                    Repaint();
                }
            }
        }
        EditorGUILayout.EndScrollView();

        GUILayout.FlexibleSpace();

        GUILayout.Box(GUIContent.none, "EyeDropperHorizontalLine",
            GUILayout.MaxHeight(1f), GUILayout.Width(listRectWidth));
        GUILayout.BeginHorizontal(EditorStyles.toolbar);
        //新建按钮
        if (GUILayout.Button("新建", EditorStyles.toolbarButton))
        {
            var item = new DevelopmentMemoItem()
            {
                title = "New Item",
                createdTime = DateTime.Now,
                estimateCompleteTime = DateTime.Now.AddDays(3)
            };
            data.list.Add(item);
        }
        GUILayout.EndHorizontal();
    }
    //右侧详情GUI
    private void OnRightGUI()
    {
        //当前未选中任何项
        if (currentItem == null) return;

        detailScroll = EditorGUILayout.BeginScrollView(detailScroll);
        //标题
        GUILayout.Label("标题：", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal(EditorStyles.helpBox);
        string newTitle = EditorGUILayout.TextField(
            currentItem.title, EditorStyles.label);
        if (newTitle != currentItem.title)
        {
            //长度限制
            if (newTitle.Length > 0 && newTitle.Length <= 20)
                currentItem.title = newTitle;
        }
        GUILayout.FlexibleSpace();
        GUILayout.Label(string.Format("{0}/{1}", currentItem.title.Length,
            20), EditorStyles.miniBoldLabel);
        GUILayout.EndHorizontal();

        //日期
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label(currentItem.createdTime.ToString(),
            EditorStyles.miniBoldLabel);
        GUILayout.EndHorizontal();

        //内容
        GUILayout.Label("内容：", EditorStyles.boldLabel);
        currentItem.content = EditorGUILayout.TextArea(currentItem.content,
            GUILayout.MaxWidth(position.width - listRectWidth - 15f),
            GUILayout.MinHeight(200f));

        EditorGUILayout.Space();
        EditorGUI.BeginChangeCheck();
        //是否为待办
        currentItem.todo = GUILayout.Toggle(currentItem.todo, "待办");
        if (EditorGUI.EndChangeCheck())
        {
            if (currentItem.todo)
                currentItem.OverdueCal();
            else currentItem.isOverdue = false;

        }
        EditorGUILayout.Space();

        if (currentItem.todo)
        {
            //当前状态
            GUILayout.Label("当前状态：", EditorStyles.boldLabel);
            if (GUILayout.Button(currentItem.isCompleted 
                ? "已完成" : "未完成", "DropDownButton"))
            {
                GenericMenu gm = new GenericMenu();
                gm.AddItem(new GUIContent("未完成"), !currentItem.isCompleted,
                    () =>
                    {
                        currentItem.isCompleted = false;
                        currentItem.OverdueCal();
                    });
                gm.AddItem(new GUIContent("已完成"), currentItem.isCompleted, 
                    () =>
                    {
                        currentItem.isCompleted = true;
                        currentItem.OverdueCal();
                    });
                gm.ShowAsContext();
            }

            if (!currentItem.isCompleted)
            {
                GUILayout.Label("预计完成日期：",
                    EditorStyles.boldLabel);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(currentItem.estimateCompleteTime
                    .ToString("D"), "DropDownButton"))
                {
                    PopupWindow.Show(dateRect, new DatePopupWindowContent(
                        new Vector2(position.width - listRectWidth - 18f, 200f),
                            currentItem));
                }
                if (Event.current.type == EventType.Repaint)
                    dateRect = GUILayoutUtility.GetLastRect();
                GUILayout.EndHorizontal();
                if (currentItem.isOverdue)
                    EditorGUILayout.HelpBox("已逾期", MessageType.Warning);
            }
        }

        //删除
        EditorGUILayout.Space();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("删除"))
        {
            if (EditorUtility.DisplayDialog("提醒",
                "是否确认删除该项？", "确认", "取消"))
            {
                data.list.Remove(currentItem);
                currentItem = null;
                Repaint();
            }
        }
        GUILayout.EndHorizontal();
        EditorGUILayout.EndScrollView();
    }
}

public class DatePopupWindowContent : PopupWindowContent
{
    //弹出窗口的尺寸
    private Vector2 windowSize;
    //滚动值
    private Vector2 scroll;
    private readonly DevelopmentMemoItem item;

    public DatePopupWindowContent(Vector2 windowSize, 
        DevelopmentMemoItem item)
    {
        this.windowSize = windowSize;
        this.item = item;
    }

    public override Vector2 GetWindowSize()
    {
        return windowSize;
    }

    public override void OnGUI(Rect rect)
    {
        Color cacheColor = GUI.color;
        scroll = EditorGUILayout.BeginScrollView(scroll);
        GUILayout.Label("年", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        int currentYear = DateTime.Now.Year;
        for (int i = 0; i < 3; i++)
        {
            GUI.color = currentYear + i == item.estimateCompleteTime.Year
                ? Color.gray : cacheColor;
            if (GUILayout.Button((currentYear + i).ToString(),
                GUILayout.Width(60f)))
            {
                item.estimateCompleteTime
                    = new DateTime(currentYear + i, 1, 1);
            }
            GUI.color = cacheColor;
        }
        GUILayout.EndHorizontal();

        EditorGUILayout.Space();
        GUILayout.Label("月", EditorStyles.boldLabel);
        int monthCountPerRow = Mathf.RoundToInt(rect.width / 53f);
        for (int i = 0; i < 12; i += monthCountPerRow)
        {
            GUILayout.BeginHorizontal();
            for (int j = 0; j < monthCountPerRow; j++)
            {
                int index = i + j;
                if (index < 12)
                {
                    GUI.color = (index + 1) 
                        == item.estimateCompleteTime.Month 
                        ? Color.gray : cacheColor;
                    if (GUILayout.Button((index + 1).ToString(),
                        GUILayout.Width(50f)))
                    {
                        item.estimateCompleteTime = new DateTime(
                           item.estimateCompleteTime.Year, index + 1, 1);
                        item.OverdueCal();
                    }
                    GUI.color = cacheColor;
                }
            }
            GUILayout.EndHorizontal();
        }

        EditorGUILayout.Space();
        GUILayout.Label("日", EditorStyles.boldLabel);
        int daysCount = DateTime.DaysInMonth(item.estimateCompleteTime.Year,
            item.estimateCompleteTime.Month);
        int dayCountPerRow = Mathf.RoundToInt(rect.width / 43f);
        for (int i = 0; i < daysCount; i += dayCountPerRow)
        {
            GUILayout.BeginHorizontal();
            for (int j = 0; j < dayCountPerRow; j++)
            {
                int index = i + j;
                if (index < daysCount)
                {
                    GUI.color = (index + 1) 
                        == item.estimateCompleteTime.Day
                        ? Color.gray : cacheColor;
                    if (GUILayout.Button((index + 1).ToString(), 
                        GUILayout.Width(40f)))
                    {
                        item.estimateCompleteTime = new DateTime(
                            item.estimateCompleteTime.Year,
                            item.estimateCompleteTime.Month, index + 1);
                        item.OverdueCal();
                    }
                    GUI.color = cacheColor;
                }
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndScrollView();
    }
}

[Serializable]
public class DevelopmentMemoItem
{
    /// <summary>
    /// 标题
    /// </summary>
    public string title;
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime createdTime;
    /// <summary>
    /// 内容
    /// </summary>
    public string content;
    /// <summary>
    /// 是否为待办
    /// </summary>
    public bool todo;
    /// <summary>
    /// 预计完成时间
    /// </summary>
    public DateTime estimateCompleteTime;
    /// <summary>
    /// 是否已经完成
    /// </summary>
    public bool isCompleted;
    /// <summary>
    /// 是否逾期
    /// </summary>
    public bool isOverdue;

    /// <summary>
    /// 计算是否已经逾期
    /// </summary>
    public void OverdueCal()
    {
        isOverdue = !isCompleted && (DateTime.Now
            - estimateCompleteTime).Days > 0;
    }
}
[Serializable]
public class DevelopmentMemoData
{
    public List<DevelopmentMemoItem> list
        = new List<DevelopmentMemoItem>(0);
}