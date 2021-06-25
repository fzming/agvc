using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AgvcCoreData.System;
using AgvcEntitys.System;
using AgvcRepository.Orgnizations.Interfaces;
using AgvcRepository.System.Interfaces;
using AgvcService.System.Models;
using CoreData;
using CoreService;
using Utility.Extensions;

namespace AgvcService.System
{
    /// <summary>
    /// 创建系统管理人员
    /// </summary>
    [Export(typeof(ISystemUserService))]
    internal class SystemUserService : AbstractService, ISystemUserService
    {
        #region 注入

        private ISystemUserRepository SystemUserRepository { get; }
        private IAuthorityService AuthorityService { get; }

        private IOrganizationRepository OrganizationRepository { get; }

        [ImportingConstructor]
        public SystemUserService(ISystemUserRepository systemUserRepository,
            IAuthorityService authorityService,
            IOrganizationRepository organizationRepository)
        {
            SystemUserRepository = systemUserRepository;
            AuthorityService = authorityService;

            OrganizationRepository = organizationRepository;
        }

        #endregion

        public Task<PageResult<SystemUser>> QuerySystemUsersAsync(SystemUserPageQuery userPageQuery)
        {
            return SystemUserRepository.AdvQuerySystemUsersAsync(userPageQuery);
        }

        public Task<IEnumerable<SystemUser>> QueryRoleAccountsAsync(string orgId, string roleId)
        {
            return SystemUserRepository.FindAsync(p => p.OrgId == orgId && p.RoleId == roleId);
        }

        /// <summary>
        /// 修复昵称拼音字母
        /// </summary>
        /// <returns></returns>
        public async Task BatchFixNickPinyinAsync()
        {
            var users = await SystemUserRepository.FindAllAsync();
            var tasks = users.Select(user =>
                SystemUserRepository.UpdateAsync(user, p => p.NickPy, PingYinHelper.GetFirstSpell(user.Nick)));
            await Task.WhenAll(tasks);
        }

        public async Task<Result<SystemUser>> CreateSystemUserAsync(SystemUserCreateModel userCreateModel, string orgId)
        {
            var validate = userCreateModel.Validate();
            if (!validate.Success) return Result<SystemUser>.Fail(validate.Error);
            var user = await this.FindSystemUserByLoginId(userCreateModel.LoginId);
            if (user != null)
            {
                return Result<SystemUser>.Fail("账号名称已经存在");
            }

            if (userCreateModel.Mobile.IsMobile() && await SystemUserRepository.AnyAsync(p => p.Mobile == userCreateModel.Mobile))
            {
                return Result<SystemUser>.Fail("手机号已在系统中存在");
            }
            //新建用户
            user = userCreateModel.MapTo(new SystemUser());
            user.OrgId = orgId;
            user.NickPy = PingYinHelper.GetFirstSpell(user.Nick);
            await SystemUserRepository.InsertAsync(user);
            return Result<SystemUser>.Ok(user);

        }

        public async Task<Result<SystemUser>> UpdateSystemUserAsync(SystemUserUpdateModel userUpdateModel)
        {
            var validate = userUpdateModel.Validate();
            if (!validate.Success) return Result<SystemUser>.Fail(validate.Error);
            var user = await this.SystemUserRepository.GetAsync(userUpdateModel.UserId);
            if (user == null)
            {
                return Result<SystemUser>.Fail("待修改的账户不存在");
            }
            //检查是否更换了账户登录ID
            if (user.LoginId != userUpdateModel.LoginId)
            {
                if (await this.SystemUserRepository.AnyAsync(p => p.LoginId == userUpdateModel.LoginId))
                {
                    return Result<SystemUser>.Fail("账户登录ID已存在，不能修改为已有账户登录ID");
                }
            }
            //建议是否更换了手机号
            if (user.Mobile != userUpdateModel.Mobile && userUpdateModel.Mobile.IsMobile())
            {
                if (await SystemUserRepository.AnyAsync(p => p.Mobile == userUpdateModel.Mobile && p.Id != user.Id))
                {
                    return Result<SystemUser>.Fail("账户手机号已存在，不能保存重复手机号");
                }
            }
            //修改用户
            //记录旧密码
            var oldPwd = user.LoginPwd;
            user = userUpdateModel.MapTo(user);
            if (userUpdateModel.LoginPwd.IsNullOrEmpty())
            {
                user.LoginPwd = oldPwd; //密码为空时，不修改密码
            }
            user.NickPy = PingYinHelper.GetFirstSpell(user.Nick);
            #region 强制要求修改密码

            var needChangePwd = user.NeedChangePassword;
            if (needChangePwd && user.LoginPwd != userUpdateModel.LoginPwd)
            {
                user.NeedChangePassword = false;
            }
            else
            {
                user.NeedChangePassword = needChangePwd;
            }

            #endregion
            await SystemUserRepository.UpdateAsync(user);
            return Result<SystemUser>.Ok(user);

        }

