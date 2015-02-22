using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Kensai.Util {
    [CustomPropertyDrawer(typeof(IntVector2))]
    public class IntVector2Drawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            SerializedProperty x = property.FindPropertyRelative("x");
            SerializedProperty y = property.FindPropertyRelative("y");

            label = EditorGUI.BeginProperty(position, label, property);

            var contentPosition = EditorGUI.PrefixLabel(position, label);
            contentPosition.width = contentPosition.width * 0.5f - 1;
            EditorGUIUtility.labelWidth = 14f;
            EditorGUI.PropertyField(contentPosition, x);
            contentPosition.x += contentPosition.width + 2;
            EditorGUI.PropertyField(contentPosition, y);
            EditorGUI.EndProperty();
        }
    }
}
