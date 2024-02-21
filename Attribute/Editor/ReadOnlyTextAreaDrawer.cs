using Export.Attribute;
using UnityEditor;
using UnityEngine;

namespace Export.Attribute.Editor
{
    [CustomPropertyDrawer(typeof(ReadOnlyTextAreaAttribute))]
    class ReadOnlyTextAreaDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attribute = (ReadOnlyTextAreaAttribute)this.attribute;

            // 计算文本区域的高度
            float textHeight = EditorStyles.textArea.CalcHeight(new GUIContent(property.stringValue), position.width);

            // 计算标签的高度
            float labelHeight = EditorGUIUtility.singleLineHeight;

            // 合并高度
            float totalHeight = textHeight + labelHeight;

            EditorGUI.BeginDisabledGroup(true);  // 禁用 GUI，使文本区域为只读

            // 绘制变量名
            EditorGUI.LabelField(new Rect(position.x, position.y, position.width, labelHeight), label);

            // 绘制只读文本区域
            EditorGUI.TextArea(new Rect(position.x, position.y + labelHeight, position.width, textHeight), property.stringValue, EditorStyles.textArea);

            EditorGUI.EndDisabledGroup();  // 重新启用 GUI
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var attribute = (ReadOnlyTextAreaAttribute)this.attribute;
            float textHeight = EditorStyles.textArea.CalcHeight(new GUIContent(property.stringValue), EditorGUIUtility.currentViewWidth);
            float labelHeight = EditorGUIUtility.singleLineHeight;
            return textHeight + labelHeight;
        }
    }
}
