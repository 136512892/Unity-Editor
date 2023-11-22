using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Sprite))]
public class SpritePropertyDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(
        SerializedProperty property, GUIContent label)
    {
        return 110f;
    }

    public override void OnGUI(Rect position,
        SerializedProperty property, GUIContent label)
    {
        label = EditorGUI.BeginProperty(
            position, label, property);
        position = EditorGUI.PrefixLabel(position,
            GUIUtility.GetControlID(FocusType.Passive), label);

        EditorGUI.ObjectField(new Rect(position.x, position.y, 100f, 100f),
            property, typeof(Sprite), GUIContent.none);

        if (property.objectReferenceValue != null)
        {
            Rect spriteNameRect = new Rect(position.x + 105f,
                position.y + 35f,position.width - 105f, position.height);
            EditorGUI.LabelField(spriteNameRect,
                property.objectReferenceValue.name);
        }
        EditorGUI.EndProperty();
    }
}