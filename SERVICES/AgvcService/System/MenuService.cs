using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgvcCoreData.System;
using AgvcEntitys.Organization;
using AgvcEntitys.System;
using AgvcEntitys.System.Authority;
using AgvcRepository.System.Interfaces;
using AgvcRepository.System.Interfaces.Authority;
using AgvcService.Organizations;
using AgvcService.System.Models;
using AutoMapper;
using CoreService;
using Utility.Extensions;

namespace AgvcService.System
{
    /// <summary>
    ///     系统菜单服务实现
    /// </summary>
    public class MenuService : AbstractService, IMenuService
    {
        private readonly Lazy<MapperConfiguration> _mapperConfiguration;

        #region 构造函数注入

        private IMenuRepository MenuRepository { get; }
        private IAuthorityService AuthorityService { get; }
        private IAuthorityCodeRepository AuthorityCodeRepository { get; }
        private IOrgnizationService OrgnizationService { get; }

        public MenuService(IMenuRepository menuRepository,
            IAuthorityService authorityService,
            IAuthorityCodeRepository authorityCodeRepository, IOrgnizationService orgnizationService)
        {
            MenuRepository = menuRepository;
            AuthorityService = authorityService;
            AuthorityCodeRepository = authorityCodeRepository;
            OrgnizationService = orgnizationService;
            _mapperConfiguration = new Lazy<MapperConfiguration>(() =>
            {
                return new MapperConfiguration(config =>
                {
                    config.CreateMap<Menu, RouteMenuDto>()
                        .ForMember(p => p.Children, opt => opt.Ignore())
                        .PreserveReferences()
                        .ReverseMap();
                });
            });
        }

        #endregion

        #region 查询

        /// <summary>
        ///     根据指定的模块列表，返回菜单项目
        /// </summary>
        /// <param name="modules">模块列表</param>
        /// <returns></returns>
        public Task<IEnumerable<Menu>> QueryModuleMenusAsync(ModuleType[] modules)
        {
            return MenuRepository.QueryModuleMenusAsync(modules);
        }

        /// <summary>
        ///     获取机构所有菜单
        /// </summary>
        /// <param name="orgId">机构ID</param>
        /// <returns></returns>
        public async Task<IEnumerable<Menu>> QueryOrgMenusAsync(string orgId)
        {
            var org = await OrgnizationService.GetOrgAsync(orgId);
            if (org == null) throw new Exception("用户所属机构不存在");
            var modules = org.Modules ?? Array.Empty<ModuleType>();
            if (org.PrimaryType == OrganizationType.System) //系统机构
                modules = typeof(ModuleType).AsEnumerable<ModuleType>().ToArray(); //系统机构默认拥有所有模块

            return await QueryModuleMenusAsync(modules);
        }

