using UnityEngine;

namespace Export.Attribute
{
    /// <summary>
    /// 允许内容以只读的方式进行多行显示
    /// </summary>
    public class ReadOnlyTextAreaAttribute : PropertyAttribute
    {
        public int minLines { get; private set; }
        public int maxLines { get; private set; }

        public ReadOnlyTextAreaAttribute(int minLines, int maxLines)
        {
            this.minLines = minLines;
            this.maxLines = maxLines;
        }

        public ReadOnlyTextAreaAttribute()
        {

        }
    } 
}