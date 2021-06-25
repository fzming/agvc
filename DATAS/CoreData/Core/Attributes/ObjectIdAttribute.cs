using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;

namespace CoreData.Core.Attributes
{
    /// <summary>
    /// 验证ObjectId
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ObjectIdAttribute:ValidationAttribute
    {
        /// <summary>
        ///   初始化 <see cref="T:System.ComponentModel.DataAnnotations.ValidationAttribute" /> 类的新实例。
        /// </summary>
        public ObjectIdAttribute():base("{0} 不是有效的ObjectId")
        {
        }

        /// <summary>验证指定的值相对于当前的验证特性。</summary>
        /// <param name="value">要验证的值。</param>
        /// <param name="validationContext">有关验证操作的上下文信息。</param>
        /// <returns>
        ///   <see cref="T:System.ComponentModel.DataAnnotations.ValidationResult" /> 类的实例。
        /// </returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            
            if (value!=null&& ObjectId.TryParse(value.ToString(),out _))
            {
                return ValidationResult.Success;
            }
            //FormatErrorMessage方法会自动使用显示的属性名称来格式化这个字符串
            var errorMessage = FormatErrorMessage(validationContext.DisplayName);
            return new ValidationResult(errorMessage);
 
        }
    }
}