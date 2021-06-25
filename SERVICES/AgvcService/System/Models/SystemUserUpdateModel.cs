using CoreData;

namespace AgvcService.System.Models
{
    /// <summary>
    /// 系统用户修改模型
    /// </summary>
    public class SystemUserUpdateModel : SystemUserCreateModel
    {
        /// <summary>
        /// 待修改人ID
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 数据完整性验证
        /// </summary>
        /// <returns></returns>
        public override Result<bool> Validate()
        {
            if (LoginId.IsNullOrEmpty())
                return Result<bool>.Fail("用户不能为空");
 
            if (RoleId.IsNullOrEmpty())
                return Result<bool>.Fail("请选择所属角色");
 

            if (UserId.IsNullOrEmpty())
                return Result<bool>.Fail("用户ID不能为空");
            return Result<bool>.Successed;
        }
    }
}