﻿using Export.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using Export.BehaviourEX;

namespace Export.Tools
{
    /// <summary>
    /// 添加DropAndStick的点
    /// </summary>
    class AddDASPoint:MonoBehaviour
    {
        /// <summary>
        /// 追加点位名称
        /// </summary>
        [SerializeField]
        public string addPointsName;

        /// <summary>
        /// 读取名称
        /// </summary>
        private Regex readName;

        /// <summary>
        /// 追加点
        /// </summary>
        [SerializeField]
        [ReadOnly]
        public Transform[] points;

        private void Start()
        {
            readName = new Regex(addPointsName);

            points = transform.GetComponentsInChildren<Transform>();
            points = points.Where(t => t != null && readName.IsMatch(t.name)).ToArray();

            DragAndStickBehaviour._points.AddRange(points);
        }
    }
}