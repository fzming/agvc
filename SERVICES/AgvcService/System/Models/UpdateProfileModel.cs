using CoreData;
using CoreData.Models;
using Utility.Extensions;

namespace AgvcService.System.Models
{
    /// <summary>
    ///     更新用户档案模型
    /// </summary>
    public class UpdateProfileModel : IValidation
    {
        /// <summary>
        ///     用户昵称
        /// </summary>
        public string Nick { get; set; }

        /// <summary>
        ///     邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        ///     联系电话
        /// </summary>
        public string ContactPhone { get; set; }

        /// <summary>
        ///     个人介绍
        /// </summary>
        public string Introduction { get; set; }

        /// <summary>
        ///     性别
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        ///     头像地址
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        ///     验证元数据
        /// </summary>
        /// <returns></returns>
        public Result<bool> Validate()
        {
            if (Nick.IsNullOrEmpty()) return Result<bool>.Fail("昵称不能为空");
            if (Email.IsNotNullOrEmpty() && !Email.IsEmail()) return Result<bool>.Fail("邮箱地址格式不正确");

            return Result<bool>.Successed;
        }
    }
}