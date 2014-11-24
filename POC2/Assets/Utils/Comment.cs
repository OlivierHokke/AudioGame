using UnityEditor;
using UnityEngine;

public class Comment
{
    public string comment;
    public Comment(string s)
    {
        comment = s;
    }
}

[CustomPropertyDrawer(typeof(Comment))]
public class CommentDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        
        EditorGUI.PrefixLabel(position, label);
        EditorGUI.PropertyField(position, property.FindPropertyRelative("position"));
    }
}