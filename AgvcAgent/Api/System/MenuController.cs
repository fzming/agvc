using System.Collections.Generic;
using System.Threading.Tasks;
using AgvcAgent.Api.Kernel;
using AgvcService.System;
using AgvcService.System.Models;
using Microsoft.AspNetCore.Mvc;

namespace AgvcAgent.Api.System
{
    /// <summary>
    ///     菜单服务接口
    /// </summary>
    [Route("api/sys/menu")]
    public class MenuController : AuthorizedApiController
    {
        #region 注入

        private IMenuService MenuService { get; }

        public MenuController(IMenuService menuService)
        {
            MenuService = menuService;
        }

        #endregion

        #region 菜单管理

        public class RoleUser
        {
            /// <summary>
            ///     当前待授权的角色ID
            /// </summary>
            public string RoleId { get; set; }

            /// <summary>
            ///     当前待授权的用户ID
            /// </summary>
            public string UserId { get; set; }
        }

        /// <summary>
        ///     获取当前登录用户菜单指令列表
        /// </summary>
        /// <param name="hasCode">是否返回指令菜单，作为最后一级返回</param>
        /// <returns></returns>
        [HttpGet]
        [Route("query/{hasCode:bool=false}")]
        public Task<IEnumerable<RouteMenuDto>> QueryRouteMenuAsync([FromQuery] bool hasCode)
        {
            return MenuService.QueryRouteMenuAsync(OrgId, AuthorizedUser.RoleId, ClientId, false, string.Empty,
                string.Empty, hasCode);
        }

        /// <summary>
        ///     获取指定用户的菜单指令指令列表
        /// </summary>
        [HttpPost]
        [Route("user-auth-query")]
        public Task<IEnumerable<RouteMenuDto>> QueryUserAuthRouteMenuAsync([FromBody] RoleUser user)
        {
            return MenuService.QueryRouteMenuAsync(OrgId, AuthorizedUser.RoleId, ClientId,
                true, user.RoleId, user.UserId, true);
        }

        [HttpPost]
        [Route("create")]
        public Task<bool> CreateMenuAsync([FromBody] RouteMenuModel menu)
        {
            return MenuService.CreateMenuAsync(menu);
        }

        [HttpPost]
        [Route("update")]
        public Task<bool> UpdateMenuAsync([FromBody] RouteMenuModel menu)
        {
            return MenuService.UpdateMenuAsync(menu);
        }

        [HttpDelete]
        [Route("delete/{menuId}")]
        public Task<bool> DeleteMenuAsync(string menuId)
        {
            return MenuService.DeleteMenuAsync(menuId);
        }

        [HttpPost]
        [Route("update-order")]
        public Task<bool> UpdateMenuOrderAsync([FromBody] UpdateMenuOrderModel orderModel)
        {
            return MenuService.UpdateMenuOrderAsync(orderModel.MenuId, orderModel.Direction);
        }

        #endregion
    }
}