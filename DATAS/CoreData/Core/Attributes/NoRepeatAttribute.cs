using System;

namespace CoreData.Core.Attributes
{
    /// <summary>
    /// 禁止重复标志
    /// </summary>

    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property)]
    public class NoRepeatAttribute : Attribute
    {
        public string ErrorMessage { get; }
        /// <summary>
        /// 是否在所有机构中都不能重复
        /// </summary>
        public bool Global { get; set; }
        public NoRepeatAttribute(string errorMessage="{0}“{1}”已在系统中存在，该记录不能重复")
        {
            ErrorMessage = errorMessage;
        }

        
    }
}