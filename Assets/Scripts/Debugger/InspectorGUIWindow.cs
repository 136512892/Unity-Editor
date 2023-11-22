using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;

public class InspectorGUIWindow : MonoBehaviour
{
    //层级窗口
    private HierarchyGUIWindow hierarchyGUIWindow;
    private Rect expandRect;
    private Rect retractRect;
    private Rect dragableRect;
    private bool isExpand;
    //当前选中的物体
    private GameObject selected;
    //当前选中物体的组件集合
    private Component[] components;
    private Vector2 listScroll;
    private Vector2 inspectorScroll;
    //当前选中的组件
    private Component currentComponent;
    private Dictionary<string, IComponentGUIInspector> inspectorDic;

    private void Start()
    {
        hierarchyGUIWindow = GetComponent<HierarchyGUIWindow>();
    }

    private void OnEnable()
    {
        inspectorDic = new Dictionary<string, IComponentGUIInspector>();
        var types = GetType().Assembly.GetTypes().Where(
            m => m.IsSubclassOf(typeof(ComponentGUIInspector))).ToArray();
        for (int i = 0; i < types.Length; i++)
        {
            var type = types[i];
            var attributes = type.GetCustomAttributes(false);
            if (attributes.Any(m => m is ComponentGUIInspectorAttribute))
            {
                var target = Array.Find(attributes,
                    m => m is ComponentGUIInspectorAttribute);
                var attribute = target as ComponentGUIInspectorAttribute;
                var instance = Activator.CreateInstance(type);
                inspectorDic.Add(attribute.ComponentType.FullName,
                    instance as IComponentGUIInspector);
            }
        }

        expandRect = new Rect(0f, 80f, 600f, 500f);
        retractRect = new Rect(0f, 80f, 100f, 60f);
        dragableRect = new Rect(0f, 0f, 600f, 20f);
    }
    private void OnDisable()
    {
        components = null;
        currentComponent = null;
        inspectorDic.Clear();
        inspectorDic = null;
    }

    private void OnGUI()
    {
        if (isExpand)
        {
            expandRect = GUI.Window(2, expandRect,
                OnExpandGUI, "Inspector");
            //限制窗口拖动范围
            expandRect.x = Mathf.Clamp(expandRect.x,
                0f, Screen.width - 600f);
            expandRect.y = Mathf.Clamp(expandRect.y,
                0f, Screen.height - 500f);
            dragableRect = new Rect(0f, 0f, 600f, 20f);
        }
        else
        {
            retractRect = GUI.Window(2, retractRect,
                OnRetractGUI, "Inspector");
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

        if (hierarchyGUIWindow.currentSelected == null)
        {
            GUILayout.Label("未选中任何物体");
            return;
        }
        if (selected != hierarchyGUIWindow.currentSelected)
        {
            selected = hierarchyGUIWindow.currentSelected;
            components = selected.GetComponents<Component>();
            currentComponent = components[0];
        }
        GUILayout.BeginHorizontal("Box");
        {
            bool active = GUILayout.Toggle(selected.activeSelf, 
                string.Empty);
            if (active != selected.activeSelf)
            {
                selected.SetActive(active);
            }
            selected.name = GUILayout.TextField(selected.name,
                GUILayout.Width(Screen.width * .1f));
            GUILayout.FlexibleSpace();
            GUILayout.Label(string.Format("Tag:{0}", selected.tag));
            GUILayout.Space(10f);
            GUILayout.Label(string.Format("Layer:{0}", 
                LayerMask.LayerToName(selected.layer)));
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        {
            GUILayout.BeginVertical("Box", GUILayout.ExpandHeight(true),
                GUILayout.Width(Screen.width * .075f));
            OnListGUI();
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box", GUILayout.ExpandHeight(true));
            OnComponentInspector();
            GUILayout.EndVertical();
        }
        GUILayout.EndHorizontal();
    }
    private void OnRetractGUI(int windowId)
    {
        GUI.DragWindow(dragableRect);
        //打开窗口
        if (GUILayout.Button("Open", GUILayout.Height(30f)))
            isExpand = true;
    }

    private void OnListGUI()
    {
        listScroll = GUILayout.BeginScrollView(listScroll);
        for (int i = 0; i < components.Length; i++)
        {
            if (GUILayout.Toggle(components[i] == currentComponent, 
                components[i].GetType().Name))
            {
                currentComponent = components[i];
            }
        }
        GUILayout.EndScrollView();
    }
    private void OnComponentInspector()
    {
        inspectorScroll = GUILayout.BeginScrollView(inspectorScroll);
        string name = currentComponent.GetType().FullName;
        if (inspectorDic.ContainsKey(name))
        {
            inspectorDic[name].Draw(currentComponent);
        }
        else
        {
            GUILayout.Label("暂不支持该类型组件的调试");
        }
        GUILayout.EndScrollView();
    }
}