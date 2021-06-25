using System.Collections.Generic;
using System.Threading.Tasks;
using AgvcCoreData.System;
using AgvcEntitys.System;
using AgvcService.System.Models;
using CoreData;
using CoreService.Interfaces;

namespace AgvcService.System
{
    /// <summary>
    /// 系统用户服务
    /// </summary>
    public interface ISystemUserService:IService
    {
        /// <summary>
        /// 分页查询当前系统用户列表
        /// </summary>
        /// <param name="userPageQuery"></param>
        /// <returns></returns>
        Task<PageResult<SystemUser>> QuerySystemUsersAsync(SystemUserPageQuery userPageQuery);
        Task<IEnumerable<SystemUser>> QueryRoleAccountsAsync(string orgId, string roleId);
        /// <summary>
        /// 修复昵称拼音字母
        /// </summary>
        /// <returns></returns>
        Task BatchFixNickPinyinAsync();
        /// <summary>
        /// 创建系统用户
        /// </summary>
        /// <param name="userCreateModel"></param>
        /// <param name="orgId"></param>
        /// <returns></returns>
        Task<Result<SystemUser>> CreateSystemUserAsync(SystemUserCreateModel userCreateModel, string orgId);
        /// <summary>
        /// 更新系统用户
        /// </summary>
        /// <param name="userUpdateModel"></param>
        /// <returns></returns>
        Task<Result<SystemUser>> UpdateSystemUserAsync(SystemUserUpdateModel userUpdateModel);
        /// <summary>
        /// 根据用户账号查找用户实体
        /// </summary>
        /// <param name="loginId"></param>
        /// <returns></returns>
        Task<SystemUser> FindSystemUserByLoginId(string loginId);
        /// <summary>
        /// 删除系统用户
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        Task<bool> DeleteSystemUserAsync(string userid);
        /// <summary>
        /// 系统用户登录
        /// </summary>
        /// <param name="loginModel"></param>
        /// <returns></returns>
        Task<Result<SystemUser>> LoginAsync(SystemUserLoginModel loginModel);
        /// <summary>
        /// 获取系统用户的资料
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        Task<SystemUserProfile> GetUserProfileAsync(string clientId);
        /// <summary>
        /// 更新系统用户档案
        /// </summary>
        /// <param name="clientId">系统用户ID</param>
        /// <param name="model">更新模型</param>
        /// <returns></returns>
        Task<bool> UpdateUserProfileAsync(string clientId, UpdateProfileModel model);
        /// <summary>
        /// 修改系统用户当前密码
        /// </summary>
        /// <param name="clientId">系统用户ID</param>
        /// <param name="model">修改模型</param>
        /// <returns></returns>
        Task<bool> ChangeUserPasswordAsync(string clientId, ChangePasswordModel model);


        Task<bool> ClearOrganizationUserAsync(string orgId);
    }
}