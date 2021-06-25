using System;
using System.Diagnostics.CodeAnalysis;

namespace Utility.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DataColumnAttribute:Attribute
    {
        #region 构造函数

        /// <summary>
        ///   初始化 <see cref="T:System.Attribute" /> 类的新实例。
        /// </summary>
        public DataColumnAttribute(string name, int order,int flag)  
        {
            Name = name;
            Order = order;
            Flag = flag;
        }

        public DataColumnAttribute(string name, int order):this(name,order,0)
        {

        }
        /// <summary>
        ///   初始化 <see cref="T:System.Attribute" /> 类的新实例。
        /// </summary>
        public DataColumnAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        ///   初始化 <see cref="T:System.Attribute" /> 类的新实例。
        /// </summary>
        public DataColumnAttribute(string name, int order, bool hidden)
        {
            Hidden = hidden;
            Name = name;
            Order = order;
        }

        /// <summary>
        ///   初始化 <see cref="T:System.Attribute" /> 类的新实例。
        /// </summary>
        public DataColumnAttribute()
        {
        }
        

        #endregion
        /// <summary>
        /// 列名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 列序
        /// </summary>
        public int Order { get; set; }
        /// <summary>
        /// 标记
        /// </summary>
        public int Flag { get;  set; }

        /// <summary>
        /// 是否隐藏
        /// </summary>
        public bool Hidden { get; set; }
        /// <summary>
        /// 一直显示，禁止隐藏
        /// </summary>
        public bool AlwaysShow { get; set; }
        /// <summary>
        /// 是否冻结
        /// </summary>
        public bool Locked { get; set; }

        /// <summary>
        /// 是否可以拖动列
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public bool Draggable { get; set; } = true;

        /// <summary>
        /// 宽度
        /// </summary>
        public string Width { get; set; }
        /// <summary>
        ///  类型
        /// </summary>
        public ColumnType ColumnType { get; set; }
        /// <summary>
        /// 是否合计
        /// </summary>
        public bool Summary { get; set; }
    }
    
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public enum ColumnType
    {
        None=0,
        RMB=1,
        USD=2
    }
}