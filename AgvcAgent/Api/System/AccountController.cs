using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgvcAgent.Api.Kernel;
using AgvcCoreData.Users;
using AgvcEntitys.Users;
using AgvcService.Organizations;
using AgvcService.System;
using AgvcService.System.Models;
using AgvcService.Users;
using AgvcService.Users.Models;
using AgvcService.Users.Models.Messages;
using CoreData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility.Extensions;
using Utility.Helpers;

namespace AgvcAgent.Api.System
{
    /// <summary>
    /// 账户服务
    /// </summary>
    [Route("api/account")]
    public class AccountController : AuthorizedApiController
    {
        #region 依赖注入

        /// <summary>
        /// ctor.
        /// </summary>
        /// <param name="accountService"></param>
        /// <param name="smsService"></param>
        /// <param name="authorityService"></param>
        /// <param name="accountBalanceService"></param>
        /// <param name="orgnizationService"></param>
        /// <param name="systemFeatureService"></param>
        /// <param name="branchCompanyService"></param>
        /// <param name="departmentService"></param>
        public AccountController(
            IAccountService accountService,
            ISmsService smsService,
            IAuthorityService authorityService,
            IAccountBalanceService accountBalanceService,
            IOrgnizationService orgnizationService,
            ISystemFeatureService systemFeatureService,
            IBranchCompanyService branchCompanyService,
            IDepartmentService departmentService)
        {
            AccountService = accountService;
            SmsService = smsService;
            AuthorityService = authorityService;
            AccountBalanceService = accountBalanceService;
            OrgnizationService = orgnizationService;
            SystemFeatureService = systemFeatureService;
            //分公司、部门服务
            BranchCompanyService = branchCompanyService;
            DepartmentService = departmentService;
        }
        private IAccountService AccountService { get; }
        private ISmsService SmsService { get; }
        private IAuthorityService AuthorityService { get; }
        private IAccountBalanceService AccountBalanceService { get; }
        private IOrgnizationService OrgnizationService { get; }
        private ISystemFeatureService SystemFeatureService { get; }
        private IBranchCompanyService BranchCompanyService { get; }
        private IDepartmentService DepartmentService { get; }

        #endregion

        /// <summary>
        /// 所在域用户
        /// </summary>
        public class DomainAppUserInfo : AppOpenIdentify
        {
            /// <summary>
            /// 所在域名
            /// </summary>
            public string Domain { get; set; }
        }
        /// <summary>
        /// 是否已经有注册的APP用户
        /// </summary>
        /// <param name="appUser"></param>
        /// <returns></returns>
        [HttpPost, AllowAnonymous]
        [Route("is-app-user")]
        public async Task<bool> IsAppUserExistsAsync([FromForm] DomainAppUserInfo appUser)
        {
            var org = await OrgnizationService.GetOrgByDomainAsync(appUser.Domain);
            var accounts = await AccountService.FindAppAccountsAsync(appUser, org?.Id);
            return accounts.AnyNullable();
        }
        /// <summary>
        /// 查找指定APP用户
        /// </summary>
        /// <param name="appUser"></param>
        /// <returns></returns>
        [HttpPost, AllowAnonymous]
        [Route("find-app-user")]
        public async Task<IEnumerable<Account>> FindAppUsersAsync([FromForm] DomainAppUserInfo appUser)
        {
            var org = await OrgnizationService.GetOrgByDomainAsync(appUser.Domain);
            var accounts = await AccountService.FindAppAccountsAsync(appUser, org?.Id);
            return accounts;

        }
        /// <summary>
        /// 强制指定客户端下线
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("kickoff")]
        public async Task<bool> KickOffAsync([FromForm] KickOffModel model)
        {
            var user = await AccountService.FindAccountAsync(ClientId);
            if (user.Mobile.IsMobile() == false)
            {
                throw new Exception("账户未绑定手机号，无法验证码短信验证码");
            }
            //短信验证码验证失败
            if (!await SmsService.ValidateAuthenticationAsync(user.Mobile, model.SmsCode))
            {
                throw new Exception("短信验证码验证失败");
            }

            return  false;
            //  return await SignalrService.KickOffConnectionAsync(model.ConnectionId, "您的账户已在别处登录,此登陆被服务器强制下线，如果不是您的操作，请及时修改密码！");

        }
       

        /// <summary>
        /// Agent模型
        /// </summary>
        public class QAgentModel
        {
            /// <summary>
            /// 机构ID
            /// </summary>
            public string OrgId { get; set; }
        }

