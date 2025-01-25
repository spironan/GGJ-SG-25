using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(TweenValue))]
public class TweenValueEditor : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label) * 3f;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        Rect typeRect = new Rect(position.x, position.y, position.width, position.height / 3f);
        Rect fromRect = new Rect(position.x, position.y + (position.height / 3f), position.width, position.height / 3f);
        Rect toRect = new Rect(position.x, position.y + (position.height / 3f * 2), position.width, position.height / 3f);

        SerializedProperty typeProperty = property.FindPropertyRelative("type");
        EditorGUI.PropertyField(typeRect, typeProperty);
        TweenType type = (TweenType)typeProperty.enumValueIndex;

        SerializedProperty fromProperty = property.FindPropertyRelative("from");
        SerializedProperty fromEnabledProperty = property.FindPropertyRelative("fromUseCurrent");
        SerializedProperty toProperty = property.FindPropertyRelative("to");
        SerializedProperty toEnabledProperty = property.FindPropertyRelative("toUseCurrent");

        DrawPropertyField(type, fromRect, fromProperty, fromEnabledProperty, "from");
        DrawPropertyField(type, toRect, toProperty, toEnabledProperty, "to");

        EditorGUI.EndProperty();
    }

    private void DrawPropertyField(TweenType type, Rect rect, SerializedProperty property, SerializedProperty enabledProperty, string label)
    {
        rect.width -= rect.height;
        GUI.enabled = !enabledProperty.boolValue;

        switch(type)
        {
            case TweenType.Position:
            case TweenType.Rotation:
            case TweenType.Scale:
                property.vector4Value = EditorGUI.Vector3Field(rect, label, property.vector4Value);
                break;
            case TweenType.Color:
                property.vector4Value = EditorGUI.ColorField(rect, label, property.vector4Value);
                break;
            case TweenType.Alpha:
            case TweenType.Fill:
                float t = EditorGUI.Slider(rect, new GUIContent(label), property.vector4Value.w, 0f, 1f);
                property.vector4Value = new Vector4(1f, 1f, 1f, t);
                break;
        }

        GUI.enabled = true;

        Rect toggleRect = new Rect(rect.x + rect.width - (rect.height * 1.75f), rect.y, rect.height * 2.75f, rect.height);
        enabledProperty.boolValue = !EditorGUI.Toggle(toggleRect, !enabledProperty.boolValue);
    }
}