        /// <summary>
        ///     获取路由菜单
        /// </summary>
        /// <param name="orgId">当前机构ID</param>
        /// <param name="roleId">当前角色ID</param>
        /// <param name="userId">当前用户ID</param>
        /// <param name="authMode">是否是授权模式菜单</param>
        /// <param name="authRoleId">被授权角色ID，仅authMode=true时需传递（角色授权和用户授权都必须传递），否则不会被采用</param>
        /// <param name="authUserId">被授权用户ID，仅authMode=true且对用户授权时需传递，否则不会被采用</param>
        /// <param name="hasCode">是否返回指令菜单，作为最后一级返回</param>
        /// <returns></returns>
        public async Task<IEnumerable<RouteMenuDto>> QueryRouteMenuAsync(string orgId,
            string roleId,
            string userId,
            bool authMode, string authRoleId, string authUserId, bool hasCode)
        {
            #region 获取系统总菜单和总指令

            var menus = (await QueryOrgMenusAsync(orgId)).ToList();
            var codes = new List<AuthorityCode>();
            if (hasCode) codes = (await AuthorityCodeRepository.FindAllAsync()).ToList();

            #endregion

            var isSupperRole = await AuthorityService.IsSupperRoleAsync(roleId);

            //授权模式下:授权的菜单不能超过当前操作人员角色的范围
            if (authMode)
                if (!isSupperRole) //超级管理角色将跳过授权模式下的菜单控制
                {
                    var authoritys = (await AuthorityService.GetUserAuthoritysAsync(roleId, userId)).ToList();

                    //限定：当前用户拥有的菜单范围
                    var authorityMenus = authoritys.Where(p => p.MenuId.IsNotNullOrEmpty()).GroupBy(p => p.MenuId)
                        .Select(p => p.Key);

                    menus = menus.Where(p => authorityMenus.Contains(p.Id)).ToList();

                    if (codes.Any())
                    {
                        //限定：当前角色指令的范围
                        var authorityCodes = authoritys.Where(p => p.AuthCode.IsNotNullOrEmpty())
                            .GroupBy(p => p.AuthCode)
                            .Select(p => p.Key);
                        codes = codes.Where(p => authorityCodes.Contains(p.Code)).ToList();
                    }
                }

            var rootMenus = menus.Where(p => p.PaMenuId.IsNullOrEmpty()).OrderByDescending(p => p.OrderIndex)
                .ToList();
            var curRoleId = authMode ? authRoleId : roleId;
            var curUserId = authMode ? authUserId : userId;
            var curRoleAuthoritys = (await AuthorityService.GetUserAuthoritysAsync(curRoleId, curUserId)).ToList();

            var dtoMenus = rootMenus.MapTo<Menu, RouteMenuDto>(_mapperConfiguration.Value);
            var tasks = dtoMenus.Select(m => RecursionChildrenMenu(m, menus, codes, curRoleAuthoritys, curRoleId));
            await Task.WhenAll(tasks);

            return dtoMenus;
        }

        /// <summary>
        ///     创建菜单
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        public async Task<bool> CreateMenuAsync(RouteMenuModel menu)
        {
            var entity = menu.MapTo(new Menu());
            entity.Id = string.Empty;
            await MenuRepository.InsertAsync(entity);
            return true;
        }

        /// <summary>
        ///     删除菜单
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteMenuAsync(string menuId)
        {
            var hasChildren = await MenuRepository.AnyAsync(p => p.PaMenuId == menuId);
            if (!hasChildren)
            {
                var deleted = await MenuRepository.DeleteAsync(menuId);
                if (deleted) await AuthorityService.DeleteMenuCodesAsync(menuId);

                return deleted;
            }

            return false;
        }

        /// <summary>
        ///     更新菜单
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        public Task<bool> UpdateMenuAsync(RouteMenuModel menu)
        {
            if (menu.Id.IsNullOrEmpty()) throw new Exception("菜单ID不能为空");
            var entity = menu.MapTo(new Menu());
            return MenuRepository.ReplaceAsync(menu.Id, entity);
        }

        /// <summary>
        ///     更新菜单的隶属模块列表
        /// </summary>
        /// <param name="menuModule"></param>
        /// <returns></returns>
        public Task<bool> UpdateMenuModulesAsync(ChangeMenuModule menuModule)
        {
            if (menuModule.MenuId.IsNullOrEmpty()) throw new Exception("菜单ID不能为空");
            return MenuRepository.UpdateAsync(menuModule.MenuId, p => p.Modules, menuModule.Modules);
        }

