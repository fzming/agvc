using CoreData;
using CoreData.Models;
using Utility;
using Utility.Extensions;

namespace AgvcService.Users.Models
{
    public abstract class AccountModel : IValidation
    {
        /// <summary>
        /// 登录ID
        /// </summary>
        public string LoginId { get; set; }

        /// <summary>
        /// 登录密码
        /// </summary>
        public string LoginPwd { get; set; }

        /// <summary>
        /// 分公司ID
        /// </summary>
        public string BranchCompanyId { get; set; }
        /// <summary>
        /// 部门ID
        /// </summary>
        public string DepartmentId { get; set; }
        /// <summary>
        /// 管理角色Id
        /// </summary>
        public string RoleId { get; set; }
        /// <summary>
        /// 姓名昵称
        /// </summary>
        public string Nick { get; set; }

        /// <summary>
        /// 源设备ID
        /// </summary>
        public string DeviceId { get; set; }
        /// <summary>
        /// 验证元数据
        /// </summary>
        /// <returns></returns>
        public virtual Result<bool> Validate()
        {
            return Result<bool>.Successed;
        }
    }
    public class CreateAccountModel : AccountModel
    {


        /// <summary>
        /// 性别
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        /// 头像地址
        /// </summary>
        public string Avatar { get; set; }
        /// <summary>
        /// 需更改密码
        /// </summary>
        public bool NeedChangePassword { get; set; }
        /// <summary>
        /// 禁止登陆
        /// 单纯创建用户，不设置用户名和密码
        /// </summary>
        public bool ForbiddenLogin { get; set; }
        /// <summary>
        /// 验证元数据
        /// </summary>
        /// <returns></returns>
        public override Result<bool> Validate()
        {
            if (!this.ForbiddenLogin)
            {
                if (LoginId.IsNullOrEmpty())
                {
                    return Result<bool>.Fail("账号必须传递");
                }
                if (LoginPwd.IsNullOrEmpty())
                {
                    return Result<bool>.Fail("密码必须传递");
                }
            }

            if (RoleId.IsNullOrEmpty())
            {
                return Result<bool>.Fail("账户的角色必须传递");
            }
            if (Nick.IsNullOrEmpty())
            {
                return Result<bool>.Fail("姓名必须传递");
            }
            if (Nick.IsNullOrEmpty())
            {
                return Result<bool>.Fail("账户的角色必须传递");
            }

            return Result<bool>.Successed;
        }
    }
}