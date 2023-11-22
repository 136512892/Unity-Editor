using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Example))]
public class ExampleEditor : Editor
{
    private bool boolValue1;
    private bool boolValue2;
    private string stringValue = "Hello World.";
    private float floatValue = 10f;
    private int intValue = 5;
    private long longValue = 1;
    private string passwordValue = "123456";
    private Vector2 vector2Value = Vector2.zero;
    private Vector3 vector3Value = Vector3.zero;
    private Vector4 vector4Value = Vector4.zero;
    public enum ExampleEnum
    {
        Enum1,
        Enum2,
        Enum3
    }
    private ExampleEnum enumValue = ExampleEnum.Enum1;
    private bool foldout1;
    private bool foldout2;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Label();
        Button();
        Toggle();
        InputField();
        Dropdown();
        Slider();
        Foldout();
        Layout();
        LayoutOption();
        Space();
    }

    private void Label()
    {
        GUILayout.Label("Hello world.", EditorStyles.label);
        GUILayout.Label("Hello world.", EditorStyles.miniLabel);
        GUILayout.Label("Hello world.", EditorStyles.largeLabel);
        GUILayout.Label("Hello world.", EditorStyles.boldLabel);
        GUILayout.Label("Hello world.", EditorStyles.miniBoldLabel);
        GUILayout.Label("Hello world.",
            EditorStyles.centeredGreyMiniLabel);
        GUILayout.Label("Hello world.",
            EditorStyles.wordWrappedMiniLabel, GUILayout.Width(50f));
        GUILayout.Label("Hello world.", EditorStyles.wordWrappedLabel);
        GUILayout.Label("Hello world.", EditorStyles.linkLabel);
        GUILayout.Label("Hello world.", EditorStyles.whiteLabel);
        GUILayout.Label("Hello world.", EditorStyles.whiteMiniLabel);
        GUILayout.Label("Hello world.", EditorStyles.whiteLargeLabel);
        GUILayout.Label("Hello world.", EditorStyles.whiteBoldLabel);
        //自定义字体样式：右对齐、字号为20
        GUILayout.Label("Hello world.", new GUIStyle()
        {
            alignment = TextAnchor.LowerRight,
            fontSize = 20
        });
    }
    private void Button()
    {
        GUILayout.Button("Button1");
        GUILayout.Button("Button2", EditorStyles.miniButton);
        GUILayout.Button("Button3", EditorStyles.radioButton);
        GUILayout.Button("Button4", EditorStyles.toolbarButton);
        //水平方向布局
        GUILayout.BeginHorizontal();
        GUILayout.Button("Button5", EditorStyles.miniButtonLeft);
        GUILayout.Button("Button6", EditorStyles.miniButtonMid);
        GUILayout.Button("Button7", EditorStyles.miniButtonMid);
        GUILayout.Button("Button8", EditorStyles.miniButtonRight);
        GUILayout.EndHorizontal();
    }
    private void Toggle()
    {
        boolValue1 = GUILayout.Toggle(boolValue1, "Toggle1");
        boolValue2 = EditorGUILayout.Toggle("Toggle2", boolValue2);
    }
    private void InputField()
    {
        stringValue = EditorGUILayout.TextField("StringValue", stringValue);
        floatValue = EditorGUILayout.FloatField("FloatValue", floatValue);
        intValue = EditorGUILayout.IntField("IntValue", intValue);
        longValue = EditorGUILayout.LongField("LongValue", longValue);
        passwordValue = EditorGUILayout.PasswordField(
            "PasswordValue", passwordValue);
        vector2Value = EditorGUILayout.Vector2Field(
            "Vector2Value", vector2Value);
        vector3Value = EditorGUILayout.Vector3Field(
            "Vector3Value", vector3Value);
        vector4Value = EditorGUILayout.Vector4Field(
            "Vector4Value", vector4Value);
    }
    private void Dropdown()
    {
        enumValue = (ExampleEnum)EditorGUILayout.EnumPopup(
            "EnumValue", enumValue);
        Selection.activeGameObject.tag = EditorGUILayout.TagField(
            "TagValue", Selection.activeGameObject.tag);
        Selection.activeGameObject.layer = EditorGUILayout.LayerField(
            "LayerValue", Selection.activeGameObject.layer);
    }
    private void Slider()
    {
        intValue = EditorGUILayout.IntSlider("IntValue",
            intValue, 0, 5); //取值范围为0～5
        floatValue = EditorGUILayout.Slider("FloatValue",
            floatValue, 0f, 10f); //取值范围为0～10
    }
    private void Foldout()
    {
        foldout1 = EditorGUILayout.Foldout(foldout1, "折叠栏1", true);
        if (foldout1)
        {
            GUILayout.Label("Hello world.", EditorStyles.miniLabel);
            GUILayout.Label("Hello world.", EditorStyles.boldLabel);
            GUILayout.Label("Hello world.", EditorStyles.largeLabel);
        }
        foldout2 = EditorGUILayout.Foldout(foldout2, "折叠栏2", true);
        if (foldout2)
        {
            GUILayout.Button("Button1");
            GUILayout.Button("Button2");
            GUILayout.Button("Button3");
        }
    }
    private void Layout()
    {
        //水平布局中嵌套两个垂直布局
        GUILayout.BeginHorizontal();
        //第一个垂直布局
        GUILayout.BeginVertical();
        GUILayout.Button("Button1");
        GUILayout.Button("Button2");
        GUILayout.EndVertical();
        //第二个垂直布局
        GUILayout.BeginVertical();
        GUILayout.Button("Button3");
        GUILayout.Button("Button4");
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
    }
    private void LayoutOption()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Button("Button1",
            GUILayout.Width(50f));
        GUILayout.Button("Button2",
            GUILayout.Width(150f), GUILayout.Height(30f));
        GUILayout.Button("Button3",
            GUILayout.Width(200f), GUILayout.Height(40f));
        GUILayout.EndHorizontal();
    }
    private void Space()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Button("Button1", GUILayout.Width(80f));
        //固定间隔50个像素
        GUILayout.Space(50f);
        GUILayout.Button("Button2", GUILayout.Width(80f));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Button("Button1", GUILayout.Width(80f));
        //灵活调整间隙（Button1在最左侧 Button2在最右侧）
        GUILayout.FlexibleSpace();
        GUILayout.Button("Button2", GUILayout.Width(80f));
        GUILayout.EndHorizontal();
    }
}