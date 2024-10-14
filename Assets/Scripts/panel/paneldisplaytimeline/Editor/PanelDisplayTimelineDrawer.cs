using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

[CustomPropertyDrawer(typeof(PanelDisplayTimelineBehaviour))]
public class PanelDisplayTimelineDrawer : PropertyDrawer
{
    public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
    {
        int fieldCount = 2;
        return fieldCount * EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty shouldOpenProp = property.FindPropertyRelative("shouldOpen");
        SerializedProperty shouldCloseProp = property.FindPropertyRelative("shouldClose");

        Rect singleFieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(singleFieldRect, shouldOpenProp);

        singleFieldRect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(singleFieldRect, shouldCloseProp);
    }
}
