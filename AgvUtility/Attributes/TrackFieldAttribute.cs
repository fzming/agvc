using System;
using Utility.Extensions;

namespace Utility.Attributes
{
    public interface ITrackFieldValueConverter
    {
        string Convert(object source, Type sourceType);
    }

    public class EnumAliasNameConverter : ITrackFieldValueConverter
    {
        public string Convert(object source, Type sourceType)
        {
            if (source == null) return string.Empty;
            var value = (Enum) Enum.Parse(sourceType, source.ToStringEx());
            return value.GetAliasName();
        }
    }

    /// <summary>
    ///     字段值变化跟踪
    /// </summary>
    public class TrackFieldAttribute : Attribute
    {
        /// <summary>
        ///     初始化 <see cref="T:System.Attribute" /> 类的新实例。
        /// </summary>
        public TrackFieldAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        ///     初始化 <see cref="T:System.Attribute" /> 类的新实例。
        /// </summary>
        public TrackFieldAttribute()
        {
        }

        public Type ValueConverter { get; set; }

        /// <summary>
        ///     字段中文名称
        /// </summary>
        public string Name { get; set; }
    }
}