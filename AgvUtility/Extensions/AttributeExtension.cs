using System;
using Utility.Attributes;

namespace Utility.Extensions
{
    /// <summary>
    /// 自定义特性扩展
    /// </summary>
    public static class AttributeExtension
    {
        /// <summary>
        /// 获取指定的特性
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static TAttribute GetAttribute<TAttribute>(this Enum t) where
            TAttribute : Attribute
        {

            var type = t.GetType();
            var memInfo = type.GetMember(t.ToString());
            if (memInfo.Length > 0)
            {
                var attrs = memInfo[0].GetCustomAttributes(typeof(TAttribute), false);
                if (attrs.Length > 0)
                    return (TAttribute)attrs[0];
            }
            return default;
        }

        public static string GetDescription<TAttribute>(this Enum t) where TAttribute : Attribute, IDescription
        {
            var attr = t.GetAttribute<TAttribute>();
            return attr?.Description;
        }
        /// <summary>
        /// 枚举的修改名称
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string GetAliasName(this Enum t)
        {
            return t.GetDescription<NameAttribute>();
        }  
        
        public static string GetTagName(this Enum t)
        {
            return t.GetDescription<TagAttribute>();
           
        } 
        public static string GetFormatName(this Enum t)
        {
            return t.GetDescription<FormatAttribute>();
        }
        
    }
}