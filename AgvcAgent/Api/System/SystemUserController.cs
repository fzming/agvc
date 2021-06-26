using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgvcAgent.Api.Kernel;
using AgvcCoreData.System;
using AgvcEntitys.System;
using AgvcService.System;
using AgvcService.System.Models;
using CoreData;
using Microsoft.AspNetCore.Mvc;
using Utility.Extensions;

namespace AgvcAgent.Api.System
{
    /// <summary>
    ///     系统用户服务接口
    /// </summary>
    [Route("api/sys/user")]
    public class SystemUserController : AuthorizedApiController
    {
        #region 注入

        private ISystemUserService SystemUserService { get; }
        private IAuthorityService AuthorityService { get; }

        /// <summary>
        ///     构造函数
        /// </summary>
        /// <param name="systemUserService"></param>
        /// <param name="authorityService"></param>
        public SystemUserController(ISystemUserService systemUserService,
            IAuthorityService authorityService)
        {
            SystemUserService = systemUserService;
            AuthorityService = authorityService;
        }

        #endregion

        #region 系统用户管理

        /// <summary>
        ///     查询所有系统用户
        /// </summary>
        /// <param name="userPageQuery"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("query")]
        public async Task<PageResult<SystemUserDto>> QuerySystemUsersAsync([FromBody] SystemUserPageQuery userPageQuery)
        {
            var pageResult = await SystemUserService.QuerySystemUsersAsync(userPageQuery);
            var users = pageResult.Datas.ToListEx();
            var datas = new List<SystemUserDto>();
            if (users.Any())
            {
                datas = users.MapTo<SystemUser, SystemUserDto>();
                var allRoles = (await AuthorityService.QueryAllRolesAsync()).ToListEx();

                datas.ForEach(d =>
                {
                    var role = allRoles.SingleOrDefault(p => p.Id == d.RoleId);
                    if (role == null) return;
                    d.RoleLevel = role.Level;
                    d.RoleName = role.Name;
                });
            }

            return new PageResult<SystemUserDto>
            {
                Datas = datas,
                PageCount = pageResult.PageCount,
                Total = pageResult.Total
            };
        }

        /// <summary>
        ///     创建系统用户
        /// </summary>
        /// <param name="userCreateModel">创建系统用户模型</param>
        /// <returns></returns>
        [HttpPost]
        [Route("create")]
        public Task<Result<SystemUser>> CreateSystemUserAsync([FromBody] SystemUserCreateModel userCreateModel)
        {
            return SystemUserService.CreateSystemUserAsync(userCreateModel, OrgId);
        }

        /// <summary>
        ///     更新系统用户
        /// </summary>
        /// <param name="userUpdateModel">更新系统用户模型</param>
        /// <returns></returns>
        [HttpPost]
        [Route("update")]
        public async Task<Result<SystemUser>> UpdateSystemUserAsync([FromBody] SystemUserUpdateModel userUpdateModel)
        {
            var us = await SystemUserService.UpdateSystemUserAsync(userUpdateModel);
            if (us.Success)
            {
                var roleChanged = us.Data.RoleId != userUpdateModel.RoleId;
                // if (roleChanged)
                // {
                //     await SignalrService.KickOffUserAsync(userUpdateModel.UserId,"您的角色已变化，请重新登陆");
                // }
            }

            return us;
        }

        /// <summary>
        ///     删除系统用户
        /// </summary>
        /// <param name="userid">用户ID</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("delete/{userid}")]
        public async Task<bool> DeleteSystemUserAsync(string userid)
        {
            //await SignalrService.KickOffUserAsync(userid, "您的账户已被删除");

            return await SystemUserService.DeleteSystemUserAsync(userid);
        }

        /// <summary>
        ///     获取管理用户资料档案
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("profile")]
        public Task<SystemUserProfile> GetUserProfileAsync()
        {
            return SystemUserService.GetUserProfileAsync(ClientId);
        }

        /// <summary>
        ///     更新用户资料档案
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("update-profile")]
        public Task<bool> UpdateUserProfileAsync([FromBody] UpdateProfileModel model)
        {
            return SystemUserService.UpdateUserProfileAsync(ClientId, model);
        }

        /// <summary>
        ///     修改密码
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("change-password")]
        public Task<bool> ChangeUserPasswordAsync([FromBody] ChangePasswordModel model)
        {
            return SystemUserService.ChangeUserPasswordAsync(ClientId, model);
        }

        #endregion
    }
}