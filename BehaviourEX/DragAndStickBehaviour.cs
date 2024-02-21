using System.Collections.Generic;
using UnityEngine;
using Export.Attribute;

namespace Export.BehaviourEX
{
    /// <summary>
    /// 拖拽吸附行为
    /// 需要继承并添加receptorPoints内容
    /// </summary>
    public abstract class DragAndStickBehaviour : MonoBehaviour
    {
        /// <summary>
        /// 所有的吸附点
        /// </summary>
        public static List<Transform> _points = new List<Transform>();

        /// <summary>
        /// 虚影的预制体
        /// </summary>
        public GameObject shadowPrefab;

        /// <summary>
        /// 当前对象上的接受点
        /// </summary>
        [ReadOnly]
        public Transform[] receptorPoints;

        /// <summary>
        /// 吸附的距离
        /// </summary>
        public float stickDistance = 0.5f;

        /// <summary>
        /// 吸附的速度
        /// </summary>
        public float moveSpeed = 5f;

        /// <summary>
        /// 当前对象高度
        /// </summary>
        public float height = 1f;

        /// <summary>
        /// 是否正在被拖拽
        /// </summary>
        [ReadOnly]
        public bool isDragging = false;

        /// <summary>
        /// 是否开始吸附
        /// </summary>
        [ReadOnly]
        public bool isSticking = false;

        /// <summary>
        /// 是否已经完成了吸附
        /// </summary>
        [ReadOnly]
        public bool isSticked = false;

        /// <summary>
        /// 虚影实例
        /// </summary>
        protected GameObject shadow;

        /// <summary>
        /// 存储离被拖拽物体最近的吸附点
        /// </summary>
        protected Transform closestPoint;

        /// <summary>
        /// 吸附点的位置
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
        /// 更新每一帧，用于拖拽和吸附逻辑
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
        /// 拖拽对象
        /// </summary>
        void DragObject()
        {
            float distanceToCamera = Vector3.Distance(transform.position, Camera.main.transform.position);
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distanceToCamera));
            transform.position = new Vector3(mousePos.x, mousePos.y, transform.position.z);

            bool canStick = CheckIfCanStick(); // 创建一个新的方法来检查是否可以吸附，使代码更整洁

            if (canStick)
            {
                if (shadow == null)
                {
                    // 修改虚影的位置为 closestPoint 的位置
                    shadow = Instantiate(shadowPrefab, closestPostion, Quaternion.identity);
                }
                else
                {
                    // 更新虚影的位置为 closestPoint 的位置
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
        /// 新方法，用于检查是否可以吸附
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
                    closestPoint = closest; // 更新 closestPoint 的值
                }
            }

            return canStick;
        }

        /// <summary>
        /// 吸附到对象
        /// </summary>
        void StickToObject()
        {
            transform.position = Vector3.MoveTowards(transform.position, closestPostion, moveSpeed * Time.deltaTime); // 移动到 closestPoint 的位置
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
        /// 当鼠标按下时开始拖拽
        /// </summary>
        void OnMouseDown()
        {
            isDragging = true;
            isSticked = false;
        }

        /// <summary>
        /// 当鼠标松开时停止拖拽
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