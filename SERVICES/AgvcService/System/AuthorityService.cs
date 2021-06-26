using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgvcEntitys.System.Authority;
using AgvcRepository.System.Interfaces.Authority;
using AgvcService.System.Models;
using Cache.IRedis.Interfaces;
using CoreService;
using Utility.Extensions;

namespace AgvcService.System
{
    /// <summary>
    ///     权限控制服务
    /// </summary>
    public class AuthorityService : AbstractService, IAuthorityService
    {
        #region 注入

        /// <summary>
        ///     指令前缀
        /// </summary>
        private const string ZlKey = "zl-";

        private IRoleRepository RoleRepository { get; }
        private IAuthorityCodeRepository AuthorityCodeRepository { get; }
        private IUserAuthorityRepository UserAuthorityRepository { get; }
        private IRedisHashCache RedisHashCache { get; }


        public AuthorityService(IRoleRepository roleRepository,
            IAuthorityCodeRepository authorityCodeRepository,
            IUserAuthorityRepository userAuthorityRepository,
            IRedisHashCache redisHashCache)
        {
            RoleRepository = roleRepository;
            AuthorityCodeRepository = authorityCodeRepository;
            UserAuthorityRepository = userAuthorityRepository;
            RedisHashCache = redisHashCache;
        }

        #endregion

        #region 角色管理

        /// <summary>
        ///     查询角色列表
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public Task<IEnumerable<Role>> QueryRolesAsync(string orgId)
        {
            return RoleRepository.FindAsync(p => p.OrgId == orgId);
        }

        public Task<IEnumerable<Role>> QueryRolesAsync()
        {
            return RoleRepository.FindAllAsync();
        }


        /// <summary>
        ///     创建角色
        /// </summary>
        /// <param name="model"></param>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public async Task<Role> CreateRoleAsync(CreateRoleModel model, string orgId)
        {
            if (model.Name.IsNullOrEmpty() ||
                await RoleRepository.AnyAsync(p => p.Name == model.Name && p.OrgId == orgId))
                throw new Exception("角色名称为空或有重复");

            var role = model.MapTo(new Role());

            role.OrgId = orgId;
            await RoleRepository.InsertAsync(role);
            await SyncRoleAuthoritysAsync(model.Authorizes, role);
            return role;
        }


        /// <summary>
        ///     同步角色权限
        /// </summary>
        /// <param name="modelAuthorizes">菜单ID或指令ID列表</param>
        /// <param name="role">角色对象</param>
        /// <returns></returns>
        public async Task SyncRoleAuthoritysAsync(string[] modelAuthorizes, Role role)
        {
            //清除所有角色权限
            if (modelAuthorizes.AnyNullable() == false)
            {
                await UserAuthorityRepository.DeleteAsync(p =>
                    p.AuthorId == role.Id && p.AuthorType == AuthorizedType.Role);
                return;
            }


            //所有角色权限
            var roleAuthoritys =
                (await UserAuthorityRepository.FindAsync(p =>
                    p.AuthorId == role.Id && p.AuthorType == AuthorizedType.Role)).ToList();
            var authorizedMenus = modelAuthorizes.Where(p => p.Contains(ZlKey) == false);
            var authorizedCodes = modelAuthorizes.Where(p => p.StartsWith(ZlKey));

            var newAuthoritys = new List<UserAuthority>();
            var deleteAuthoritys = new List<string>();

            roleAuthoritys.ForEach(ra =>
            {
                if (ra.MenuId.IsNotNullOrEmpty() && authorizedMenus.Contains(ra.MenuId) == false) //角色菜单权限:删除
                    deleteAuthoritys.Add(ra.Id); //标记删除菜单权限
                else if (ra.AuthCode.IsNotNullOrEmpty() && authorizedCodes.Contains(ra.AuthCode) == false) //角色指令权限：删除
                    deleteAuthoritys.Add(ra.Id); //标记删除指令权限
            });
            //菜单权限分析
            authorizedMenus.ToList().ForEach(am =>
            {
                if (!roleAuthoritys.Any(p => p.MenuId == am)) //角色菜单权限:新增
                    newAuthoritys.Add(new UserAuthority
                    {
                        MenuId = am,
                        AuthorId = role.Id,
                        AuthorType = AuthorizedType.Role
                    });
            });
            //指令权限分析
            authorizedCodes.ToList().ForEach(ac =>
            {
                if (!roleAuthoritys.Any(p => p.AuthCode == ac)) //角色指令权限:新增
                    newAuthoritys.Add(new UserAuthority
                    {
                        AuthCode = ac,
                        AuthorId = role.Id,
                        AuthorType = AuthorizedType.Role
                    });
            });
            //删除权限设定
            if (deleteAuthoritys.Any()) await UserAuthorityRepository.DeleteAsync(p => deleteAuthoritys.Contains(p.Id));

            //新增权限设定
            if (newAuthoritys.Any()) await UserAuthorityRepository.InsertAsync(newAuthoritys);
        }

