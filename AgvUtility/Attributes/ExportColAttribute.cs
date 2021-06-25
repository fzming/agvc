using System;

namespace Utility.Attributes
{
    public class ExportColAttribute : Attribute
    {
        /// <summary>
        /// 排序
        /// </summary>
        public int Order { get; set; }
        public bool Bold { get; set; }
        public bool Merge { get; set; }
        public int Flag { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Caption { get; set; }
        /// <summary>
        ///   初始化 <see cref="T:System.Attribute" /> 类的新实例。
        /// </summary>
        public ExportColAttribute(int order, bool merge = true)
        {
            Order = order;
            Merge = merge;
        }

    }
     
}