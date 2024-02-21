using System.Collections.Generic;
using UnityEngine;
using Export.Attribute;

namespace Export.BehaviourEX
{
    /// <summary>
    /// ��ק������Ϊ
    /// ��Ҫ�̳в����receptorPoints����
    /// </summary>
    public abstract class DragAndStickBehaviour : MonoBehaviour
    {
        /// <summary>
        /// ���е�������
        /// </summary>
        public static List<Transform> _points = new List<Transform>();

        /// <summary>
        /// ��Ӱ��Ԥ����
        /// </summary>
        public GameObject shadowPrefab;

        /// <summary>
        /// ��ǰ�����ϵĽ��ܵ�
        /// </summary>
        [ReadOnly]
        public Transform[] receptorPoints;

        /// <summary>
        /// �����ľ���
        /// </summary>
        public float stickDistance = 0.5f;

        /// <summary>
        /// �������ٶ�
        /// </summary>
        public float moveSpeed = 5f;

        /// <summary>
        /// ��ǰ����߶�
        /// </summary>
        public float height = 1f;

        /// <summary>
        /// �Ƿ����ڱ���ק
        /// </summary>
        [ReadOnly]
        public bool isDragging = false;

        /// <summary>
        /// �Ƿ�ʼ����
        /// </summary>
        [ReadOnly]
        public bool isSticking = false;

        /// <summary>
        /// �Ƿ��Ѿ����������
        /// </summary>
        [ReadOnly]
        public bool isSticked = false;

        /// <summary>
        /// ��Ӱʵ��
        /// </summary>
        protected GameObject shadow;

        /// <summary>
        /// �洢�뱻��ק���������������
        /// </summary>
        protected Transform closestPoint;

        /// <summary>
        /// �������λ��
        /// </summary>
        protected Vector3 closestPostion
        {
            get
            {
                return new Vector3(
                    closestPoint.position.x,
                    closestPoint.position.y + height / 2,
                    closestPoint.position.z
                );
            }
        }

        /// <summary>
        /// ����ÿһ֡��������ק�������߼�
        /// </summary>
        protected void Upgrade()
        {
            if (isDragging)
            {
                DragObject();
            }
            if (isSticking && !isSticked)
            {
                StickToObject();
            }
        }

        /// <summary>
        /// ��ק����
        /// </summary>
        void DragObject()
        {
            float distanceToCamera = Vector3.Distance(transform.position, Camera.main.transform.position);
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distanceToCamera));
            transform.position = new Vector3(mousePos.x, mousePos.y, transform.position.z);

            bool canStick = CheckIfCanStick(); // ����һ���µķ���������Ƿ����������ʹ���������

            if (canStick)
            {
                if (shadow == null)
                {
                    // �޸���Ӱ��λ��Ϊ closestPoint ��λ��
                    shadow = Instantiate(shadowPrefab, closestPostion, Quaternion.identity);
                }
                else
                {
                    // ������Ӱ��λ��Ϊ closestPoint ��λ��
                    shadow.transform.position = closestPostion;
                    shadow.SetActive(true);
                }
            }
            else
            {
                shadow.SetActive(false);
            }
        }

        /// <summary>
        /// �·��������ڼ���Ƿ��������
        /// </summary>
        /// <returns></returns>
        private bool CheckIfCanStick() 
        {
            bool canStick = true;

            float minDistance = float.MaxValue;
            foreach (var receptor in receptorPoints)
            {
                Transform closest = null;

                foreach (var point in _points)
                {
                    float distance = Vector3.Distance(receptor.position, point.position);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closest = point;
                    }
                }

                if (minDistance > stickDistance)
                {
                    canStick = false;
                    break;
                }
                else
                {
                    closestPoint = closest; // ���� closestPoint ��ֵ
                }
            }

            return canStick;
        }

        /// <summary>
        /// ����������
        /// </summary>
        void StickToObject()
        {
            transform.position = Vector3.MoveTowards(transform.position, closestPostion, moveSpeed * Time.deltaTime); // �ƶ��� closestPoint ��λ��
            if (Vector3.Distance(transform.position, closestPostion) <= 0.01f)
            {
                isSticking = false;
                isSticked = true;
                if (shadow != null)
                {
                    Destroy(shadow);
                    shadow = null;
                    //shadow.SetActive(false);
                }
            }
        }

        /// <summary>
        /// ����갴��ʱ��ʼ��ק
        /// </summary>
        void OnMouseDown()
        {
            isDragging = true;
            isSticked = false;
        }

        /// <summary>
        /// ������ɿ�ʱֹͣ��ק
        /// </summary>
        void OnMouseUp()
        {
            isDragging = false;
            if (shadow!=null&&shadow.activeSelf==true)
            {
                isSticking = true;
            }
        }
    } 
}