        public Task<SystemUser> FindSystemUserByLoginId(string loginId)
        {
            return SystemUserRepository.FirstAsync(p => p.LoginId == loginId);
        }

        public async Task<bool> DeleteSystemUserAsync(string userid)
        {
            //删除用户时：需要删除已赋予用户的权限记录
            await AuthorityService.DeleteAuthoritysAsync(userid);
            return await SystemUserRepository.DeleteAsync(userid);
        }

        public async Task<Result<SystemUser>> LoginAsync(SystemUserLoginModel loginModel)
        {
            var user = await SystemUserRepository.FirstAsync(p =>
                p.Mobile == loginModel.LoginId || p.LoginId == loginModel.LoginId);

            if (user == null) return Result<SystemUser>.Fail("账号不存在");
            if (!user.LoginPwd.EqualsIgnoreCase(loginModel.LoginPwd))
                return Result<SystemUser>.Fail("密码错误");

            #region 判断机构是否有效

            if (user.OrgId.IsNotNullOrEmpty())
            {
                var org = await OrganizationRepository.GetAsync(user.OrgId);
                if (org == null)
                {
                    return Result<SystemUser>.Fail("当前尚未确定机构,不支持登录");
                }
            }

            #endregion
            return Result<SystemUser>.Ok(user);
        }

        public async Task<SystemUserProfile> GetUserProfileAsync(string clientId)
        {
            var user = await SystemUserRepository.GetAsync(clientId);
            if (user == null)
            {
                return null;
            }

            return await ConvertUserProfileAsync(user);
        }

        /// <summary>
        /// 更新系统用户档案
        /// </summary>
        /// <param name="clientId">系统用户ID</param>
        /// <param name="model">更新模型</param>
        /// <returns></returns>
        public async Task<bool> UpdateUserProfileAsync(string clientId, UpdateProfileModel model)
        {
            var user = await SystemUserRepository.GetAsync(clientId);
            if (user == null)
            {
                return false;
            }

            var r = model.Validate();
            if (!r.Success)
            {
                throw new Exception(r.Error);
            }

            model.MapTo(user);
            user.NickPy = PingYinHelper.GetFirstSpell(user.Nick);
            return await SystemUserRepository.UpdateAsync(user);
        }

        /// <summary>
        /// 修改系统用户当前密码
        /// </summary>
        /// <param name="clientId">系统用户ID</param>
        /// <param name="model">修改模型</param>
        /// <returns></returns>
        public async Task<bool> ChangeUserPasswordAsync(string clientId, ChangePasswordModel model)
        {
            var user = await SystemUserRepository.GetAsync(clientId);
            if (user == null)
            {
                throw new Exception("用户不存在");
            }

            if (model.OldPwd != clientId && !user.LoginPwd.EqualsIgnoreCase(model.OldPwd))
            {
                throw new Exception("旧密码不匹配");
            }

            if (model.NewPwd.Length < 6)
            {
                throw new Exception("新密码至少需6位");
            }

            var updates = new Dictionary<Expression<Func<SystemUser, object>>, object>
            {
                {p => p.LoginPwd, model.NewPwd},
                { p => p.NeedChangePassword, false}
            };

            return await SystemUserRepository.UpdateAsync(user.Id, updates);
        }

        public Task<bool> ClearOrganizationUserAsync(string orgId)
        {
            return SystemUserRepository.DeleteAsync(p => p.OrgId == orgId);
        }

        private async Task<SystemUserProfile> ConvertUserProfileAsync(SystemUser user)
        {
            var profile = user.MapTo(new SystemUserProfile());
            var roleTask = AuthorityService.GetRoleAsync(user.RoleId);
            var orgTask = OrganizationRepository.GetAsync(user.OrgId);
            await Task.WhenAll(roleTask, orgTask);

            profile.Roles = new[] { roleTask.Result };
            if (orgTask.Result != null)
            {
                profile.OrgName = orgTask.Result.Name;
                profile.OrgId = orgTask.Result.Id;
                profile.Modules = orgTask.Result.Modules;
            }

            return profile;
        }
    }
}