        /// <summary>
        ///     更改菜单的同级排序
        /// </summary>
        /// <param name="menuId">菜单ID</param>
        /// <param name="direction">排序方向</param>
        /// <returns></returns>
        public async Task<bool> UpdateMenuOrderAsync(string menuId, OrderDirection direction)
        {
            var menu = await MenuRepository.GetAsync(menuId);
            var paid = menu.PaMenuId;
            var sibblings = (await MenuRepository.FindAsync(p => p.PaMenuId == paid))
                .OrderByDescending(p => p.OrderIndex).ToList();
            if (!sibblings.Any()) //没有兄弟节点，没有排序的意义
                return false;

            var j = 0;
            //修正排序
            for (var i = sibblings.Count - 1; i >= 0; i--)
            {
                sibblings[j].OrderIndex = i;
                j++;
            }

            //找出当前所处兄弟节点的位置索引
            var index = sibblings.FindIndex(p => p.Id == menu.Id);
            var menuIndex = menu.OrderIndex;

            switch (direction)
            {
                case OrderDirection.Up: //向上
                    if (index == 0) return false;


                    var prevIndex = sibblings[index - 1].OrderIndex;
                    sibblings[index].OrderIndex = prevIndex;
                    sibblings[index - 1].OrderIndex = menuIndex;

                    break;
                case OrderDirection.Down: //向下

                    if (index >= sibblings.Count - 1) return false;


                    var nextIndex = sibblings[index + 1].OrderIndex;
                    sibblings[index + 1].OrderIndex = menuIndex;
                    sibblings[index].OrderIndex = nextIndex;

                    break;
                default:
                    return false;
            }

            await Task.WhenAll(sibblings.Select(UpdateOrderIndexAsync));
            return true;
        }

        /// <summary>
        ///     查找所有菜单
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<Menu>> FindAllMenusAsync()
        {
            return MenuRepository.FindAllAsync();
        }

        private Task<bool> UpdateOrderIndexAsync(Menu menu)
        {
            return MenuRepository.UpdateAsync(menu, p => p.OrderIndex, menu.OrderIndex);
        }


        /// <summary>
        ///     递归获取子菜单
        /// </summary>
        /// <param name="menu">当前菜单</param>
        /// <param name="menus"></param>
        /// <param name="codes"></param>
        /// <param name="roleAuthorities">用户权限列表</param>
        /// <param name="roleId">当前用户角色</param>
        private async Task RecursionChildrenMenu(RouteMenuDto menu, IEnumerable<Menu> menus,
            IReadOnlyCollection<AuthorityCode> codes,
            IReadOnlyCollection<UserAuthority> roleAuthorities, string roleId)
        {
            var childrens = menus.Where(p => p.PaMenuId == menu.Id).OrderByDescending(p => p.OrderIndex).ToList();
            var menuHasAuthed = roleAuthorities.Any(p => p.MenuId == menu.Id);

            menu.Meta.Roles = menuHasAuthed ? new[] {roleId} : Enumerable.Empty<string>().ToArray();
            if (childrens.Any())
            {
                var dtoChildrens = childrens
                    .MapTo<Menu, RouteMenuDto>(_mapperConfiguration.Value)
                    .ToList();

                menu.Children = dtoChildrens.ToArray();


                var tasks = dtoChildrens.Select(child =>
                    RecursionChildrenMenu(child, menus, codes, roleAuthorities, roleId));
                await Task.WhenAll(tasks);
            }
            else if (codes.Any())
            {
                //构造指令菜单

                var codeChildrens = codes.Where(p => p.MenuId == menu.Id).ToList();
                if (codeChildrens.Any())
                    menu.Children = codeChildrens.Select(c => CreateMenuFromCode(c, roleAuthorities, roleId)).ToArray();
            }
        }

        private RouteMenuDto CreateMenuFromCode(AuthorityCode authorityCode,
            IReadOnlyCollection<UserAuthority> roleAuthorities, string roleId)
        {
            var roles = new List<string>();
            if (roleAuthorities.Any())
            {
                var codeHasAuthed = roleAuthorities.Any(p => p.AuthCode == authorityCode.Code);
                if (codeHasAuthed) roles.Add(roleId);
            }

            return new RouteMenuDto
            {
                Id = authorityCode.Code,
                PaMenuId = authorityCode.MenuId,
                Name = authorityCode.Name,
                Meta = new MenuMeta
                {
                    Title = authorityCode.Name,
                    Roles = roles.ToArray()
                }
            };
        }

        #endregion
    }
}