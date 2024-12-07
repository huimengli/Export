using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Export.AddFunc
{
    /// <summary>
    /// 字典类添加功能
    /// </summary>
    public static class DictionaryAdd
    {
        /// <summary>
        /// 键直接转换成列表
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static List<TKey> KeysToArray<TKey,TValue>(this Dictionary<TKey,TValue> dict)
        {
            var keys = dict.Keys;
            return new List<TKey>(keys);
        }

        /// <summary>
        /// 值直接转换成列表
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static List<TValue> ValuesToArray<TKey,TValue>(this Dictionary<TKey,TValue> dict)
        {
            var values = dict.Values;
            return new List<TValue>(values);
        }
    }
}
