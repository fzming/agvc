using CoreData;
using CoreData.Models;

namespace AgvcService.System.Models
{
    /// <summary>
    /// 系统用户创建模型
    /// </summary>
    public class SystemUserCreateModel : IValidation
    {
        /// <summary>
        /// 登录ID 
        /// </summary>
        /// <remarks>
        /// 系统唯一，账号，手机号,邮箱，用户名
        /// </remarks>
        public string LoginId { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 登录密码
        /// </summary>
        public string LoginPwd { get; set; }
        /// <summary>
        /// 用户昵称
        /// </summary>
        public string Nick { get; set; }
        /// <summary>
        /// 所属角色Id
        /// </summary>
        public string RoleId { get; set; }
        /// <summary>
        /// 头像地址
        /// </summary>
        public string Avatar { get; set; }
        /// <summary>
        /// 需更改密码
        /// </summary>
        public bool NeedChangePassword { get; set; }
        public virtual Result<bool> Validate()
        {
            if (LoginId.IsNullOrEmpty())
                return Result<bool>.Fail("用户不能为空");
            if (LoginPwd.IsNullOrEmpty())
                return Result<bool>.Fail("密码不能为空");

            if (RoleId.IsNullOrEmpty())
                return Result<bool>.Fail("请选择所属角色");
            return Result<bool>.Successed;
        }
    }
}