        /// <summary>
        ///     删除角色
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteRoleAsync(string roleId)
        {
            //删除角色时，需要删除已赋予角色的权限
            await UserAuthorityRepository.DeleteAsync(p => p.AuthorType == AuthorizedType.Role && p.AuthorId == roleId);
            //删除角色
            return await RoleRepository.DeleteAsync(roleId);
        }

        /// <summary>
        ///     更新角色
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool> UpdateRoleAsync(UpdateRoleModel model)
        {
            var role = await RoleRepository.GetAsync(model.RoleId);
            if (role == null) throw new Exception("角色不存在");
            role = model.MapTo(role);
            var ok = await RoleRepository.UpdateAsync(role);
            if (ok) await SyncRoleAuthoritysAsync(model.Authorizes, role);
            return ok;
        }

        #endregion

        #region 用户权限管理

        /// <summary>
        ///     获取用户的所有权限定义（包含从角色中继承的权限）
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        public async Task<IEnumerable<UserAuthority>> GetUserAuthoritysAsync(string roleId, string userId)
        {
            //角色权限定义
            var roleAuthoritys = (await GetRoleAuthoritysAsync(roleId)).ToList();
            if (userId.IsNullOrEmpty()) return roleAuthoritys;
            //用户权限定义
            var userAuthoritys =
                (await UserAuthorityRepository.FindAsync(
                    p => p.AuthorId == userId && p.AuthorType == AuthorizedType.User)).ToList();
            //新增的权限
            var newAuthoritys =
                userAuthoritys.Where(p => p.UserDeny == false && !roleAuthoritys.Any(k => k.Id == p.Id)).ToList();
            if (newAuthoritys.Any()) roleAuthoritys.AddRange(newAuthoritys);

            //删除已禁用的权限
            var denyAuthoritys = userAuthoritys.Where(p => p.UserDeny).ToList();
            if (denyAuthoritys.Any())

            {
                var denyMenus = denyAuthoritys.Where(p => p.MenuId.IsNotNullOrEmpty())
                    .GroupBy(p => p.MenuId).Select(p => p.Key);
                var denyCodes = denyAuthoritys.Where(p => p.AuthCode.IsNotNullOrEmpty())
                    .GroupBy(p => p.AuthCode).Select(p => p.Key);

                if (denyMenus.Any())
                    roleAuthoritys.RemoveAll(p => denyMenus.Contains(p.MenuId));
                if (denyCodes.Any())
                    roleAuthoritys.RemoveAll(p => denyCodes.Contains(p.AuthCode));
            }


            return roleAuthoritys;
        }

