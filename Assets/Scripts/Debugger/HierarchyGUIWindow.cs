using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class HierarchyGUIWindow : MonoBehaviour
{
    private Rect expandRect;
    private Rect retractRect;
    private Rect dragableRect;
    private bool isExpand;
    //层级列表
    private List<HierarchyGUIWindowItem> list;
    //滚动视图的滚动值
    private Vector2 scroll;
    //当前选中的物体
    [HideInInspector] public GameObject currentSelected;

    private void OnEnable()
    {
        list = new List<HierarchyGUIWindowItem>();
        CollectRoots();

        expandRect = new Rect(0f, 0f, 300f, 500f);
        retractRect = new Rect(0f, 0f, 100f, 60f);
        dragableRect = new Rect(0f, 0f, 300f, 20f);
    }
    private void OnDisable()
    {
        list.Clear();
        list = null;
    }

    //收集根级物体
    private void CollectRoots()
    {
        list.Clear();
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            var scene = SceneManager.GetSceneAt(i);
            if (!scene.isLoaded) continue;

            var roots = scene.GetRootGameObjects();
            for (int j = 0; j < roots.Length; j++)
            {
                CollectChildrens(roots[j].transform, list);
            }
        }
    }
    //收集子物体
    private void CollectChildrens(Transform transform,
        List<HierarchyGUIWindowItem> list)
    {
        var item = new HierarchyGUIWindowItem(transform, this);
        list.Add(item);
        for (int i = 0; i < transform.childCount; i++)
        {
            CollectChildrens(transform.GetChild(i),
                item.childrens);
        }
    }

    private void OnGUI()
    {
        if (isExpand)
        {
            expandRect = GUI.Window(1, expandRect, 
                OnExpandGUI, "Hierarchy");
            //限制窗口拖动范围
            expandRect.x = Mathf.Clamp(expandRect.x, 
                0f, Screen.width - 300f);
            expandRect.y = Mathf.Clamp(expandRect.y,
                0f, Screen.height - 500f);
            dragableRect = new Rect(0f, 0f, 300f, 20f);
        }
        else
        {
            retractRect = GUI.Window(1, retractRect,
                OnRetractGUI, "Hierarchy");
            //限制窗口拖动范围
            retractRect.x = Mathf.Clamp(retractRect.x, 
                0f, Screen.width - 100f);
            retractRect.y = Mathf.Clamp(retractRect.y,
                0f, Screen.height - 60f);
            dragableRect = new Rect(0f, 0f, 100f, 20f);
        }
    }
    private void OnExpandGUI(int windowId)
    {
        GUI.DragWindow(dragableRect);
        //关闭窗口
        if (GUILayout.Button("Close", GUILayout.Height(20f)))
            isExpand = false;

        //重绘按钮
        if (GUILayout.Button("Repaint"))
        {
            CollectRoots();
            return;
        }
        scroll = GUILayout.BeginScrollView(scroll);
        for (int i = 0; i < list.Count; i++)
        {
            list[i].Draw();
        }
        GUILayout.EndScrollView();
    }
    private void OnRetractGUI(int windowId)
    {
        GUI.DragWindow(dragableRect);
        //打开窗口
        if (GUILayout.Button("Open", GUILayout.Height(30f)))
            isExpand = true;
    }
}