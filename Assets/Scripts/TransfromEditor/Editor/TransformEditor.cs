using System;
using System.Text;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Transform))]
public class TransformEditor : Editor
{
    private Editor instance;

    private void OnEnable()
    {
        //获取TransformInspector类型
        Type editorType = Assembly.GetAssembly(typeof(Editor)).GetTypes()
            .FirstOrDefault(m => m.Name == "TransformInspector");
        //创建编辑器实例
        instance = CreateEditor(targets, editorType);
    }
    private void OnDisable()
    {
        //销毁编辑器实例
        if (instance != null)
            DestroyImmediate(instance);
    }

    public override void OnInspectorGUI()
    {
        instance.OnInspectorGUI();
        GUILayout.Space(10f);
        if (GUILayout.Button("Copy Full Path"))
        {
            List<Transform> tfs = new List<Transform>();
            Transform tf = target as Transform;
            tfs.Add(tf);
            while (tf.parent)
            {
                tf = tf.parent;
                tfs.Add(tf);
            }
            StringBuilder sb = new StringBuilder();
            sb.Append(tfs[tfs.Count - 1].name);
            for (int i = tfs.Count - 2; i >= 0; i--)
            {
                sb.Append("/" + tfs[i].name);
            }
            GUIUtility.systemCopyBuffer = sb.ToString();
        }
    }
}