        /// <summary>
        ///     同步用户权限管理
        /// </summary>
        /// <param name="modelAuthorizes">菜单或指令权限</param>
        /// <param name="roleId">角色ID</param>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        public async Task<bool> SyncUserAuthoritysAsync(string[] modelAuthorizes, string roleId, string userId)
        {
            try
            {
                var allAuthoritys = (await UserAuthorityRepository.FindAsync(p =>
                    p.AuthorId == userId && p.AuthorType == AuthorizedType.User ||
                    p.AuthorId == roleId && p.AuthorType == AuthorizedType.Role)).ToList();
                //当前已经在用户中存在的菜单ID和指令ID
                var currentModelAuthorizes = new List<string>();
                currentModelAuthorizes.AddRange(allAuthoritys.Where(p => p.MenuId.IsNotNullOrEmpty())
                    .GroupBy(p => p.MenuId).Select(p => p.Key));
                currentModelAuthorizes.AddRange(allAuthoritys.Where(p => p.AuthCode.IsNotNullOrEmpty())
                    .GroupBy(p => p.AuthCode).Select(p => p.Key));
                //所有角色权限
                var roleAuthoritys = allAuthoritys.Where(p => p.AuthorType == AuthorizedType.Role).ToList();
                //所有用户权限
                var userAuthoritys = allAuthoritys.Where(p => p.AuthorType == AuthorizedType.User).ToList();

                var authorizedMenus = modelAuthorizes.Where(p => p.Contains(ZlKey) == false).ToList();
                var authorizedCodes = modelAuthorizes.Where(p => p.StartsWith(ZlKey)).ToList();

                //待新增的用户权限
                var newAuthoritys = new List<UserAuthority>();
                //待禁止的用户权限
                var denyAuthoritys = new List<string>();
                //待删除的用户权限
                var deleteAuthoritys = new List<string>();

                #region 判断新增权限：对应的角色中不存在

                //菜单权限分析
                authorizedMenus.ForEach(am =>
                {
                    //角色权限中和当前用户权限中不存在
                    var roleHasMenu = roleAuthoritys.Any(p => p.MenuId == am);
                    var userHasMenu = userAuthoritys.Any(p => p.MenuId == am);
                    if (!roleHasMenu && !userHasMenu) //菜单权限:新增
                        newAuthoritys.Add(new UserAuthority
                        {
                            MenuId = am,
                            AuthorId = userId,
                            AuthorType = AuthorizedType.User,
                            UserDeny = false
                        });
                });
                //指令权限分析
                authorizedCodes.ForEach(ac =>
                {
                    if (!roleAuthoritys.Any(p => p.AuthCode == ac) &&
                        !userAuthoritys.Any(p => p.AuthCode == ac)) //指令权限:新增
                        newAuthoritys.Add(new UserAuthority
                        {
                            AuthCode = ac,
                            AuthorId = userId,
                            AuthorType = AuthorizedType.User,
                            UserDeny = false
                        });
                });

                #endregion

                #region 判断禁用权限：取消勾选

                var canceledAuthorizes = currentModelAuthorizes.Where(p => !modelAuthorizes.Contains(p)).ToList();

                canceledAuthorizes.ForEach(au =>
                {
                    if (au.StartsWith(ZlKey)) //指令
                    {
                        if (!userAuthoritys.Any(p => p.AuthCode == au))
                            newAuthoritys.Add(new UserAuthority
                            {
                                AuthCode = au,
                                AuthorId = userId,
                                AuthorType = AuthorizedType.User,
                                UserDeny = true
                            });
                    }
                    else //菜单
                    {
                        if (!userAuthoritys.Any(p => p.MenuId == au))
                            newAuthoritys.Add(new UserAuthority
                            {
                                MenuId = au,
                                AuthorId = userId,
                                AuthorType = AuthorizedType.User,
                                UserDeny = true
                            });
                    }
                });

                #endregion

                #region 判断修改权限

                //对已有用户权限进行设置
                userAuthoritys.ForEach(ra =>
                {
                    #region 已有的菜单权限

                    if (ra.MenuId.IsNotNullOrEmpty())
                    {
                        var roleHasMenu = roleAuthoritys.Any(p => p.MenuId == ra.MenuId);
                        var menuChoice = authorizedMenus.Contains(ra.MenuId);
                        if (!menuChoice) //未选择
                        {
                            //角色包含此权限
                            if (roleHasMenu)
                            {
                                if (!ra.UserDeny) denyAuthoritys.Add(ra.Id); //禁用此权限
                            }
                            else
                            {
                                deleteAuthoritys.Add(ra.Id); //删除
                            }
                        }
                        else //已选择
                        {
                            //角色包含权限
                            if (roleHasMenu) deleteAuthoritys.Add(ra.Id); //删除
                        }

                        return;
                    }

                    #endregion


                    #region 针对已有的指令权限

                    if (ra.AuthCode.IsNullOrEmpty()) return;

                    var roleHasCode = roleAuthoritys.Any(p => p.AuthCode == ra.AuthCode);
                    var codeChoice = authorizedCodes.Contains(ra.AuthCode);

                    if (!codeChoice) //未选择
                    {
                        //角色包含此权限
                        if (roleHasCode)
                        {
                            if (ra.UserDeny == false)
                                denyAuthoritys.Add(ra.Id); //禁用指令权限
                        }
                        else
                        {
                            deleteAuthoritys.Add(ra.Id); //删除
                        }
                    }
                    else //进行了选择
                    {
                        //角色包含权限
                        if (roleHasCode) deleteAuthoritys.Add(ra.Id); //删除
                    }

                    #endregion
                });

                #endregion

                if (newAuthoritys.Any()) await UserAuthorityRepository.InsertAsync(newAuthoritys);

                if (denyAuthoritys.Any()) await UserAuthorityRepository.DenyAuthoritysAsync(denyAuthoritys, true);

                if (deleteAuthoritys.Any())
                    await UserAuthorityRepository.DeleteAsync(p => deleteAuthoritys.Contains(p.Id));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public async Task<bool> AddUserAuthorityCodeAsync(UserAuthority userAuthority)
        {
            if (await UserAuthorityRepository.AnyAsync(p =>
                p.AuthCode == userAuthority.AuthCode && p.AuthorId == userAuthority.AuthorId)) return false;
            await UserAuthorityRepository.InsertAsync(userAuthority);
            return true;
        }

        #endregion

        #region 指令管理

        /// <summary>
        ///     查询指令列表
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        public Task<IEnumerable<AuthorityCode>> QueryCodesAsync(string menuId = "")
        {
            if (menuId.IsNullOrEmpty()) return AuthorityCodeRepository.FindAllAsync();

            return AuthorityCodeRepository.FindAsync(p => p.MenuId == menuId);
        }

        /// <summary>
        ///     创建指令
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<AuthorityCode> CreateCodeAsync(CreateCodeModel model)
        {
            var entity = model.MapTo(new AuthorityCode());
            entity.Code = $"{ZlKey}{Guid.NewGuid():N}";
            await AuthorityCodeRepository.InsertAsync(entity);
            return entity;
        }

        /// <summary>
        ///     更新指令
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool> UpdateCodeAsync(UpdateCodeModel model)
        {
            var entity = await AuthorityCodeRepository.GetAsync(model.CodeId);
            if (entity == null) throw new Exception("指定ID的指令不存在");

            model.MapTo(entity);
            await AuthorityCodeRepository.ReplaceAsync(model.CodeId, entity);
            return true;
        }

        /// <summary>
        ///     删除指令权限
        /// </summary>
        /// <param name="codeId"></param>
        /// <returns></returns>
        public Task<bool> DeleteCodeAsync(string codeId)
        {
            return AuthorityCodeRepository.DeleteAsync(codeId);
        }

        private async Task<bool> DeleteCodeAsync(AuthorityCode code)
        {
            var authCode = code.Code; //指令编码
            //删除已赋予用户的指令权限
            await UserAuthorityRepository.DeleteAsync(p => p.AuthCode == authCode);
            return await AuthorityCodeRepository.DeleteAsync(code.Id);
        }

        /// <summary>
        ///     删除菜单关联的指令
        /// </summary>
        /// <param name="menuId">菜单ID</param>
        /// <returns></returns>
        public async Task DeleteMenuCodesAsync(string menuId)
        {
            //删除已赋予用户的菜单权限
            await UserAuthorityRepository.DeleteAsync(p => p.MenuId == menuId);
            //删除菜单关联的指令
            var codes = await AuthorityCodeRepository.FindAsync(p => p.MenuId == menuId);
            await codes.ForEachAsync(DeleteCodeAsync);
        }

        /// <summary>
        ///     获取菜单的角色ID列表
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        public async Task<string[]> GetMenuRolesAsync(string menuId)
        {
            var roleMenus =
                await UserAuthorityRepository.FindAsync(p => p.MenuId == menuId && p.AuthorType == AuthorizedType.Role);
            return roleMenus.GroupBy(p => p.AuthorId).Select(p => p.Key).ToArray();
        }

        public Task<IEnumerable<UserAuthority>> GetRoleAuthoritysAsync(string roleId)
        {
            return UserAuthorityRepository.FindAsync(p => p.AuthorId == roleId && p.AuthorType == AuthorizedType.Role);
        }

        /// <summary>
        ///     是否是系统超级管理角色
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public async Task<bool> IsSupperRoleAsync(string roleId)
        {
            var role = await RoleRepository.GetAsync(roleId);
            if (role == null) return false;

            return role.Level > 99;
        }

        /// <summary>
        ///     查询角色授权码
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<RoleAuthorityCodeDto>> QueryUserAuthorityCodesAsync(string roleId, string userId)
        {
            var roleAuthoritys = GetUserAuthoritysAsync(roleId, userId);
            var allCodes = QueryCodesAsync();
            await Task.WhenAll(roleAuthoritys, allCodes);
            var roleCodes = roleAuthoritys.Result.Where(p => p.AuthCode.IsNotNullOrEmpty()).ToList();
            var enableCodes = allCodes.Result.Where(p => p.Disabled == false); //有效的指令
            var dtos = enableCodes.MapTo<AuthorityCode, RoleAuthorityCodeDto>(
                map => map.ForMember(p => p.HasPermission,
                    opt => opt.MapFrom(src => roleCodes.AnyNullable(c => c.AuthCode == src.Code)))
            );

            //            dtos.ForEach(dto => { dto.HasPermission = roleCodes.Any(c => c.AuthCode == dto.Code); });
            return dtos;
        }

        /// <summary>
        ///     返回所有角色
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<Role>> QueryAllRolesAsync()
        {
            return RoleRepository.FindAllAsync();
        }

        /// <summary>
        ///     获取角色
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public async Task<RoleDto> GetRoleAsync(string roleId)
        {
            var role = await RoleRepository.GetAsync(roleId);
            return role.MapTo(new RoleDto());
        }

        public async Task<bool> ClearOrganizationAuthAsync(string orgId)
        {
            var task0 = RoleRepository.DeleteAsync(p => p.OrgId == orgId);
            var task1 = UserAuthorityRepository.DeleteAsync(p => p.OrgId == orgId);
            await Task.WhenAll(task0, task1);
            return true;
        }

        public async Task CacheUserAuthoritysAsync(string orgId, string userId, IEnumerable<string> menuIdArray,
            IEnumerable<string> codeArray)
        {
            await RedisHashCache.HashSetAsync("UserMenuAuth-" + orgId, userId, menuIdArray);
            await RedisHashCache.HashSetAsync("UserCodeAuth-" + orgId, userId, codeArray);
        }

        /// <summary>
        ///     获取包含某个指令权限的登录用户
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetCodeUsersFromCacheAsync(string orgId, string code)
        {
            var orgCodes = await RedisHashCache.HashAllAsync<IEnumerable<string>>("UserCodeAuth-" + orgId);
            if (!orgCodes.AnyNullable()) return Enumerable.Empty<string>();
            return orgCodes.Where(p => p.Value.Contains(code)).Select(p => p.Key);
        }

        /// <summary>
        ///     删除用户授权
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public Task DeleteAuthoritysAsync(string userid)
        {
            return UserAuthorityRepository.DeleteAsync(p =>
                p.AuthorType == AuthorizedType.User && p.AuthorId == userid);
        }

        #endregion

        // /// <summary>
        // /// 为机构克隆角色
        // /// </summary>
        // /// <param name="fromOrgId"></param>
        // /// <param name="toOrgId"></param>
        // /// <returns>旧RoleId,新RoleId</returns>
        // public async Task<IEnumerable<Role>> CloneRolesAsync(string fromOrgId, string toOrgId)
        // {
        //     if((await QueryRolesAsync(toOrgId)).AnyNullable()) throw  new Exception("新的机构不允许存在任何角色");
        //     var oRoles = await QueryRolesAsync(fromOrgId);
        //     var nRoles = oRoles.Select(role =>
        //     {
        //         var nRole = role.MapTo(new Role());
        //         nRole.Id = string.Empty;
        //         nRole.OrgId = toOrgId;
        //         nRole.CloneId = role.Id;
        //         return nRole;
        //     });
        //     await RoleRepository.InsertAsync(nRoles);
        //     return nRoles;
        // }
        //
        // /// <summary>
        // /// 为结构克隆权限
        // /// </summary>
        // /// <param name="fromOrgId"></param>
        // /// <param name="toOrgId"></param>
        // /// <returns></returns>
        // public async Task<IEnumerable<UserAuthority>> CloneUserAuthoritiesAsync(string fromOrgId,string toOrgId)
        // {
        //     //原机构用户权限表
        //     if ((await UserAuthorityRepository.AnyAsync(p=>p.OrgId==toOrgId))) throw new Exception("新的机构不允许存在任何用户权限");
        //     var oUserAuthorities = await UserAuthorityRepository.FindAsync(p => p.OrgId == fromOrgId);
        //
        //     var nAuthorities = oUserAuthorities.Select(authority =>
        //     {
        //         var nAuth = authority.MapTo(new UserAuthority());
        //         nAuth.Id = string.Empty;
        //         nAuth.OrgId = toOrgId;
        //         nAuth.CloneId = authority.Id;
        //         return nAuth;
        //     });
        //     await UserAuthorityRepository.InsertAsync(nAuthorities);
        //     return nAuthorities;
        // }
    }
}