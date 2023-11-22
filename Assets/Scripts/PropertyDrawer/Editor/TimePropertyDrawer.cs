using System;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(TimeAttribute))]
public sealed class TimePropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position,
        SerializedProperty property, GUIContent label)
    {
        if (property.propertyType == SerializedPropertyType.Float)
        {
            property.floatValue = EditorGUI.FloatField(
                new Rect(position.x, position.y,
                    position.width * .6f, position.height), 
                label, property.floatValue);
            EditorGUI.LabelField(new Rect(
                position.x + position.width * .6f, position.y, 
                position.width * .4f, position.height),
                GetTimeFormat(property.floatValue));
        }
        else
        {
            //如果将Time用于float之外的类型 报错提示
            EditorGUI.HelpBox(position, 
                "只支持float类型字段", MessageType.Error);
        }
    }
    //转换为时间格式字符串
    private string GetTimeFormat(float time)
    {
        //取整获得总共的秒数
        int l = Convert.ToInt32(time);
        //小时数是秒数对3600取整
        int hours = l / 3600;
        //分钟数是秒数对3600取余后再对60取整
        int minutes = l % 3600 / 60;
        //最终秒数是对3600取余后再对60取余
        int seconds = l % 3600 % 60; 
        return string.Format("({0:D2}:{1:D2}:{2:D2})",
            hours, minutes, seconds);
    }
}