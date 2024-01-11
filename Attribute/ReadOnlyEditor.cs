//#define UNTIY_EDITOR

using UnityEngine;
using UnityEditor;

namespace Export.Attribute
{
#if UNTIY_EDITOR
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyAttributeDrawer : PropertyDrawer
    {
        #region UNITY_EDITOR
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label);
        }
        #endregion
    }  
#endif
}