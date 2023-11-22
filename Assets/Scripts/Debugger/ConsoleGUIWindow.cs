using System;
using UnityEngine;
using System.Collections.Generic;

public class ConsoleGUIWindow : MonoBehaviour
{
    [SerializeField]
    private WorkingType workingType = WorkingType.ALWAYS_OPEN;
    private Rect expandRect;
    private Rect retractRect;
    private Rect dragableRect;
    private bool isExpand;
    private int fps;
    private float lastShowFPSTime;
    private Color fpsColor = Color.white;
    //日志列表
    private readonly List<ConsoleItem> logs = new List<ConsoleItem>();

    //列表滚动视图
    private Vector2 listScroll;
    //详情滚动视图
    private Vector2 detailScroll;
    //普通日志数量
    private int infoCount;
    //告警日志数量
    private int warnCount;
    //错误日志数量
    private int errorCount;
    //是否显示普通日志
    [SerializeField] private bool showInfo = true;
    //是否显示告警日志
    [SerializeField] private bool showWarn = true;
    //是否显示错误日志
    [SerializeField] private bool showError = true;
    //当前选中的日志项
    private ConsoleItem currentSelected;
    //是否显示日志时间
    [SerializeField] private bool showTime = true;
    //最大缓存数量
    [SerializeField] private int maxCacheCount = 100;
    //检索内容
    private string searchContent;

    private void Start()
    {
        switch (workingType)
        {
            case WorkingType.ALWAYS_OPEN: 
                enabled = true; 
                break;
            case WorkingType.ONLY_OPEN_WHEN_DEVELOPMENT_BUILD:
                enabled = Debug.isDebugBuild;
                break;
            case WorkingType.ONLY_OPEN_IN_EDITOR:
                enabled = Application.isEditor; 
                break;
            case WorkingType.ALWAYS_CLOSE: 
                enabled = false; 
                break;
        }
        if (!enabled) return;

        expandRect = new Rect(Screen.width * .7f, 0f, 
            Screen.width * .3f, Screen.height * .5f);
        retractRect = new Rect(Screen.width - 100f, 0f, 100f, 60f);
        dragableRect = new Rect(0, 0, Screen.width * .3f, 20f);
        //事件注册
        Application.logMessageReceived += OnLogMessageReceived;
    }
    private void OnDestroy()
    {
        Application.logMessageReceived -= OnLogMessageReceived;
    }

    private void OnLogMessageReceived(string condition,
        string stackTrace, LogType logType)
    {
        var item = new ConsoleItem(DateTime.Now,
            logType, condition, stackTrace);
        if (logType == LogType.Log) infoCount++;
        else if (logType == LogType.Warning) warnCount++;
        else errorCount++;
        logs.Add(item);
        if (logs.Count > maxCacheCount)
        {
            logs.RemoveAt(0);
        }
    }

