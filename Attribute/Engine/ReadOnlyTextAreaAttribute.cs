using UnityEngine;

namespace Export.Attribute
{
    /// <summary>
    /// 允许内容以只读的方式进行多行显示
    /// </summary>
    public class ReadOnlyTextAreaAttribute : PropertyAttribute
    {
        /// <summary>
        /// 最小行数
        /// </summary>
        public int minLines { get; private set; }
        /// <summary>
        /// 最大行数
        /// </summary>
        public int maxLines { get; private set; }

        /// <summary>
        /// 只读文字区域构造函数
        /// </summary>
        /// <param name="minLines"></param>
        /// <param name="maxLines"></param>
        public ReadOnlyTextAreaAttribute(int minLines, int maxLines)
        {
            this.minLines = minLines;
            this.maxLines = maxLines;
        }

        /// <summary>
        /// 只读文字区域构造函数
        /// </summary>
        public ReadOnlyTextAreaAttribute()
        {

        }
    } 
}