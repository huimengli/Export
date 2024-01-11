using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Export.BehaviourEX
{
    /// <summary>
    /// 专门设计给Nreal项目用
    /// <br />
    /// 放弃继承此类需要放弃使用Start和Update以防止出错
    /// </summary>
    public abstract class NrealBehaviour : MonoBehaviour
    {
        /// <summary>
        /// NRInput对象名称
        /// </summary>
        public static readonly List<string> NRINPUT = new List<string> {
            "NRInput"
        };
        /// <summary>
        /// NRInput右摇杆控制器名称
        /// </summary>
        public static readonly List<string> NRINPUT_RIGHT = new List<string> {
            "Right"
        };
        /// <summary>
        /// NRInput手机屏幕名称
        /// </summary>
        public static readonly List<string> NR_VIRTUAL_DISPLAYER = new List<string> {
            "NRVirtualDisplayer",
            "NRVirtualDisplayer(Clone)"
        };
        /// <summary>
        /// NR摄像头对象
        /// </summary>
        public static readonly List<string> NR_CAMERA_RIG = new List<string>
        {
            "NRCameraRig",
            "NRCameraRig(Clone)"
        };
        /// <summary>
        /// 最大重试次数
        /// </summary>
        public static readonly int MAX_ERROR_TIMES = 10;
        /// <summary>
        /// 重试等待时间
        /// <br />
        /// (单位：秒)
        /// </summary>
        public static readonly float WAIT_ERROR_TIME = 0.2f;

        /// <summary>
        /// NRInput对象
        /// </summary>
        private static GameObject nrInput;

        /// <summary>
        /// NRInput
        /// 右摇杆控制器
        /// </summary>
        private static GameObject nrInputRight;

        /// <summary>
        /// NRInput手机屏幕对象
        /// </summary>
        private static GameObject nrVirtualDisplayer;

        /// <summary>
        /// Nreal摄像头
        /// </summary>
        private static GameObject nrCameraRig;

        /// <summary>
        /// NRInput对象
        /// </summary>
        public static GameObject NRInput
        {
            get
            {
                if (nrInput != null)
                {
                    return nrInput;
                }
                else
                {
                    // nrInput = GameObject.Find(NRINPUT);
                    nrInput = NRINPUT.Map(str =>
                    {
                        return GameObject.Find(str);
                    }).GetOnlyOne();
                    return nrInput;
                }
            }
        }

        /// <summary>
        /// NRInput
        /// 右遥感控制器
        /// </summary>
        public static GameObject NRInputRight
        {
            get
            {
                if (nrInputRight != null)
                {
                    return nrInputRight;
                }
                else
                {
                    if (NRInput == null)
                    {
                        return null;
                    }
                    else
                    {
                        // var t = NRInput.transform.Find(NRINPUT_RIGHT);
                        var t = NRINPUT_RIGHT.Map(str =>
                        {
                            return NRInput.transform.Find(str);
                        }).GetOnlyOne();
                        if (t == null)
                        {
                            return null;
                        }
                        else
                        {
                            nrInputRight = t.gameObject;
                            return nrInputRight;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// NRInput手机屏幕对象
        /// </summary>
        public static GameObject NRVirtualDisplayer
        {
            get
            {
                if (nrVirtualDisplayer != null)
                {
                    return nrVirtualDisplayer;
                }
                else
                {
                    // nrVirtualDisplayer = GameObject.Find(NR_VIRTUAL_DISPLAYER);
                    nrVirtualDisplayer = NR_VIRTUAL_DISPLAYER.Map(str =>
                    {
                        return GameObject.Find(str);
                    }).GetOnlyOne();
                    return nrVirtualDisplayer;
                }
            }
        }

        /// <summary>
        /// Nreal摄像头
        /// </summary>
        public static GameObject NRCameraRig
        {
            get
            {
                if (nrCameraRig!=null)
                {
                    return nrCameraRig;
                }
                else
                {
                    nrCameraRig = NR_CAMERA_RIG.Map(str =>
                    {
                        return GameObject.Find(str);
                    }).GetOnlyOne();
                    return nrCameraRig;
                }
            }
        }

        /// <summary>
        /// 根据名称查找父对象下的对象
        /// </summary>
        /// <param name="objName">对象名称</param>
        /// <param name="parent">父对象</param>
        /// <returns></returns>
        public static GameObject Find(string objName, GameObject parent)
        {
            if (parent == null)
            {
                return null;
            }
            var t = parent.transform.Find(objName);
            if (t == null)
            {
                return null;
            }
            else
            {
                return t.gameObject;
            }
        }

        /// <summary>
        /// 查找右摇杆下的对象
        /// </summary>
        /// <param name="objName"></param>
        /// <returns></returns>
        public static GameObject Find(string objName)
        {
            return Find(objName, NRInputRight);
        }

        /// <summary>
        /// 启动错误列表
        /// </summary>
        private List<string> StartErrorList = new List<string>();

        /// <summary>
        /// 启动错误次数
        /// </summary>
        private int errorTimes = 0;
        /// <summary>
        /// 当前模块是否启动
        /// </summary>
        private bool nrealToolsIsInit = false;
        /// <summary>
        /// 当前模块是否启动
        /// </summary>
        public bool NrealToolsIsInit
        {
            get
            {
                return nrealToolsIsInit;
            }
        }

        /// <summary>
        /// 获取启动错误列表
        /// </summary>
        /// <remarks>
        /// 如果想要添加加载项，请修改此函数
        /// </remarks>
        private void GetStartErrorList()
        {
            if (NRInput == null)
            {
                StartErrorList.Add("NRInput初始化失败!");
            }
            if (NRInputRight == null)
            {
                StartErrorList.Add("NRInputRight初始化失败!");
            }
            if (NRVirtualDisplayer == null)
            {
                StartErrorList.Add("NRVirtualDisplayer初始化失败!");
            }
            if (NRCameraRig == null)
            {
                StartErrorList.Add("NRCameraRig初始化失败!");
            }
        }

        /// <summary>
        /// 等待NRInput初始化完成<br />
        /// 运行结束时会调用NewStart
        /// </summary>
        /// <returns></returns>
        IEnumerator WaitNRInputInit()
        {
            while (nrealToolsIsInit == false)
            {
                //检测初始化
                StartErrorList.Clear();
                GetStartErrorList();
                if (StartErrorList.Length() == 0)
                {
                    nrealToolsIsInit = true;
                    Debug.Log("NRInput组件加载完成");
                }
                else
                {
                    errorTimes++;
                    if (errorTimes >= MAX_ERROR_TIMES)
                    {
                        nrealToolsIsInit = true;
                        Debug.LogWarning(StartErrorList.Join(",\n") + "组件无法加载成功");
                    }
                }

                // 运行NewStart
                if (nrealToolsIsInit)
                {
                    NewStart();
                }

                // 等待一段时间
                yield return new WaitForSeconds(WAIT_ERROR_TIME); // 处理等待时间
            }
        }

        // Start is called before the first frame update
        /// <summary>
        /// 此方法已经被废弃<br />
        /// 请使用NewStart
        /// </summary>
        [Obsolete("此方法已经被废弃,请使用NewStart")]
        protected virtual void Start()
        {
            //GetStartErrorList();
            //Debug.LogWarning(StartErrorList.Join(",\n"));
            // // 启动继承类的Start方法
            //NewStart();

            //启动协程
            StartCoroutine(WaitNRInputInit());
        }

        /// <summary>
        /// 让继承此类的代码放弃使用Start
        /// 转而使用NewStart
        /// </summary>
        /// <remarks>
        /// 此函数会在等待NRInput模块加载完或者超过最大重试次数后运行<br />
        /// 只在第一帧运行一次
        /// </remarks>
        protected abstract void NewStart();

        // Update is called once per frame
        /// <summary>
        /// 此方法已经被弃用
        /// 请使用NewUpdate
        /// </summary>
        [Obsolete("此方法已经被废弃，请使用NewUpdate")]
        void Update()
        {
            if (NrealToolsIsInit)
            {
                NewUpdate();
            }
        }

        /// <summary>
        /// 让继承此类的代码放弃使用Update
        /// 转为使用NewUpdate
        /// </summary>
        /// <remarks>
        /// 此函数会在等待NRInput模块加载完或者超过最大重试次数后运行<br />
        /// 每帧运行一次
        /// </remarks>
        protected abstract void NewUpdate();

        /// <summary>
        /// 此方法已经被废弃<br />
        /// 请使用OnNewDestroy
        /// </summary>
        /// <remarks>
        /// 添加对象时,这里也需要修改
        /// </remarks>
        [Obsolete("此方法已经被废弃,请使用OnNewDestroy")]
        private void OnDestroy()
        {
            //清空数据
            nrInput = null;
            nrInputRight = null;
            nrVirtualDisplayer = null;
            nrCameraRig = null;

            OnNewDestroy();
        }

        /// <summary>
        /// 让继承此类的代码放弃使用OnDestroy
        /// 转而使用OnNewDestroy
        /// </summary>
        protected virtual void OnNewDestroy()
        {

        }
    }

    /// <summary>
    /// NrealBehaviour追加方法
    /// </summary>
    public static class NrealBehaviourAdd
    {
        /// <summary>
        /// 获取列表长度
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static int Length<T>(this List<T> list)
        {
            return list.Count;
        }

        /// <summary>
        /// 遍历执行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name=""></param>
        /// <param name="action"></param>
        public static void ForEach<T>(this List<T> list, Action<T, int> action)
        {
            for (int i = 0; i < list.Count; i++)
            {
                T t = list[i];
                action(t, i);
            }
        }

        /// <summary>
        /// 将列表连接成字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="conconnectionString"></param>
        /// <returns></returns>
        public static string Join<T>(this List<T> list, string conconnectionString)
        {
            var ret = new StringBuilder();
            list.ForEach((t, i) =>
            {
                ret.Append(t);
                if (i < list.Count - 1)
                {
                    ret.Append(conconnectionString);
                }
            });
            return ret.ToString();
        }

        /// <summary>
        /// 对列表中的每个元素执行给定的转换操作，并返回一个包含转换结果的新列表。
        /// </summary>
        /// <typeparam name="T">列表中元素的类型。</typeparam>
        /// <typeparam name="R">转换操作的结果类型。</typeparam>
        /// <param name="list">要进行转换的源列表。</param>
        /// <param name="getReturn">一个转换函数，它接受列表中的元素（类型为 T）和元素的索引（int），返回转换后的结果（类型为 R）。</param>
        /// <returns>一个新的列表，包含源列表中每个元素经过转换函数处理后的结果。</returns>
        public static List<R> Map<T, R>(this List<T> list, Func<T, int, R> getReturn)
        {
            var ret = new List<R>();
            for (int i = 0; i < list.Length(); i++)
            {
                var t = list[i];
                if (t == null)
                {
                    ret.Add(default(R));
                }
                else
                {
                    ret.Add(getReturn(t, i));
                }
            }
            return ret;
        }

        /// <summary>
        /// 对列表中的每个元素执行给定的转换操作，并返回一个包含转换结果的新列表。
        /// </summary>
        /// <typeparam name="T">列表中元素的类型。</typeparam>
        /// <typeparam name="R">转换操作的结果类型。</typeparam>
        /// <param name="list">要进行转换的源列表。</param>
        /// <param name="getReturn">一个转换函数，它接受列表中的元素（类型为 T），返回转换后的结果（类型为 R）。</param>
        /// <returns>一个新的列表，包含源列表中每个元素经过转换函数处理后的结果。</returns>
        public static List<R> Map<T, R>(this List<T> list, Func<T, R> getReturn)
        {
            var ret = new List<R>();
            for (int i = 0; i < list.Length(); i++)
            {
                var t = list[i];
                if (t == null)
                {
                    ret.Add(default(R));
                }
                else
                {
                    ret.Add(getReturn(t));
                }
            }
            return ret;
        }

        /// <summary>
        /// 对列表进行过滤，根据提供的条件函数选择元素。
        /// </summary>
        /// <typeparam name="T">列表中元素的类型。</typeparam>
        /// <param name="list">要进行过滤的源列表。</param>
        /// <param name="isFilter">过滤函数，它接受列表中的元素（类型为 T）和元素的索引（int），并返回一个布尔值，指示是否应该包含当前元素。</param>
        /// <returns>一个新的列表，仅包含满足过滤条件的元素。</returns>
        public static List<T> Filter<T>(this List<T> list, Func<T, int, bool> isFilter)
        {
            var ret = new List<T>();
            for (int i = 0; i < list.Length(); i++)
            {
                var t = list[i];
                var r = isFilter(t, i);
                if (r)
                {
                    ret.Add(t);
                }
            }
            return ret;
        }

        /// <summary>
        /// 对列表进行过滤，根据提供的条件函数选择元素。
        /// </summary>
        /// <typeparam name="T">列表中元素的类型。</typeparam>
        /// <param name="list">要进行过滤的源列表。</param>
        /// <param name="isFilter">过滤函数，它接受列表中的元素（类型为 T），并返回一个布尔值，指示是否应该包含当前元素。</param>
        /// <returns>一个新的列表，仅包含满足过滤条件的元素。</returns>
        public static List<T> Filter<T>(this List<T> list, Func<T, bool> isFilter)
        {
            var ret = new List<T>();
            for (int i = 0; i < list.Length(); i++)
            {
                var t = list[i];
                var r = isFilter(t);
                if (r)
                {
                    ret.Add(t);
                }
            }
            return ret;
        }

        /// <summary>
        /// 从泛型列表中获取第一个符合特定条件的元素。这些条件包括不是 null、不是空字符串、
        /// 对于数值类型不是 0、不是 NaN、不是无穷大。
        /// </summary>
        /// <typeparam name="T">列表中元素的类型。</typeparam>
        /// <param name="list">要检查的源列表。</param>
        /// <returns>
        /// 返回列表中的第一个符合条件的元素。如果没有找到符合条件的元素，返回类型 T 的默认值。
        /// 对于数值类型，检查元素是否为 0、NaN 或无穷大；对于字符串，检查是否为空字符串。
        /// </returns>
        public static T GetOnlyOne<T>(this List<T> list)
        {
            foreach (var item in list)
            {
                if (item == null) continue;

                if (item is string str && string.IsNullOrEmpty(str)) continue;

                if (item is IConvertible convertible)
                {
                    TypeCode typeCode = convertible.GetTypeCode();
                    switch (typeCode)
                    {
                        case TypeCode.Double:
                        case TypeCode.Single:
                            double d = convertible.ToDouble(null);
                            if (double.IsNaN(d) || double.IsInfinity(d)) continue;
                            break;
                        case TypeCode.Decimal:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Int16:
                        case TypeCode.Byte:
                        case TypeCode.UInt32:
                        case TypeCode.UInt64:
                        case TypeCode.UInt16:
                        case TypeCode.SByte:
                            if (convertible.ToInt64(null) == 0) continue;
                            break;
                            // 您可以根据需要添加更多的类型处理
                    }
                }

                return item; // 返回符合条件的第一个元素
            }

            return default(T); // 如果没有找到符合条件的元素，则返回默认值
        }

        public static List<int> Range<T>(this List<T> list, int end)
        {
            var ret = new List<int>();
            for (int i = 0; i < end; i++)
            {
                ret.Add(i);
            }
            return ret;
        }

        public static List<int> Range<T>(this List<T> list, int start, int end)
        {
            var ret = new List<int>();
            for (int i = start; i < end; i++)
            {
                ret.Add(i);
            }
            return ret;
        }

        public static List<int> Range<T>(this List<T> list, int start, int end, int step)
        {
            var ret = new List<int>();
            for (int i = start; i < end; i += step)
            {
                ret.Add(i);
            }
            return ret;
        }
    }

}