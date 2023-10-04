using UnityEngine;
using UnityEditor;

namespace Export.Attribute
{
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyAttributeDrawer : PropertyDrawer
    {
        #region UNITY_EDITOR
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginDisabledGroup(true);

            // 检查是否也有 TextArea 属性
            var textArea = this.fieldInfo.GetCustomAttributes(typeof(TextAreaAttribute), false);
            if (textArea != null && textArea.Length > 0)
            {
                // 如果有 TextArea 属性，使用 TextAreaField 渲染属性
                string value = property.stringValue;
                value = EditorGUI.TextArea(position, value, EditorStyles.textArea);
            }
            else
            {
                // 否则，正常渲染属性
                EditorGUI.PropertyField(position, property, label, true);
            }

            EditorGUI.EndDisabledGroup();
        }
        #endregion
    } 
}