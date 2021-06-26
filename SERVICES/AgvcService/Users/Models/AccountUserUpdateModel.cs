using CoreData;
using Utility.Extensions;

namespace AgvcService.Users.Models
{
    public class AccountUserUpdateModel : AccountModel
    {
        /// <summary>
        ///     待修改的用户ID
        /// </summary>
        public string Id { get; set; }


        public override Result<bool> Validate()
        {
            if (LoginId.IsNullOrEmpty())
                return Result<bool>.Fail("登录ID不能为空");

            if (RoleId.IsNullOrEmpty())
                return Result<bool>.Fail("用户角色不能为空");

            if (Nick.IsNullOrEmpty())
                return Result<bool>.Fail("姓名不能为空");

            return Result<bool>.Successed;
        }
    }
}