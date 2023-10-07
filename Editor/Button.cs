//#define UNITY_EDITOR

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
                Item.GetInput(
                    $"对象:[{message.name}]的备注内容:",
                    message.value,
                    value => {
                        //Debug.Log($"回调显示:{value}");
                        message.ChangeValue(value);
                    }
                );
            }
#if UNITY_EDITOR
            if (GUILayout.Button("保存数据"))
            {
                if (PrefabUtility.GetPrefabInstanceStatus(message.gameObject) != PrefabInstanceStatus.NotAPrefab)
                {
                    PrefabUtility.SavePrefabAsset(message.gameObject);
                    Debug.Log($"预制件[{message.gameObject.name}] 已保存");
                }
            }
#endif
        }
    }
}
