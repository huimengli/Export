using Export.Attribute;
using Export.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Export
{
    /// <summary>
    /// ��Ϣ��
    /// ����ʹ��Dispatcher���滻MonoBehaviour
    /// </summary>
    public class Message : Dispatcher
    {
        /// <summary>
        /// ���ﲻд����
        /// �Ƕ��������������˿���Ϣ�õ�
        /// </summary>
        [ReadOnly]
        public string value;

        /// <summary>
        /// �޸���������
        /// </summary>
        /// <param name="text"></param>
        public void ChangeValue(string text)
        {
            this.value = text;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    } 
}
