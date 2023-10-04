using UnityEditor;
using UnityEngine;

namespace Export.Attribute
{
    [CustomPropertyDrawer(typeof(ReadOnlyTextAreaAttribute))]
    class ReadOnlyTextAreaDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attribute = (ReadOnlyTextAreaAttribute)this.attribute;

            EditorGUI.BeginDisabledGroup(true);  // Disable editing

            EditorGUI.BeginChangeCheck();
            string newValue = EditorGUI.TextArea(position, property.stringValue, EditorStyles.textArea);

            if (EditorGUI.EndChangeCheck())
            {
                property.stringValue = newValue;
            }

            EditorGUI.EndDisabledGroup();  // Re-enable GUI
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var attribute = (ReadOnlyTextAreaAttribute)this.attribute;
            return EditorStyles.textArea.CalcHeight(new GUIContent(property.stringValue), EditorGUIUtility.currentViewWidth) * Mathf.Clamp(attribute.maxLines, 1, int.MaxValue);
        }
    }
}
