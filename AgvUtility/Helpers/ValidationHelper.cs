using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Utility.Helpers
{
    /// <summary>
    /// 验证结果
    /// </summary>
    public class ValidResult
    {
        /// <summary>
        /// 错误成员列表
        /// </summary>
        public List<ErrorMember> ErrorMembers { get; set; }
        /// <summary>
        /// 是否通过验证
        /// </summary>
        public bool IsValid { get; set; }
    }
    /// <summary>
    /// 错误成员
    /// </summary>
    public class ErrorMember
    {
        /// <summary>
        /// 错误消息
        /// </summary>
        public string ErrorMessage { get; set; }
        /// <summary>
        /// 错误的成员名称
        /// </summary>
        public string ErrorMemberName { get; set; }
    }
    /// <summary>
    /// 数据注解验证帮助类
    /// https://blog.csdn.net/litao2/article/details/78568364
    /// https://msdn.microsoft.com/zh-cn/library/system.componentmodel.dataannotations(v=vs.110).aspx
    /// </summary>
    public class ValidationHelper
    {
        /// <summary>
        /// 验证指定的对象
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ValidResult Validate(object value)
        {
            var result = new ValidResult();
            try
            {
                var validationContext = new ValidationContext(value);
                var results = new List<ValidationResult>();
                var isValid = Validator.TryValidateObject(value, validationContext, results, true);

                if (!isValid)
                {
                    result.IsValid = false;
                    result.ErrorMembers = new List<ErrorMember>();
                    foreach (var item in results)
                    {
                        result.ErrorMembers.Add(new ErrorMember
                        {
                            ErrorMessage = item.ErrorMessage,
                            ErrorMemberName = item.MemberNames.FirstOrDefault()
                        });
                    }
                }
                else
                {
                    result.IsValid = true;
                }
            }
            catch (Exception ex)
            {
                result.IsValid = false;
                result.ErrorMembers = new List<ErrorMember>
                {
                    new() {ErrorMessage = ex.Message, ErrorMemberName = "Internal validation error"}
                };
            }

            return result;
        }
    }
}