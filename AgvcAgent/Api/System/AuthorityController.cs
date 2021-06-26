using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AgvcAgent.Api.Kernel;
using AgvcEntitys.System.Authority;
using AgvcService.System;
using AgvcService.System.Models;
using AgvcService.Users;
using Microsoft.AspNetCore.Mvc;
using Utility.Extensions;

namespace AgvcAgent.Api.System
{
    /// <summary>
    ///     权限接口
    /// </summary>
    [Route("api/sys/auth")]
    public class AuthorityController : AuthorizedApiController
    {
        #region 注入

        private IAuthorityService AuthorityService { get; }
        private IAccountService AccountService { get; }
        private ISystemUserService SystemUserService { get; }

        public AuthorityController(IAuthorityService authorityService, IAccountService accountService,
            ISystemUserService systemUserService)
        {
            AuthorityService = authorityService;
            AccountService = accountService;
            SystemUserService = systemUserService;
        }

        #endregion

        #region 角色管理

        /// <summary>
        ///     获取所有角色
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("roles")]
        public async Task<IEnumerable<Role>> QueryRolesAsync()
        {
            return await AuthorityService.QueryRolesAsync(OrgId);
        }

        /// <summary>
        ///     创建角色
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("role/create")]
        public async Task<Role> CreateRoleAsync([FromBody] CreateRoleModel model)
        {
            return await AuthorityService.CreateRoleAsync(model, OrgId);
        }

        /// <summary>
        ///     更新角色
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("role/update")]
        public async Task<bool> UpdateRoleAsync([FromBody] UpdateRoleModel model)
        {
            return await AuthorityService.UpdateRoleAsync(model);
        }

        /// <summary>
        ///     删除角色
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("role/delete/{roleId}")]
        public async Task<bool> DeleteRoleAsync(string roleId)
        {
            var accountUsersTask = AccountService.QueryRoleAccountsAsync(OrgId, roleId);
            var systemUsersTask = SystemUserService.QueryRoleAccountsAsync(OrgId, roleId);
            await Task.WhenAll(accountUsersTask, systemUsersTask);
            if (accountUsersTask.Result.AnyNullable() || systemUsersTask.Result.AnyNullable())
                throw new Exception("当前角色下包含用户，不能直接删除");
            return await AuthorityService.DeleteRoleAsync(roleId);
        }

        #endregion

        #region 用户权限

        public class UpdateUserAuthorityModel
        {
            public string[] Authorizes { get; set; }

            public string RoleId { get; set; }

            public string UserId { get; set; }
        }

        /// <summary>
        ///     更新用户权限
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("user/update")]
        public Task<bool> SyncUserAuthoritysAsync([FromBody] UpdateUserAuthorityModel model)
        {
            return AuthorityService.SyncUserAuthoritysAsync(model.Authorizes, model.RoleId, model.UserId);
        }

        /// <summary>
        ///     用户指令权限
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("user-codes")]
        public async Task<IEnumerable<RoleAuthorityCodeDto>> QueryUserAuthorityCodesAsync()
        {
            return await AuthorityService.QueryUserAuthorityCodesAsync(AuthorizedUser.RoleId, ClientId);
        }

        #endregion

        #region 指令管理

        /// <summary>
        ///     获取指令列表
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("codes")]
        public async Task<IEnumerable<AuthorityCode>> QueryCodesAsync(string menuId = "")
        {
            return await AuthorityService.QueryCodesAsync(menuId);
        }

        /// <summary>
        ///     创建指令
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("code/create")]
        public async Task<AuthorityCode> CreateCodeAsync([FromBody] CreateCodeModel model)
        {
            return await AuthorityService.CreateCodeAsync(model);
        }

        [HttpPost]
        [Route("code/update")]
        public async Task<bool> UpdateCodeAsync([FromBody] UpdateCodeModel model)
        {
            return await AuthorityService.UpdateCodeAsync(model);
        }

        [HttpDelete]
        [Route("code/delete/{codeId}")]
        public async Task<bool> DeleteCodeAsync(string codeId)
        {
            return await AuthorityService.DeleteCodeAsync(codeId);
        }

        #endregion
    }
}