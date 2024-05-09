using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AI.Editor
{
    /// <summary>
    /// AIBrain.PatrolArea editor.
    /// </summary>
    [CustomPropertyDrawer(typeof(AIBrain.PatrolArea))]
    public class PatrolAreaDrawer : PropertyDrawer
    {
        private readonly string[] popupOptions = { "Circle", "Rectangle" };

        public GUIStyle popupStyle;
        public int propertyHeight = 20;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (popupStyle == null)
            {
                popupStyle = new GUIStyle(GUI.skin.GetStyle("PaneOptions"));
                popupStyle.imagePosition = ImagePosition.ImageOnly;
            }

            label = EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, label);

            EditorGUI.BeginChangeCheck();

            SerializedProperty useCircle = property.FindPropertyRelative("useCircle");
            SerializedProperty center = property.FindPropertyRelative("center");

            SerializedProperty radius = property.FindPropertyRelative("radius");
            SerializedProperty rect = property.FindPropertyRelative("rect");

            // calculate the rect position
            Rect buttonRect = new Rect(position);
            buttonRect.y += 20;
            buttonRect.yMin += popupStyle.margin.top;
            buttonRect.width = popupStyle.fixedWidth + popupStyle.margin.right;
            position.xMin = buttonRect.xMax;

            // store old indent value
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            int result = EditorGUI.Popup(buttonRect, useCircle.boolValue ? 0 : 1, popupOptions, popupStyle);

            useCircle.boolValue = result == 0;

            EditorGUI.PropertyField(position, center, GUIContent.none);
            position.y += 20;
            position.height *= .5f;
            EditorGUI.PropertyField(position, useCircle.boolValue ? radius : rect, GUIContent.none);

            // apply any changes we made in the inspector to the values they reprisent
            if (EditorGUI.EndChangeCheck())
            {
                property.serializedObject.ApplyModifiedProperties();
            }
            // move the indent back
            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) + propertyHeight;
        }
    }
}
