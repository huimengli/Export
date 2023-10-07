using System.Collections.Generic;
using UnityEngine;
using System;

namespace Export.BehaviourEX
{
    /// <summary>
    /// UnityMainThreadDispatcher
    /// 简单的 Unity 主线程调度器
    /// 它允许你安排方法在主线程上执行
    /// </summary>
    public class Dispatcher : MonoBehaviour
    {
        /// <summary>
        /// 定义一个静态的、只读的动作队列，用于储存需要在主线程上执行的动作。
        /// 这个队列是线程安全的，因为我们在操作它时使用了 lock 语句来保证线程安全。
        /// </summary>
        private static readonly Queue<Action> _executionQueue = new Queue<Action>();

        /// <summary>
        /// 执行线程上的操作
        /// </summary>
        public void Upgrate()
        {
            // 使用 lock 语句确保线程安全，因为这个队列可能会被其他线程访问和修改
            lock (_executionQueue)
            {
                // 当队列中有未处理的动作时，继续处理
                while (_executionQueue.Count > 0)
                {
                    // 从队列中取出动作并执行它
                    // 这确保了这个动作是在主线程上执行的
                    _executionQueue.Dequeue().Invoke();
                }
            }
        }

        /// <summary>
        /// 这个静态方法允许其他线程安排动作在主线程上执行
        /// 这是通过把动作加到 _executionQueue 队列中实现的
        /// </summary>
        /// <param name="action"></param>
        public static void InvokeOnMainThread(Action action)
        {
            // 使用 lock 语句确保线程安全
            lock (_executionQueue)
            {
                // 把动作加入到队列中
                // 它会在下一次 Upgrade 方法调用时被执行
                _executionQueue.Enqueue(action);
            }
        }

        /// <summary>
        /// 这个静态方法允许其他线程安排动作在主线程上执行
        /// </summary>
        /// <param name="action"></param>
        public static void Invoke(Action action)
        {
            InvokeOnMainThread(action);
        }

        void Update()
        {
            Debug.Log("更新");
        }
    }
}