        [HttpPost]
        [Route("regsiter"), AllowAnonymous]
        public async Task<bool> Regsiter([FromForm] RegisterModel registerModel)
        {
            var r = await AccountService.RegsiterAsync(registerModel);
            if (r.Success)
            {
                return r.Data;
            }
            return false;

        }
        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("fetchSms"), AllowAnonymous]
        public async Task<bool> FetchSmsCode(string mobile, string key)
        {
            var dto = new SmsCodeDto
            {
                Mobile = mobile,
                Key = key,
                Code = RandomizeHelper.GenerateNumbers(4)
            };
            var smsCodeSendOk = await SmsService.SendAuthenticationAsync(dto);
            if (!smsCodeSendOk)
            {
                throw new Exception("短信验证码发送失败");
            }

            return true;
        }

        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("fetchSmsSecurity"), AllowAnonymous]
        public async Task<bool> FetchSmsCodeSecurityAsync(string mobile, string key, string securityKey)
        {

            if (!securityKey.IsValidSecurityKey())
            {
                throw new Exception("安全验证失败");
            }

            var dto = new SmsCodeDto
            {
                Mobile = mobile,
                Key = key,
                Code = RandomizeHelper.GenerateNumbers(4)
            };
            var smsCodeSendOk = await SmsService.SendAuthenticationAsync(dto);
            if (!smsCodeSendOk)
            {
                throw new Exception("短信验证码发送失败");
            }

            return true;
        }
        /// <summary>
        /// 获取指定用户账户资料
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<AccountUserProfile> FindAccountProfileAsync(string id)
        {
            var account = await AccountService.FindAccountAsync(id);
            var orgTask = OrgnizationService.GetOrgAsync(account.OrgId);
            var balanceTask = AccountBalanceService.GetBalanceTotalAsync(id, BalanceType.Bean);
            var roleTask = AuthorityService.GetRoleAsync(account.RoleId);
            var featuresTask = SystemFeatureService.GetOrganizationFeaturesNoSafetyAsync(account.OrgId);

            await Task.WhenAll(orgTask, balanceTask, roleTask, featuresTask);
            var profile = account.MapTo<Account, AccountUserProfile>();
            profile.BalanceTotal = balanceTask.Result;
            profile.BoxOwnerIds = account.BoxOwnerIds;
            profile.AppUserIdentifys = account.AppUserIdentifys;
            profile.AppUserInfo = account.AppUserInfo;
            //role
            if (roleTask.Result != null)
            {
                profile.Roles = new[] { roleTask.Result };
            }
            //org
            if (orgTask.Result != null)
            {
                profile.OrgId = orgTask.Result.Id;
                profile.OrgName = orgTask.Result.Name;
                profile.Modules = orgTask.Result.Modules;
                profile.Features = featuresTask.Result;
            }
            //分公司
            if (account.BranchCompanyId.IsNotNullOrEmpty())
            {
                profile.BranchCompany = (await BranchCompanyService.GetAsync(account.BranchCompanyId)).Name;
            }
            //部门
            if (account.DepartmentId.IsNotNullOrEmpty())
            {
                profile.Department = (await DepartmentService.GetAsync(account.DepartmentId)).Name;
            }

            return profile;

        }
        /// <summary>
        /// 获取当前登录账户资料
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("profile")]
        public async Task<AccountUserProfile> GetProfileAsync()
        {
            var id = ClientId;
            return await this.FindAccountProfileAsync(id);
        }

        /// <summary>
        /// 绑定手机号
        /// </summary>
        /// <param name="model">手机验证模型</param>
        /// <returns></returns>
        [HttpPost]
        [Route("bind-mobile")]
        public Task<Result<bool>> BindAccountMobileAsync([FromForm] MobileValidateModel model)
        {
            return AccountService.BindMobileAsync(ClientId, model.Mobile,
                model.SmsCode);
        }
        /// <summary>
        /// 解绑手机号
        /// </summary>
        /// <param name="smsCode">手机验证码</param>
        /// <returns></returns>
        [HttpPost]
        [Route("unbind-mobile")]
        public Task<Result<bool>> UnBindAccountMobileAsync([FromForm] SmsCodeType smsCode)
        {
            return AccountService.UnBindMobileAsync(ClientId, smsCode);
        }



        /// <summary>
        /// 更新用户资料档案
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("update-profile")]
        public Task<bool> UpdateUserProfileAsync([FromForm] UpdateProfileModel model)
        {

            return AccountService.UpdateUserProfileAsync(ClientId, model);
        }

        /// <summary>
        /// 更新已征得车主授权
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("accept-agreement")]
        public Task<bool> UpdateAgreementAsync()
        {
            return AccountService.UpdateAgreementAsync(ClientId, true);
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("change-password")]
        public Task<bool> ChangeUserPasswordAsync([FromForm] ChangePasswordModel model)
        {

            return AccountService.ChangeUserPasswordAsync(ClientId, model);
        }

        /// <summary>
        /// 查找密码
        /// </summary>
        [HttpPost, AllowAnonymous]
        [Route("find-password")]
        public Task<Result<IdentSecurity>> FindUserPasswordAsync([FromForm] MobileValidateModel model)
        {
            return AccountService.FindUserPasswordAsync(model);
        }

        /// <summary>
        /// 重设密码
        /// </summary>
        [HttpPost, AllowAnonymous]
        [Route("reset-password")]
        public Task<Result<bool>> ReSetPasswordAsync([FromForm] ReSetPasswordModel model)
        {
            return AccountService.ReSetPasswordAsync(model);
        }
        #region 积分，油滴

        /// <summary>
        /// 获取当前账户油滴数
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("balance")]
        public async Task<double> GetBalanceAsync()
        {
            return await AccountBalanceService.GetBalanceTotalAsync(ClientId, BalanceType.Bean);

        }
        /// <summary>
        /// 查询油滴流水日志记录
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("balance_query")]
        public async Task<PageResult<AccountBalance>> QueryBalanceLogs([FromForm] BalanceLogQuery query)
        {
            return await AccountBalanceService.QueryBalanceLogsAsync(ClientId, BalanceType.Bean, query);

        }

        #endregion




        /// <summary>
        /// 提交当前登录设备的Appid 和 OpenId
        /// </summary>
        /// <param name="identify"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("update_identify")]
        public async Task<bool> UpdateAppOpenIdentify([FromForm] AppOpenIdentify identify)
        {
            return await AccountService.UpdateAppOpenIdentifyAsync(ClientId, identify);
        }

        #region 机构用户维护

        [HttpPost]
        [Route("query")]
        public async Task<PageResult<UserDto>> QueryAccountUsersAsync([FromForm] AccountUserPageQuery userPageQuery)
        {
            var pageResult = await AccountService.QueryAccountUsersAsync(userPageQuery, OrgId);
            var users = pageResult.Datas.ToListEx();
            var datas = new List<UserDto>();
            var branchCompanys =
                (await BranchCompanyService.PageQueryAsync(new PageQuery(), p => p.OrgId == OrgId, o => o.CreatedOn))
                .Datas;
            var departments = (await DepartmentService.PageQueryAsync(new PageQuery(), p => p.OrgId == OrgId, o => o.CreatedOn))
                .Datas;
            if (users.Any())
            {
                datas = users.MapTo<Account, UserDto>();
                var allRoles = (await AuthorityService.QueryAllRolesAsync()).ToListEx();

                datas.ForEach(d =>
                {

                    var role = allRoles.SingleOrDefault(p => p.Id == d.RoleId);
                    if (role == null) return;
                    d.RoleName = role.Name;
                    d.RoleLevel = role.Level;
                    d.BranchCompany = branchCompanys.FirstOrDefault(p => p.Id == d.BranchCompanyId)?.Name;
                    d.Department = departments.FirstOrDefault(p => p.Id == d.DepartmentId)?.Name;
                });

            }
            return new PageResult<UserDto>
            {
                Datas = datas,
                PageCount = pageResult.PageCount,
                Total = pageResult.Total
            };

        }
        /// <summary>
        /// 创建机构用户
        /// </summary>
        /// <param name="userCreateModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("create")]
        public Task<Result<Account>> CreateAccountUserAsync([FromForm] CreateAccountModel userCreateModel)
        {
            return AccountService.CreateAccountAsync(userCreateModel, OrgId);
        }
        /// <summary>
        /// 更新机构用户
        /// </summary>
        /// <param name="userUpdateModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("update")]
        public async Task<UserUpdateResult<Account>> UpdateAccountUserAsync([FromForm] AccountUserUpdateModel userUpdateModel)
        {
            var result = await AccountService.UpdateAccountUserAsync(userUpdateModel);
            return result;
        }
        /// <summary>
        /// 删除机构用户
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("delete/{userid}")]
        public async Task<bool> DeleteAccountUserAsync(string userid)
        {
            return await AccountService.DeleteAccountUserAsync(userid);
        }

        /// <summary>
        /// 机构用户自动完成搜索
        /// </summary>
        /// <param name="kw">关键字,注意：kw支持传递以逗号分隔的ID列表</param>
        /// <returns></returns>
        [HttpGet]
        [Route("identity-users")]
        public async Task<IEnumerable<Identity>> IdentitySuggestsAsync(string kw)
        {
            return await AccountService.IdentitySuggestsAsync(OrgId, kw);
        }

        public class CreateTempAuthModel
        {
            /// <summary>
            /// 是否是车船查类型
            /// </summary>
             public  bool  IsTruckMap { get; set; }
            /// <summary>
            /// 用户ID
            /// </summary>
             public string AccountId { get; set; }
        }

        #endregion
    }


}