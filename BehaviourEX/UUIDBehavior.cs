using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Export.Tools;

namespace Export.BehaviourEX
{
    /// <summary>
    /// 通用单一标识符
    /// </summary>
    public class UUIDBehavior : MonoBehaviour
    {
        /// <summary>
        /// 全局静态随机数
        /// 用于生成UUID
        /// </summary>
        private static System.Random random = new System.Random();

        /// <summary>
        /// UUID
        /// </summary>
        public string UUID { get; } = Item.NewUUID(random.NextDouble().ToString());
    }
}
