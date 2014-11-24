using UnityEngine;
using UnityEditor;
using System.Collections;

public class MyTooltipAttribute : PropertyAttribute
{
    public readonly string comment;
    public MyTooltipAttribute(string c)
    {
        comment = c;
    }
}

[CustomPropertyDrawer(typeof(TooltipAttribute))]
public class MyTooltipDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
    {
        var atr = (MyTooltipAttribute)attribute;
        var content = new GUIContent(label.text, atr.comment);
        EditorGUI.PropertyField(position, prop, content);
    }
}