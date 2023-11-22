using UnityEngine;
using System.Collections.Generic;

public class HierarchyGUIWindowItem
{
    private readonly Transform transform;
    private readonly HierarchyGUIWindow window;
    public readonly List<HierarchyGUIWindowItem> childrens;
    private bool expand;
    private int level;

    public HierarchyGUIWindowItem(Transform transform, 
        HierarchyGUIWindow window)
    {
        this.transform = transform;
        this.window = window;
        childrens = new List<HierarchyGUIWindowItem>();
        GetParent(transform);
    }
    private void GetParent(Transform transform)
    {
        Transform parent = transform.parent;
        if (parent != null)
        {
            level++;
            GetParent(parent);
        }
    }

    public void Draw()
    {
        if (transform == null) return;

        GUILayout.BeginHorizontal();
        GUILayout.Space(15f * level);
        if (transform.childCount > 0)
        {
            if (GUILayout.Button(expand ? "▾" : "▸", 
                GUILayout.Width(17.5f), GUILayout.Height(15f)))
            {
                expand = !expand;
            }
        }
        else
        {
            GUILayout.Label(GUIContent.none, GUILayout.Width(17.5f));
        }
        if (GUILayout.Toggle(window.currentSelected 
            == transform.gameObject, transform.name))
        {
            window.currentSelected = transform.gameObject;
        }
        GUILayout.EndHorizontal();
        if (expand)
        {
            for (int i = 0; i < childrens.Count; i++)
            {
                childrens[i].Draw();
            }
        }
    }
}