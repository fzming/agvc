using System.Collections.Generic;
using System.Threading.Tasks;
using AgvcEntitys.System.Authority;
using AgvcService.System.Models;
using CoreService.Interfaces;

namespace AgvcService.System
{
    /// <summary>
    ///     权限角色服务接口
    /// </summary>
    public interface IAuthorityService : IService
    {
        /// <summary>
        ///     清除某个机构所有权限设置
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        Task<bool> ClearOrganizationAuthAsync(string orgId);

        Task CacheUserAuthoritysAsync(string orgId, string userId, IEnumerable<string> menuIdArray,
            IEnumerable<string> codeArray);

        /// <summary>
        ///     获取包含某个指令权限的登录用户
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        Task<IEnumerable<string>> GetCodeUsersFromCacheAsync(string orgId, string code);


        Task DeleteAuthoritysAsync(string userid);

        #region 角色管理

        /// <summary>
        ///     查询当前机构所有角色
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        Task<IEnumerable<Role>> QueryRolesAsync(string orgId);

        Task<IEnumerable<Role>> QueryRolesAsync();

        /// <summary>
        ///     创建角色
        /// </summary>
        /// <param name="model"></param>
        /// <param name="orgId"></param>
        /// <returns></returns>
        Task<Role> CreateRoleAsync(CreateRoleModel model, string orgId);

        /// <summary>
        ///     删除角色
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        Task<bool> DeleteRoleAsync(string roleId);

        /// <summary>
        ///     更新角色
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool> UpdateRoleAsync(UpdateRoleModel model);

        #endregion

        #region 指令权限

        /// <summary>
        ///     查询指令集
        /// </summary>
        /// <param name="menuId">是否根据菜单ID进行过滤</param>
        /// <returns></returns>
        Task<IEnumerable<AuthorityCode>> QueryCodesAsync(string menuId = "");

        /// <summary>
        ///     创建指令
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<AuthorityCode> CreateCodeAsync(CreateCodeModel model);

        /// <summary>
        ///     更新指令
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool> UpdateCodeAsync(UpdateCodeModel model);

        /// <summary>
        ///     删除指令
        /// </summary>
        /// <param name="codeId"></param>
        /// <returns></returns>
        Task<bool> DeleteCodeAsync(string codeId);

        /// <summary>
        ///     删除菜单关联的指令
        /// </summary>
        /// <param name="menuId">菜单ID</param>
        /// <returns></returns>
        Task DeleteMenuCodesAsync(string menuId);

        #endregion

        #region 角色权限

        /// <summary>
        ///     同步角色权限
        /// </summary>
        /// <param name="modelAuthorizes">菜单ID或指令ID列表</param>
        /// <param name="role">目标角色</param>
        /// <returns></returns>
        Task SyncRoleAuthoritysAsync(string[] modelAuthorizes, Role role);

        /// <summary>
        ///     获取菜单的角色列表
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        Task<string[]> GetMenuRolesAsync(string menuId);

        /// <summary>
        ///     获取角色的所有权限定义
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        Task<IEnumerable<UserAuthority>> GetRoleAuthoritysAsync(string roleId);

        /// <summary>
        ///     是否是系统超级管理角色
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        Task<bool> IsSupperRoleAsync(string roleId);

        /// <summary>
        ///     查询角色授权码
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<IEnumerable<RoleAuthorityCodeDto>> QueryUserAuthorityCodesAsync(string roleId, string userId);

        /// <summary>
        ///     返回所有角色
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Role>> QueryAllRolesAsync();

        /// <summary>
        ///     获取角色
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        Task<RoleDto> GetRoleAsync(string roleId);

        #endregion

        #region 用户权限

        /// <summary>
        ///     获取用户的所有权限定义（包含从角色中继承的权限）
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        Task<IEnumerable<UserAuthority>> GetUserAuthoritysAsync(string roleId, string userId);

        /// <summary>
        ///     同步用户权限管理
        /// </summary>
        /// <param name="modelAuthorizes">菜单或指令权限</param>
        /// <param name="roleId">角色ID</param>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        Task<bool> SyncUserAuthoritysAsync(string[] modelAuthorizes, string roleId, string userId);

        Task<bool> AddUserAuthorityCodeAsync(UserAuthority userAuthority);

        #endregion
    }
}