    private void OnGUI()
    {
        if (isExpand)
        {
            expandRect = GUI.Window(0, expandRect, OnExpandGUI, "Console");
            //限制范围
            expandRect.x = Mathf.Clamp(expandRect.x, 0, Screen.width * .7f);
            expandRect.y = Mathf.Clamp(expandRect.y, 0, Screen.height * .5f);
            dragableRect = new Rect(0, 0, Screen.width * .3f, 20f);
        }
        else
        {
            retractRect = GUI.Window(0, retractRect, OnRetractGUI, "Console");
            //限制范围
            retractRect.x = Mathf.Clamp(retractRect.x, 0, Screen.width - 100f);
            retractRect.y = Mathf.Clamp(retractRect.y, 0, Screen.height - 60f);
            dragableRect = new Rect(0, 0, 100f, 20f);
        }
        //FPS计算
        if (Time.realtimeSinceStartup - lastShowFPSTime >= 1)
        {
            fps = Mathf.RoundToInt(1f / Time.deltaTime);
            lastShowFPSTime = Time.realtimeSinceStartup;
            fpsColor = errorCount > 0 ? Color.red
                : warnCount > 0 ? Color.yellow : Color.white;
        }
    }
    //窗口为收起状态
    private void OnRetractGUI(int windowId)
    {
        GUI.DragWindow(dragableRect);
        GUI.contentColor = fpsColor;
        if (GUILayout.Button(string.Format("FPS：{0}", fps),
            GUILayout.Height(30f)))
            isExpand = true;
        GUI.contentColor = Color.white;
    }
    //窗口为展开状态
    private void OnExpandGUI(int windowId)
    {
        GUI.DragWindow(dragableRect);
        GUI.contentColor = fpsColor;
        if (GUILayout.Button(string.Format("FPS：{0}", fps),
            GUILayout.Height(20f)))
            isExpand = false;
        OnTopGUI();
        OnListGUI();
        OnDetailGUI();
    }
    private void OnTopGUI()
    {
        GUILayout.BeginHorizontal();
        //清空所有日志
        if (GUILayout.Button("Clear", GUILayout.Width(50f)))
        {
            logs.Clear();
            infoCount = 0;
            warnCount = 0;
            errorCount = 0;
            currentSelected = null;
        }
        //是否显示日志时间
        showTime = GUILayout.Toggle(showTime, "ShowTime",
            GUILayout.Width(80f));

        //检索输入框
        searchContent = GUILayout.TextField(searchContent,
            GUILayout.ExpandWidth(true));

        GUI.contentColor = showInfo ? Color.white : Color.grey;
        showInfo = GUILayout.Toggle(showInfo, string.Format(
            "Info [{0}]", infoCount), GUILayout.Width(60f));
        GUI.contentColor = showWarn ? Color.white : Color.grey;
        showWarn = GUILayout.Toggle(showWarn, string.Format(
            "Warn [{0}]", warnCount), GUILayout.Width(65f));
        GUI.contentColor = showError ? Color.white : Color.grey;
        showError = GUILayout.Toggle(showError, string.Format(
            "Error [{0}]", errorCount), GUILayout.Width(65f));
        GUI.contentColor = Color.white;
        GUILayout.EndHorizontal();
    }
    private void OnListGUI()
    {
        GUILayout.BeginVertical("Box",
            GUILayout.Height(Screen.height * .3f));
        //滚动视图
        listScroll = GUILayout.BeginScrollView(listScroll);
        for (int i = logs.Count - 1; i >= 0; i--)
        {
            var temp = logs[i];
            //是否符合检索内容
            if (!string.IsNullOrEmpty(searchContent) && !temp.message
                .ToLower().Contains(searchContent.ToLower())) continue;
            bool show = false;
            switch (temp.type)
            {
                case LogType.Log: 
                    if (showInfo) show = true;
                    break;
                case LogType.Warning:
                    if (showWarn) show = true; 
                    GUI.contentColor = Color.yellow;
                    break;
                case LogType.Error:
                case LogType.Assert:
                case LogType.Exception: 
                    if (showError) show = true; 
                    GUI.contentColor = Color.red; 
                    break;
            }
            if (show)
            {
                if (GUILayout.Toggle(currentSelected == temp, 
                    showTime ? temp.brief : temp.message))
                    currentSelected = temp;
            }
            GUI.contentColor = Color.white;
        }
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }
    private void OnDetailGUI()
    {
        GUILayout.BeginVertical("Box", GUILayout.ExpandHeight(true));
        detailScroll = GUILayout.BeginScrollView(detailScroll);
        if (currentSelected != null)
        {
            GUILayout.Label(currentSelected.detail);
        }
        GUILayout.EndScrollView();
        GUILayout.FlexibleSpace();
        GUI.enabled = currentSelected != null;
        //点击按钮时将日志详情复制到系统粘贴板中
        if (GUILayout.Button("Copy", GUILayout.Height(20f)))
        {
            GUIUtility.systemCopyBuffer = currentSelected.detail;
        }
        GUILayout.EndVertical();
    }
}

public enum WorkingType
{
    /// <summary>
    /// 始终打开
    /// </summary>
    ALWAYS_OPEN,
    /// <summary>
    /// 仅在Development Build模式下打开
    /// </summary>
    ONLY_OPEN_WHEN_DEVELOPMENT_BUILD,
    /// <summary>
    /// 仅在Editor中打开
    /// </summary>
    ONLY_OPEN_IN_EDITOR,
    /// <summary>
    /// 始终关闭
    /// </summary>
    ALWAYS_CLOSE
}

public class ConsoleItem
{
    public LogType type;
    public DateTime time;
    public string message;
    public string stackTrace;
    public string brief;
    public string detail;

    public ConsoleItem(DateTime time, LogType type,
        string message, string stackTrace)
    {
        this.type = type;
        this.time = time;
        this.message = message;
        this.stackTrace = stackTrace;
        brief = string.Format("[{0}] {1}",
            time, message);
        detail = string.Format("[{0}] {1}\r\n{2}",
            time, message, stackTrace);
    }
}