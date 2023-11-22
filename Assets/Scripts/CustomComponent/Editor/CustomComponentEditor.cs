using UnityEngine;
using UnityEditor;
using System.Reflection;

[CustomEditor(typeof(CustomComponent))]
public class CustomComponentEditor : Editor 
{
    private CustomComponent component;
    private SerializedProperty stringValueProperty;
    private FieldInfo boolValueFieldInfo;
    private SerializedProperty gameObjectProperty;
    private SerializedProperty enumValue;

    private void OnEnable()
    {
        component = target as CustomComponent;
        stringValueProperty = serializedObject.FindProperty("stringValue");
        boolValueFieldInfo = typeof(CustomComponent).GetField(
            "boolValue", BindingFlags.Instance | BindingFlags.NonPublic);
        gameObjectProperty = serializedObject.FindProperty("go");
        enumValue = serializedObject.FindProperty("enumValue");
    }

    public override void OnInspectorGUI()
    {
        SetTransformParentExample();
    }
    private void SetTransformParentExample()
    {
        if (GUILayout.Button("Set As Root"))
        {
            Undo.SetTransformParent(component.transform,
                null, "Change Transform Parent");
        }
    }
    private void DestroyObjectImmediateExample()
    {
        if (GUILayout.Button("Destroy"))
        {
            Undo.DestroyObjectImmediate(component);
        }
    }
    private void RegisterCreatedObjectUndoExample()
    {
        if (GUILayout.Button("Create New GameObject"))
        {
            GameObject go = new GameObject();
            Undo.RegisterCreatedObjectUndo(go, 
                "Create New GameObject");
        }
    }
    private void AddComponentExample()
    {
        if (GUILayout.Button("Add BoxCollider"))
        {
            Undo.AddComponent(component.gameObject,
                typeof(BoxCollider));
        }
    }
    private void RecordObjectExample()
    {
        int newIntValue = EditorGUILayout.IntField(
            "Int Value", component.intValue);
        if (newIntValue != component.intValue)
        {
            Undo.RecordObject(component, "Change Int Value");
            component.intValue = newIntValue;
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(component);
        }
    }

    private void ApplyModificationExample()
    {
        //public修饰的字段 可以直接访问和修改其值
        component.intValue = EditorGUILayout.IntField(
            "Int Value", component.intValue);
        //private修饰的字段 通过序列化属性的方式访问和修改其值
        stringValueProperty.stringValue = EditorGUILayout.TextField(
            "String Value", stringValueProperty.stringValue);
        //private修饰的字段 通过反射的方式访问和修改其值
        boolValueFieldInfo.SetValue(component, EditorGUILayout.Toggle(
            "Bool Value", (bool)boolValueFieldInfo.GetValue(component)));
        EditorGUILayout.PropertyField(gameObjectProperty);
        enumValue.enumValueIndex = EditorGUILayout.Popup(
            "Enum Value", enumValue.enumValueIndex, enumValue.enumNames);

        //有任何控件发生变更
        if (GUI.changed)
        {
            //应用修改
            serializedObject.ApplyModifiedProperties();
            //标记为"脏"
            EditorUtility.SetDirty(component);
        }
    }
    private void ChangeCheckExample()
    {
        //开启一块代码块检测GUI是否变更
        EditorGUI.BeginChangeCheck();
        //public修饰的字段 可以直接访问和修改其值
        component.intValue = EditorGUILayout.IntField(
            "Int Value", component.intValue);
        //如果在代码块中GUI发生了变更 返回值为true
        if (EditorGUI.EndChangeCheck())
        {
            Debug.Log("IntValue发生变更");
        }
        //private修饰的字段 通过序列化属性的方式访问和修改其值
        //接收控件的返回值
        string newStringValue = EditorGUILayout.TextField(
            "String Value", stringValueProperty.stringValue);
        //对比是否一致 不一致则更新
        if (newStringValue != stringValueProperty.stringValue)
        {
            stringValueProperty.stringValue = newStringValue;
            Debug.Log("StringValue发生变更");
        }
        //private修饰的字段 通过反射的方式访问和修改其值
        boolValueFieldInfo.SetValue(component, EditorGUILayout.Toggle(
            "Bool Value", (bool)boolValueFieldInfo.GetValue(component)));
        EditorGUILayout.PropertyField(gameObjectProperty);
        enumValue.enumValueIndex = EditorGUILayout.Popup(
            "Enum Value", enumValue.enumValueIndex, enumValue.enumNames);
        //有任何控件发生变更
        if (GUI.changed)
        {
            Debug.Log("GUI发生了变更");
        }
    }
    private void CustomExample()
    {
        //public修饰的字段 可以直接访问和修改其值
        component.intValue = EditorGUILayout.IntField(
            "Int Value", component.intValue);
        //private修饰的字段 通过序列化属性的方式访问和修改其值
        stringValueProperty.stringValue = EditorGUILayout.TextField(
            "String Value", stringValueProperty.stringValue);
        //private修饰的字段 通过反射的方式访问和修改其值
        boolValueFieldInfo.SetValue(component, EditorGUILayout.Toggle(
            "Bool Value", (bool)boolValueFieldInfo.GetValue(component)));
        EditorGUILayout.PropertyField(gameObjectProperty);
        enumValue.enumValueIndex = EditorGUILayout.Popup(
            "Enum Value", enumValue.enumValueIndex, enumValue.enumNames);
    }

    public override bool HasPreviewGUI()
    {
        return EditorApplication.isPlaying;
    }
    public override GUIContent GetPreviewTitle()
    {
        return new GUIContent("这里是窗口的标题");
    }
    public override void OnPreviewSettings()
    {
        GUILayout.Button("Button1", EditorStyles.toolbarButton);
        GUILayout.Button("Button2", EditorStyles.toolbarButton);
    }
    public override void OnPreviewGUI(Rect r, GUIStyle background)
    {
        GUILayout.Label(string.Format("Int Value: {0}",
            component.intValue));
        GUILayout.Label(string.Format("String Value: {0}",
            stringValueProperty.stringValue));
        GUILayout.Label(string.Format("Bool Value: {0}",
            (bool)boolValueFieldInfo.GetValue(component)));
        GUILayout.Label(string.Format("Go Value: {0}",
            gameObjectProperty.objectReferenceValue));
        GUILayout.Label(string.Format("Enum Value: {0}",
            enumValue.enumNames[enumValue.enumValueIndex]));
    }
}