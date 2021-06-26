using System.Collections.Generic;
using System.Threading.Tasks;
using AgvcCoreData.System;
using AgvcEntitys.System;
using AgvcService.System.Models;

namespace AgvcService.System
{
    /// <summary>
    ///     菜单服务
    /// </summary>
    public interface IMenuService
    {
        /// <summary>
        ///     获取路由菜单
        /// </summary>
        /// <param name="orgId">当前调用者机构ID</param>
        /// <param name="roleId">当前调用者角色ID</param>
        /// <param name="userId">当前调用者用户ID</param>
        /// <param name="authMode">是否是授权模式菜单</param>
        /// <param name="authRoleId">被授权角色ID，仅authMode=true时需传递，否则不会被采用</param>
        /// <param name="authUserId">被授权用户ID，仅authMode=true时需传递，否则不会被采用</param>
        /// <param name="hasCode">是否返回指令菜单，作为最后一级返回</param>
        Task<IEnumerable<RouteMenuDto>> QueryRouteMenuAsync(string orgId,
            string roleId,
            string userId,
            bool authMode,
            string authRoleId,
            string authUserId,
            bool hasCode);

        Task<IEnumerable<Menu>> QueryModuleMenusAsync(ModuleType[] modules);
        Task<IEnumerable<Menu>> QueryOrgMenusAsync(string orgId);

        /// <summary>
        ///     创建菜单
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        Task<bool> CreateMenuAsync(RouteMenuModel menu);

        /// <summary>
        ///     删除菜单
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        Task<bool> DeleteMenuAsync(string menuId);

        /// <summary>
        ///     更新菜单
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        Task<bool> UpdateMenuAsync(RouteMenuModel menu);

        /// <summary>
        ///     更新菜单的隶属模块列表
        /// </summary>
        /// <param name="menuModule"></param>
        /// <returns></returns>
        Task<bool> UpdateMenuModulesAsync(ChangeMenuModule menuModule);

        /// <summary>
        ///     更改菜单的同级排序
        /// </summary>
        /// <param name="menuId">菜单ID</param>
        /// <param name="direction">排序方向</param>
        /// <returns></returns>
        Task<bool> UpdateMenuOrderAsync(string menuId, OrderDirection direction);

        /// <summary>
        ///     查找所有菜单
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Menu>> FindAllMenusAsync();
    }
}