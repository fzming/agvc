using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AgvcCoreData.System;
using AgvcEntitys.Organization;
using AgvcRepository.Orgnizations.Interfaces;
using AgvcService.Organizations.Models;
using AgvcService.System;
using AgvcService.System.Models;
using AgvcService.Users;
using AgvcService.Users.Models;
using CoreService;
using Utility.Extensions;

namespace AgvcService.Organizations
{
    public class OrgnizationService : AbstractService, IOrgnizationService
    {
        /// <summary>
        ///     机构缩写黑名单
        /// </summary>
        private readonly string[] DomainBlacklists = {"www", "lanyun", "admin", "yy5156", "com", "net"};

        /// <inheritdoc />
        public Task<IEnumerable<Organization>> QueryOrgsAsync(string paOrgId)
        {
            return OrganizationRepository.FindAsync(p => p.ParentOrgId == paOrgId);
        }

        /// <inheritdoc />
        public async Task<Organization> CreateOrgAsync(CreateOrgModel model, string paOrgId)
        {
            //创建机构
            model.Prefix = model.Prefix.ToUpper();
            var organization = model.MapTo(new Organization());
            organization.ParentOrgId = paOrgId;


            if (model.PrimaryType != OrganizationType.System)
            {
                if (paOrgId.IsNullOrEmpty()) throw new Exception("非系统型机构需传入父机构ID");

                if (model.Prefix.IsNullOrEmpty()) throw new Exception("非系统型机构需传入机构简称");
                if (model.Prefix.Length > 4) throw new Exception("机构简称不能超过4位");

                if (DomainBlacklists.Contains(model.Prefix, new IgnoreCaseComparer()))
                    throw new Exception("不能使用一个已禁用的机构简称");

                if (!Regex.IsMatch(model.Prefix, @"^[aA-zZ]*$", RegexOptions.Singleline))
                    throw new Exception("机构简称必须全为英文字母构成");
                if (await OrganizationRepository.IsFieldRepeatAsync(p => p.Prefix, model.Prefix))
                    throw new Exception($"机构简称“{model.Prefix}”已被其他机构占用，请重新选择机构简称。");
                if (model.Modules.AnyNullable() == false) throw new Exception("非系统型机构至少需选择一个功能模块");
                //创建货代用户
                var rs = await AccountService.CreateAccountAsync(new CreateAccountModel
                {
                    Nick = model.Nick,
                    LoginId = model.LoginId,
                    LoginPwd = model.LoginPwd,
                    RoleId = model.RoleId,
                    NeedChangePassword = true
                }, organization.Id);
                if (!rs.Success) throw new Exception(rs.Error);
            }
            else //系统机构
            {
                //创建管理用户
                var rs = await SystemUserService.CreateSystemUserAsync(new SystemUserCreateModel
                {
                    LoginId = model.LoginId,
                    LoginPwd = model.LoginPwd,
                    //用户所属角色
                    Nick = model.Nick,
                    RoleId = model.RoleId,
                    NeedChangePassword = true
                }, organization.Id);
                if (!rs.Success) throw new Exception(rs.Error);
            }

            await OrganizationRepository.InsertAsync(organization);
            return organization;
        }

        /// <inheritdoc />
        public async Task<bool> UpdateOrgAsync(UpdateOrgModel model)
        {
            var org = await OrganizationRepository.GetAsync(model.OrgId);
            org = model.MapTo(org);
            org.Prefix = org.Prefix.ToUpper();
            if (org.PrimaryType != OrganizationType.System)
            {
                if (org.Prefix.IsNullOrEmpty()) throw new Exception("非系统型机构需传入机构简称");

                if (org.Prefix.Length > 4) throw new Exception("机构简称不能超过4位");
                if (DomainBlacklists.Contains(model.Prefix, new IgnoreCaseComparer()))
                    throw new Exception("不能使用一个已禁用的机构简称");
                if (!Regex.IsMatch(org.Prefix, @"^[aA-zZ]*$", RegexOptions.Singleline))
                    throw new Exception("机构简称必须全为英文字母构成");
                if (await OrganizationRepository.IsFieldRepeatAsync(p => p.Prefix, org.Prefix, "", org.Id))
                    throw new Exception($"机构简称“{model.Prefix}”已被其他机构占用，请重新选择机构简称。");
                if (org.Modules.AnyNullable() == false) throw new Exception("非系统型机构至少需选择一个功能模块");
            }

            return await OrganizationRepository.UpdateAsync(org);
        }

        /// <inheritdoc />
        public async Task<bool> DeleteOrgAsync(string orgId)
        {
            var taskDeleteAcc = AccountService.ClearOrganizationUserAsync(orgId);
            var taskDeleteSys = SystemUserService.ClearOrganizationUserAsync(orgId);
            var taskAuths = AuthorityService.ClearOrganizationAuthAsync(orgId);
            await Task.WhenAll(taskDeleteAcc, taskDeleteSys, taskAuths);
            //删除机构
            return await OrganizationRepository.DeleteAsync(orgId);
        }

        /// <summary>
        ///     获取机构
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public Task<Organization> GetOrgAsync(string orgId)
        {
            return OrganizationRepository.GetAsync(orgId);
        }

        /// <summary>
        ///     获取所有机构ID列表
        /// </summary>
        /// <returns></returns>
        public Task<List<string>> GroupAllIdsAsync()
        {
            return OrganizationRepository.GroupAllIdsAsync();
        }

        /// <summary>
        ///     更新模块
        /// </summary>
        /// <param name="modules"></param>
        /// <returns></returns>
        public Task UpdateModulesAsync(ModuleType[] modules)
        {
            return OrganizationRepository.UpdateModulesAsync(modules);
        }

        public async Task<Organization> GetOrgByDomainAsync(string domain)
        {
            if (domain.IsNullOrEmpty() || domain.EqualsIgnoreCase("lanyun") || domain == "localhost") return null;
            // if (domain == "localhost") domain = "jit";//本地开发域名默认使用，jit
            domain = domain.ToUpper();
            return await OrganizationRepository.FirstAsync(p => p.Prefix == domain);
        }

        #region 依赖注入

        private IAccountService AccountService { get; }
        private ISystemUserService SystemUserService { get; }
        private IOrganizationRepository OrganizationRepository { get; }
        private IAuthorityService AuthorityService { get; }

        public OrgnizationService(IAccountService accountService, ISystemUserService systemUserService,
            IOrganizationRepository organizationRepository, IAuthorityService authorityService)
        {
            AccountService = accountService;
            SystemUserService = systemUserService;
            OrganizationRepository = organizationRepository;
            AuthorityService = authorityService;
        }

        #endregion
    }
}