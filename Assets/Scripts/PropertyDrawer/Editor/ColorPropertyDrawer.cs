using System;
using System.Globalization;

using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Color))]
public class ColorPropertyDrawer : PropertyDrawer
{
    private const float spacing = 5f;
    private const float hexWidth = 60f;
    private const float alphaWidth = 32f;

    public override void OnGUI(Rect position, 
        SerializedProperty property, GUIContent label)
    {
        label = EditorGUI.BeginProperty(
            position, label, property);
        position = EditorGUI.PrefixLabel(position,
            GUIUtility.GetControlID(FocusType.Passive), label);

        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;
        float colorWidth = position.width - hexWidth
            - spacing - alphaWidth - spacing;

        Color newColor = EditorGUI.ColorField(new Rect(
            position.x, position.y, colorWidth, position.height), 
                property.colorValue);
        if (!newColor.Equals(property.colorValue))
            property.colorValue = newColor;

        //16进制颜色值字符串
        string hex = EditorGUI.TextField(new Rect(position.x + colorWidth 
            + spacing, position.y, hexWidth, position.height),
                ColorUtility.ToHtmlStringRGB(property.colorValue));
        try
        {
            newColor = FromHex(hex, property.colorValue.a);
            if (!newColor.Equals(property.colorValue))
                property.colorValue = newColor;
        }
        catch(Exception e)
        {
            Debug.LogError(e);
        }

        //颜色Alpha值
        float newAlpha = EditorGUI.Slider(new Rect(position.x + colorWidth 
            + hexWidth + (spacing * 2f), position.y, alphaWidth,
            position.height), property.colorValue.a, 0f, 1f);
        if (!newAlpha.Equals(property.colorValue.a))
            property.colorValue = new Color(property.colorValue.r,
                property.colorValue.g, property.colorValue.b, newAlpha);

        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }

    //16进制颜色值字符串转Color值
    private static Color FromHex(string hexValue, float alpha)
    {
        if (string.IsNullOrEmpty(hexValue)) 
            return Color.clear;
        if (hexValue[0] == '#') 
            hexValue = hexValue.TrimStart('#');
        if (hexValue.Length > 6) 
            hexValue = hexValue.Remove(6, hexValue.Length - 6);
        int value = int.Parse(hexValue, NumberStyles.HexNumber);
        int r = value >> 16 & 255;
        int g = value >> 8 & 255;
        int b = value & 255;
        return new Color(r / 255f, g / 255f, b / 255f, alpha);
    }
}