using UnityEditor;
using UnityEngine;
using Export.Tools;

namespace Export.Editor
{
    /// <summary>
    /// 自定义编辑器
    /// </summary>
    [CustomEditor(typeof(Message))]
    class MessageButton : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();//显示默认的检查器

            var message = (Message)target;
            if (GUILayout.Button("修改消息"))
            {
                Item.GetInput(value => {
                    //Debug.Log($"回调显示:{value}");
                    message.ChangeValue(value);
                });
            }
        }
    }
}
