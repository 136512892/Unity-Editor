using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;

[CustomEditor(typeof(GUISkinExample))]
public class GUISkinExampleEditor : Editor
{
    private GUIStyle m_Style;
    private Texture axe;
    private AnimBool animBool;

    private void OnEnable()
    {
        //axe = Resources.Load<Texture>("Axe");
        animBool = new AnimBool(false);
        animBool.valueChanged.AddListener(Repaint);
    }
    private void OnDisable()
    {
        animBool.valueChanged.RemoveListener(Repaint);
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        OnAnimExample();
    }
    private void OnAnimExample()
    {
        animBool.target = EditorGUILayout.Foldout(animBool.target,
            "这是一个折叠栏", true);
        //该折叠栏展开或收起时将会有动画过程
        if (EditorGUILayout.BeginFadeGroup(animBool.faded))
        {
            GUILayout.Label("这里是折叠栏里的内容");
            GUILayout.Button("这是一个按钮");
            GUILayout.Toggle(false, "这是一个开关");
        }
        EditorGUILayout.EndFadeGroup();
    }
    private void OnLabelImageExample()
    {
        GUILayout.Label(axe, GUILayout.Height(50f));
        GUILayout.Label(new GUIContent("这是一把斧子", axe), 
            GUILayout.Height(50f));
    }
    private void OnGUIStyleExample()
    {
        if (m_Style == null)
        {
            GUISkin skin = Resources.Load<GUISkin>("New GUISkin");
            m_Style = skin.label;
        }
        GUILayout.Label("Hello World.", m_Style);
        GUILayout.Label("Today is a good day.", m_Style);
    }
}