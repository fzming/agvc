using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AgvcCoreData.Users;
using AgvcEntitys.Users;
using AgvcRepository.Orgnizations.Interfaces;
using AgvcRepository.Users.Interfaces;
using AgvcService.System.Models;
using AgvcService.Users.Models;
using CoreData;
using CoreData.Core;
using CoreService;
using Utility.Extensions;

namespace AgvcService.Users
{

    [Export(typeof(IAccountService))]
    internal class AccountService : AbstractService, IAccountService
    {
        #region IOC

        [ImportingConstructor]
        public AccountService(IAccountRepository accountRepository,
            ISmsService smsService,
            ICompanyService companyService,
            IOrganizationRepository organizationRepository,
            IRedisStringCache redisStringCache,
            IUpYunUploader upYunUploader,
            IRedisKeyCache redisKeyCache)
        {
            AccountRepository = accountRepository;
            SmsService = smsService;
            CompanyService = companyService;
            OrganizationRepository = organizationRepository;
            RedisStringCache = redisStringCache;
            UpYunUploader = upYunUploader;
            RedisKeyCache = redisKeyCache;
        }
        private IAccountRepository AccountRepository { get; }
        private ISmsService SmsService { get; }
        private ICompanyService CompanyService { get; }
        private IOrganizationRepository OrganizationRepository { get; }
        private IRedisStringCache RedisStringCache { get; }
        private IUpYunUploader UpYunUploader { get; }
        private IRedisKeyCache RedisKeyCache { get; }
        #endregion

        public async Task<Result<bool>> RegsiterAsync(RegisterModel registerModel)
        {

            if (!registerModel.Mobile.IsMobile())
            {
                return Result<bool>.Fail("手机号格式不正确");
            }


            var isMobileHasRegistered = await AccountRepository.AnyAsync(p =>
                p.Mobile == registerModel.Mobile || p.LoginId == registerModel.Mobile);
            if (isMobileHasRegistered)
            {
                return Result<bool>.Fail("手机号已经存在存在，请更换手机号");
            }

            //短信服务验证
            if (registerModel.SmsCode == null)
            {
                return Result<bool>.Fail("请输入短信验证码");
            }

            //短信验证码验证失败
            if (!await SmsService.ValidateAuthenticationAsync(registerModel.Mobile, registerModel.SmsCode))
            {
                return Result<bool>.Fail("短信验证码验证失败");
            }

            var account = new Account
            {
                LoginId = registerModel.Mobile,
                LoginPwd = registerModel.Password,
                Device = registerModel.Device,

                //用户信息
                Mobile = registerModel.Mobile,
                Nick = $"手机用户{registerModel.Mobile.Right(4)}",
                Gender = "男"
                // Avatar = "",
                // Email = 

            };
            await AccountRepository.InsertAsync(account);


            return Result<bool>.Successed;
        }

        public async Task<Account> FindAccountAsync(string id)
        {
            var account = await AccountRepository.GetAsync(id);
            return account;
        }

        public async Task<Result<bool>> UpdateAccountInfo(AccountUserProfile accountUserProfile)
        {
            var userId = accountUserProfile.Id;
            var account = await AccountRepository.GetAsync(userId);
            if (account != null)
            {
                account = accountUserProfile.MapTo(account);//dto->entity
                account.NickPy = PingYinHelper.GetFirstSpell(account.Nick);
                return Result<bool>.From(await AccountRepository.ReplaceAsync(accountUserProfile.Id, account));//更新单个文档
            }

            return Result<bool>.Fail("账户不存在");
        }

        public async Task<string> GetDomainOrgIdAsync(string domain)
        {
            domain = domain.ToStringEx();
            if (domain.IsNullOrEmpty() || domain.EqualsIgnoreCase("lanyun") || domain.EqualsIgnoreCase("localhost")) return string.Empty;
            domain = domain.ToUpper();
            try
            {
                return (await OrganizationRepository.FirstAsync(p => p.Prefix == domain))?.Id;
            }
            catch (Exception)
            {
                return string.Empty;
            }

        }
        public async Task<Result<Account>> LoginAccountWithPasswordAsync(string userName, string password,
            AppOpenIdentify identify, AppUserInfo appUserInfo, string loginDomain)
        {
            var accounts = await AccountRepository.FindAsync(p => p.Mobile == userName || p.LoginId == userName);

            if (accounts.AnyNullable() == false)
            {
                return Result<Account>.Fail("登陆失败：账号不存在");
            }

            var passAccounts = accounts.Where(p => p.LoginPwd.Equals(password));
            if (passAccounts.AnyNullable() == false)
            {

                return Result<Account>.Fail("登陆失败：账户密码错误");
            }

            var domainOrgId = await GetDomainOrgIdAsync(loginDomain);
            Account account = null;
            if (domainOrgId.IsNotNullOrEmpty()) //已明确登录域
            {
                account = passAccounts.FirstOrDefault(p => p.OrgId == domainOrgId);
            }
            else if (passAccounts.Count() == 1) //只有一个用户且没有登录域，兼容车船查情况，本地登录
            {
                account = passAccounts.FirstOrDefault();
            }

            if (account == null)
            {
                return Result<Account>.Fail("登陆失败：账户不存在或机构不存在");
            }

            if (identify != null)
            {
                var appUserIdentifys = account.AppUserIdentifys ?? new List<AppOpenIdentify>();
                if (appUserIdentifys.Any(p => p.AppId == identify.AppId))
                {
                    return Result<Account>.Fail($"当前输入用户({account.LoginId})已绑定了微信({account.AppUserInfo?.nickName})，不允许重复绑定");
                }
                await TryBindAppOpenIdentifyAsync(account, identify, appUserInfo);
            }
            else
            {

            }
            return Result<Account>.Ok(account);
        }

        /// <summary>
        /// 检查更新账户AppIdentify(仅移动端支持)
        /// </summary>
        /// <param name="account">账户</param>
        /// <param name="identify">移动设备识别对象</param>
        /// <param name="appUserInfo"></param>
        /// <returns></returns>
        private async Task TryBindAppOpenIdentifyAsync(Account account, AppOpenIdentify identify,
            AppUserInfo appUserInfo = null)
        {
            if (identify == null || account == null)
            {
                return;
            }

            //AppId和OpenId必须传递
            if (identify.AppId.IsNullOrEmpty() || identify.OpenId.IsNullOrEmpty())
            {
                return;
            }
            var accountIdentifiers = account.AppUserIdentifys ?? new List<AppOpenIdentify>();

            if (accountIdentifiers.Any(p => p.AppId == identify.AppId))
            {
                //一个账户只允许一个Appid对应的openid,不能对应多个openid 
                accountIdentifiers.RemoveAll(p => p.AppId == identify.AppId);
            }
            accountIdentifiers.Add(identify);
            account.AppUserIdentifys = accountIdentifiers;
            if (appUserInfo != null) account.AppUserInfo = appUserInfo;

            await AccountRepository.UpdateAsync(account);

        }

        public Task<IEnumerable<Account>> FindAppAccountsAsync(AppOpenIdentify openIdentify, string orgId)
        {
            if (orgId.IsNullOrEmpty())
            {
                return AccountRepository.FindAsync(p => p.AppUserIdentifys.Any(k =>
                    k.AppId == openIdentify.AppId &&
                    (k.OpenId == openIdentify.OpenId ||
                     k.UnionId == openIdentify.UnionId)));
            }

            return AccountRepository.FindAsync(p => p.AppUserIdentifys.Any(k =>
                k.AppId == openIdentify.AppId &&
                (k.OpenId == openIdentify.OpenId ||
                 k.UnionId == openIdentify.UnionId)) && p.OrgId == orgId);
        }

        public async Task<Account> LoginAppAccountAsync(AppOpenIdentify openIdentify, string domain)
        {
            var orgId = await GetDomainOrgIdAsync(domain);
            if (orgId.IsNullOrEmpty())
            {
                return await AccountRepository.FirstAsync(p => p.AppUserIdentifys.Any(k =>
                                                                   k.AppId == openIdentify.AppId &&
                                                                   (k.OpenId == openIdentify.OpenId ||
                                                                    k.UnionId == openIdentify.UnionId)));
            }
            return await AccountRepository.FirstAsync(p => p.AppUserIdentifys.Any(k =>
                                                               k.AppId == openIdentify.AppId &&
                                                               (k.OpenId == openIdentify.OpenId ||
                                                                k.UnionId == openIdentify.UnionId)) &&
                                                           p.OrgId == orgId);
        }

        /// <summary>
        /// 短信验证码登录
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="smsCode"></param>
        /// <param name="smsKey"></param>
        /// <param name="identify"></param>
        /// <param name="appUserInfo"></param>
        /// <param name="loginDomain"></param>
        /// <returns></returns>
        public async Task<Result<Account>> SmsLoginAsync(string mobile, string smsCode, string smsKey,
            AppOpenIdentify identify, AppUserInfo appUserInfo, string loginDomain)
        {
            if (!mobile.IsMobile())
            {
                return Result<Account>.Fail("手机号格式不正确");
            }
            //短信服务验证
            if (smsCode.IsNullOrEmpty() || smsKey.IsNullOrEmpty())
            {
                return Result<Account>.Fail("请输入短信验证码");
            }

            //短信验证码验证失败
            if (!await SmsService.ValidateAuthenticationAsync(mobile,
                new SmsCodeType(smsKey, smsCode)))
            {
                return Result<Account>.Fail("短信验证码验证失败");
            }
            var accounts = await AccountRepository.FindAsync(p => p.Mobile == mobile);
            if (accounts.AnyNullable() == false)
            {
                return Result<Account>.Fail("账户不存在");
            }
            var domainId = await GetDomainOrgIdAsync(loginDomain);
            Account account;
            // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
            if (domainId.IsNullOrEmpty())
            {
                account = accounts.OrderBy(p => p.CreatedOn).FirstOrDefault(); //多个手机号账户，只取第一个账户，车船查账户手机号不允许重复
            }
            else
            {
                account = accounts.FirstOrDefault(p => p.OrgId == domainId); //使用domain登录情况下，需要明确具体的domain
            }

            if (account == null)
            {
                return Result<Account>.Fail("账户不存在");
            }
            //使用短信登陆成功

            await TryBindAppOpenIdentifyAsync(account, identify, appUserInfo);
            return Result<Account>.Ok(account);


        }

        /// <summary>
        /// 短信验证码登陆并创建账户
        /// </summary>
        /// <param name="mobile">手机号</param>
        /// <param name="smsCode">短信验证码</param>
        /// <param name="smsKey">短信验证类型KEY</param>
        /// <param name="identify">设备识别</param>
        /// <param name="appUserInfo">用户授权信息</param>
        /// <param name="referrer">推荐人信息</param>
        /// <param name="loginDomain"></param>
        /// <returns></returns>
        public async Task<Result<Account>> SmsLoginAndCreateAccountAsync(string mobile, string smsCode, string smsKey,
            AppOpenIdentify identify, AppUserInfo appUserInfo, ActReferrer referrer, string loginDomain)
        {
            var smsLoginResult = await SmsLoginAsync(mobile, smsCode, smsKey, identify, appUserInfo, loginDomain);
            if (smsLoginResult.Success)
            {
                return smsLoginResult;
            }

            if (smsLoginResult.Error != "账户不存在")
            {
                throw new Exception(smsLoginResult.Error);
            }
            //账户不存在情况下：注册非货代用户账号
            try
            {
                var loginId = mobile;
                if (await IsLoginIdExsitsAsync(loginId)) //检查手机号是否已经注册了LoginId
                {
                    loginId = Guid.NewGuid().ToString("N");
                }

                if (await IsMobileExsitsAsync(mobile))
                {
                    throw new Exception("登录失败：该账户手机号存在多个账户中，无法使用短信登陆，请使用密码进行登录");
                }
                var account = new Account
                {
                    LoginId = loginId,
                    LoginPwd = "",
                    Device = identify == null ? "PC" : identify.AppId,

                    //用户信息
                    Mobile = mobile,
                    Nick = $"手机用户{mobile.Right(4)}",
                    Gender = "男",

                    Referrer = referrer //推荐人
                                        // Avatar = "",
                                        // Email = 

                };

                await AccountRepository.InsertAsync(account);
                await TryBindAppOpenIdentifyAsync(account, identify, appUserInfo);

                return Result<Account>.Ok(account);
            }
            catch (Exception e)
            {
                return Result<Account>.Fail(e.Message);
            }


        }

        private Task<bool> IsLoginIdExsitsAsync(string loginId)
        {
            return AccountRepository.AnyAsync(p => p.LoginId == loginId);
        }
        private Task<bool> IsMobileExsitsAsync(string mobile)
        {
            return AccountRepository.AnyAsync(p => p.Mobile == mobile);
        }

        public async Task<Result<Account>> LoginAccountWithAuthKeyAsync(string clientId, string authkey)
        {
            //查找redis

            var exsit = await RedisKeyCache.KeyExistsAsync(authkey);
            if (!exsit)
            {
                return Result<Account>.Fail("授权码已过期");
            }
            var account = await AccountRepository.GetAsync(clientId);
            if (account == null)
            {
                return Result<Account>.Fail("账户不存在");
            }
            //登录成功
            return Result<Account>.Ok(account);

        }


        /// <summary>
        /// 修复昵称拼音字母
        /// </summary>
        /// <returns></returns>
        public async Task BatchFixNickPinyinAsync()
        {
            var users = await AccountRepository.FindAllAsync();
            var tasks = users.Select(user =>
                AccountRepository.UpdateAsync(user, p => p.NickPy, PingYinHelper.GetFirstSpell(user.Nick)));
            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Creates the auth key async.
        /// </summary>
        /// <returns>The auth key async.</returns>
        /// <param name="clientId">Client identifier.</param>
        /// <param name="expires">Expires.</param>
        public async Task<string> CreateAuthKeyAsync(string clientId, TimeSpan expires)
        {
            var authkey = $"AuthKey-{Guid.NewGuid():N}{clientId}".Md516();
            await RedisStringCache.StringSetAsync(authkey, clientId, expires);
            return authkey;
        }

        /// <summary>
        /// 个人用户升级企业用户
        /// </summary>
        /// <param name="account"></param>
        /// <param name="accountUpgrade"></param>
        /// <returns></returns>
        public  Task<bool> UpgradeEnterpriseAccountAsync(Account account, RequestAccountUpgrade accountUpgrade)
        {
            // var enterprise = await EnterpriseService.AddEnterpriseAsync(accountUpgrade.EnterpriseName, accountUpgrade.USCC);
            // //创建企业
            // var company = await CompanyService.GetCreateCompany(enterprise, new Contacter
            // {
            //     User = account.Nick,
            //     Mobile = account.Mobile
            // });
            // //营业执照
            // var result = accountUpgrade.Certificate.Save();
            // var certificate = await UpYunUploader.UploadFileAsync(result.PhysicalPath);
            // return await CompanyService.SetCertificateAsync(company.Id, certificate);
            return Task.FromResult(true);

        }

        public Task<IEnumerable<Account>> QueryAllAsync()
        {
            return AccountRepository.FindAllAsync();
        }

        public Task<IEnumerable<Account>> QueryReferrersAsync(string clientId, string actId)
        {
            return AccountRepository.FindAsync(p => p.Referrer.UserId == clientId && p.Referrer.ActId == actId);
        }

        public async Task<bool> UpdateAppOpenIdentifyAsync(string clientId, AppOpenIdentify identify)
        {
            var account = await this.FindAccountAsync(clientId);

            //AppId和OpenId必须传递
            if (identify.AppId.IsNullOrEmpty() || identify.OpenId.IsNullOrEmpty())
            {
                return false;
            }
            var accountIdentifiers = account.AppUserIdentifys ?? new List<AppOpenIdentify>();

            if (accountIdentifiers.Any(p => p.AppId == identify.AppId))
            {
                //一个账户只允许一个Appid对应的openid,不能对应多个openid 
                accountIdentifiers.RemoveAll(p => p.AppId == identify.AppId);
            }
            accountIdentifiers.Add(identify);

            return await AccountRepository.UpdateAsync(account, p => p.AppUserIdentifys, accountIdentifiers);
        }
        /// <summary>
        /// 创建机构账户
        /// </summary>
        /// <param name="createAccountModel"></param>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public async Task<Result<Account>> CreateAccountAsync(CreateAccountModel createAccountModel, string orgId)
        {

            var validate = createAccountModel.Validate();
            if (!validate.Success)
            {
                return Result<Account>.Fail(validate.Error);
            }

            if (orgId.IsNullOrEmpty())
            {
                return Result<Account>.Fail("机构ID不能为空");
            }

            Account account;
            if (createAccountModel.ForbiddenLogin)
            {
                createAccountModel.LoginId = Guid.NewGuid().ToString("N");
                createAccountModel.LoginPwd = Guid.NewGuid().ToString("N");

            }
            else
            {
                #region 将普通用户升级为机构用户

                account = await this.AccountRepository.FirstAsync(p => p.LoginId == createAccountModel.LoginId);
                if (account != null)
                {
                    //将非货代用户升级为货代用户
                    if (account.OrgId.IsNullOrEmpty() || account.RoleId.IsNullOrEmpty())
                    {
                        if (account.LoginPwd.IsNullOrEmpty()) account.LoginPwd = createAccountModel.LoginPwd;
                        account.RoleId = createAccountModel.RoleId;
                        account.OrgId = orgId;
                        if (createAccountModel.Nick.IsNotNullOrEmpty())
                        {
                            account.Nick = createAccountModel.Nick;
                            account.NickPy = PingYinHelper.GetFirstSpell(account.Nick);
                        }
                        account.Gender = "男";
                        await AccountRepository.UpdateAsync(account);
                        return Result<Account>.Ok(account);
                    }
                }
                #endregion

            }
            //检查登录ID是否重复
            if (await AccountRepository.AnyAsync(p => p.LoginId == createAccountModel.LoginId && p.OrgId == orgId))
            {
                return Result<Account>.Fail("同一机构下账号有重复");
            }
            //检查姓名是否重复
            if (await AccountRepository.AnyAsync(p => p.Nick == createAccountModel.Nick && p.OrgId == orgId))
            {
                return Result<Account>.Fail("同一机构下姓名不能重复");
            }


            //创建新的机构用户
            account = createAccountModel.MapTo(new Account());
            //用户信息
            account.LoginPwd = createAccountModel.LoginPwd;
            account.Nick = createAccountModel.Nick;
            account.NickPy = PingYinHelper.GetFirstSpell(account.Nick);
            account.Gender = "男";
            account.Avatar = createAccountModel.Avatar;
            //机构角色
            account.RoleId = createAccountModel.RoleId;
            account.OrgId = orgId;

            await AccountRepository.InsertAsync(account);
            return Result<Account>.Ok(account);
        }

        /// <summary>
        /// 查询机构用户
        /// </summary>
        /// <param name="userPageQuery"></param>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public Task<PageResult<Account>> QueryAccountUsersAsync(AccountUserPageQuery userPageQuery, string orgId)
        {
            return AccountRepository.AdvQueryAccountUsersAsync(userPageQuery, orgId);
        }

        public Task<IEnumerable<Account>> QueryAccountUsersAsync(string orgId)
        {
            if (!orgId.IsObjectId())
            {
                return AccountRepository.FindAllAsync();
            }
            return AccountRepository.FindAsync(p => p.OrgId == orgId && !p.Ghost);
        }

        public Task<IEnumerable<Account>> QueryRoleAccountsAsync(string orgId, string roleId)
        {
            return AccountRepository.FindAsync(p => p.OrgId == orgId && p.RoleId == roleId && !p.Ghost);
        }

        /// <summary>
        /// 更新机构用户
        /// </summary>
        /// <param name="userUpdateModel"></param>
        /// <returns></returns>
        public async Task<UserUpdateResult<Account>> UpdateAccountUserAsync(AccountUserUpdateModel userUpdateModel)
        {
            var validate = userUpdateModel.Validate();
            if (!validate.Success) return Result<Account>.Fail(validate.Error) as UserUpdateResult<Account>;
            var user = await this.AccountRepository.GetAsync(userUpdateModel.Id);
            if (user == null)
            {
                return Result<Account>.Fail("待修改的账户不存在") as UserUpdateResult<Account>;
            }

            var roleChanged = user.RoleId != userUpdateModel.RoleId; //角色已改变

            //检查是否更换了用户名
            if (user.LoginId != userUpdateModel.LoginId)
            {
                if (await this.AccountRepository.AnyAsync(p => p.LoginId == userUpdateModel.LoginId && p.OrgId == user.OrgId))
                {
                    return Result<Account>.Fail("(同一机构下)账户名称已存在，不能修改为已有账户名称") as UserUpdateResult<Account>;
                }
            }

            //姓名重复判断
            if (user.Nick != userUpdateModel.Nick)
            {
                if (await this.AccountRepository.AnyAsync(p =>
                    p.Nick == userUpdateModel.Nick && p.OrgId == user.OrgId && p.Id != user.Id))
                {
                    return Result<Account>.Fail("同一个机构姓名不能重复") as UserUpdateResult<Account>;
                }
            }
            //记录旧密码
            var oldPwd = user.LoginPwd;
            //修改用户
            user = userUpdateModel.MapTo(user);
            if (userUpdateModel.LoginPwd.IsNullOrEmpty())
            {
                user.LoginPwd = oldPwd;
            }
            user.NickPy = PingYinHelper.GetFirstSpell(user.Nick);
            await AccountRepository.UpdateAsync(user);
            return new UserUpdateResult<Account>
            {
                Success = true,
                Data = user,
                RoleChanged = roleChanged
            };
        }

        /// <summary>
        /// 删除机构用户
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public Task<bool> DeleteAccountUserAsync(string userid)
        {
            return AccountRepository.DeleteAsync(userid);
        }
        public Task<List<Identity>> GroupIdentityUsersAsync(string orgId)
        {
            return AccountRepository.GroupIdentityUsersAsync(orgId);
        }
        /// <summary>
        /// 按关键字搜索用户
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="kw"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Identity>> IdentitySuggestsAsync(string orgId, string kw)
        {
            if (kw.IsNullOrEmpty())
            {
                return new List<Identity>();
            }
            IEnumerable<Account> users;
            if (kw.TryExtractObjectIdArray(out var arr))
            {

                users = await AccountRepository.FindAsync(p => p.OrgId == orgId && arr.Contains(p.Id) && !p.Ghost);
            }
            else
            {
                users = await AccountRepository.FindAsync(p => p.OrgId == orgId && p.Nick.Contains(kw) && !p.Ghost);
            }

            return users.Select(p => new Identity { Id = p.Id, Name = p.Nick });
        }

        /// <summary>
        /// 更新系统用户档案
        /// </summary>
        /// <param name="clientId">系统用户ID</param>
        /// <param name="model">更新模型</param>
        /// <returns></returns>
        public async Task<bool> UpdateUserProfileAsync(string clientId, UpdateProfileModel model)
        {
            var user = await AccountRepository.GetAsync(clientId);
            if (user == null)
            {
                return false;
            }

            var r = model.Validate();
            if (!r.Success)
            {
                throw new Exception(r.Error);
            }

            if (user.Nick != model.Nick) //修改了昵称
            {
                //更改系统数据中相关该用户ID相关昵称
                user.NickPy = PingYinHelper.GetFirstSpell(user.Nick);
            }

            model.MapTo(user);
            return await AccountRepository.UpdateAsync(user);
        }

        /// <summary>
        /// 修改系统用户当前密码
        /// </summary>
        /// <param name="clientId">系统用户ID</param>
        /// <param name="model">修改模型</param>
        /// <returns></returns>
        public async Task<bool> ChangeUserPasswordAsync(string clientId, ChangePasswordModel model)
        {
            var user = await AccountRepository.GetAsync(clientId);
            if (user == null)
            {
                throw new Exception("用户不存在");
            }

            if (model.OldPwd != clientId && !user.LoginPwd.EqualsIgnoreCase(model.OldPwd))
            {
                throw new Exception("旧密码不匹配");
            }

            #region 密码强度检测

            if (model.NewPwd.Contains(user.LoginId))
            {
                throw new Exception("密码不能包含用户名信息");
            }
            if (model.NewPwd.Length < 8)
            {
                throw new Exception("新密码至少需8位");
            }
            //密码必须是长度为8到16位之间的字母和数字组合
            if (model.NewPwd.IsWeakPassword())
            {
                throw new Exception("密码强度过弱，密码必须是长度为8到16位之间的字母和数字组合");
            }
            #endregion


            var updates = new Dictionary<Expression<Func<Account, object>>, object>
            {
                {p => p.LoginPwd, model.NewPwd},
                { p => p.NeedChangePassword, false}
            };
            await AccountRepository.UpdateAsync(user.Id, updates);
            return true;

        }

        /// <summary>
        /// 找回用户密码
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<Result<IdentSecurity>> FindUserPasswordAsync(MobileValidateModel model)
        {
            if (!model.Mobile.IsMobile())
            {
                throw new Exception("手机号格式不正确");
            }
            var mobileValidate = await SmsService.ValidateAuthenticationAsync(model.Mobile, model.SmsCode);

            if (!mobileValidate)
            {
                return Result<IdentSecurity>.Fail("短信验证码错误");
            }

            var user = await AccountRepository.FirstAsync(p =>
                p.Mobile == model.Mobile && string.IsNullOrEmpty(p.OrgId) == false);
            if (user == null)
            {
                return Result<IdentSecurity>.Fail("手机号不存在");
            }
            //返回安全会话码
            var securityKey = RandomizeHelper.GenerateSecurityKey();
            var uid = user.Id;
            return Result<IdentSecurity>.Ok(new IdentSecurity
            {
                Id = uid,
                SecurityKey = securityKey
            });
        }

        /// <summary>
        /// 找回密码之重设密码
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<Result<bool>> ReSetPasswordAsync(ReSetPasswordModel model)
        {
            var securityKey = model.Security.SecurityKey;
            var uid = model.Security.Id;
            if (!securityKey.IsValidSecurityKey(600))
            {
                return Result<bool>.Fail("安全码已失效");
            }

            if (model.NewPassword.IsNullOrEmpty())
            {
                return Result<bool>.Fail("新密码不能为空");
            }
            if (model.NewPassword.Length < 6)
            {
                throw new Exception("新密码至少需6位");
            }
            var user = await FindAccountAsync(uid);
            if (user == null)
            {
                return Result<bool>.Fail("用户不存在");

            }
            var updates = new Dictionary<Expression<Func<Account, object>>, object>
            {
                {p => p.LoginPwd, model.NewPassword},
                { p => p.NeedChangePassword, false}
            };
            await AccountRepository.UpdateAsync(uid, updates);

            return Result<bool>.Successed;

        }

        /// <summary>
        /// 绑定手机号
        /// </summary>
        /// <param name="id"></param>
        /// <param name="mobile">手机号</param>
        /// <param name="smsCode">短信验证码</param>
        /// <returns></returns>
        public async Task<Result<bool>> BindMobileAsync(string id, string mobile,
            SmsCodeType smsCode)
        {
            #region 绑定手机号验证

            var mobileValidate = await SmsService.ValidateAuthenticationAsync(mobile, smsCode);

            if (!mobileValidate)
            {
                return Result<bool>.Fail("短信验证码错误");
            }

            var user = await AccountRepository.GetAsync(id);
            if (user == null)
            {
                return Result<bool>.Fail("当前用户不存在");
            }

            if (user.Mobile == mobile)
            {
                return Result<bool>.Fail("新绑定的手机号不能有旧手机号相同");
            }

            if (await AccountRepository.AnyAsync(p => p.Mobile == mobile && p.OrgId == user.OrgId))
            {
                return Result<bool>.Fail("手机号已经存在，不能重复绑定");
            }

            #endregion

            return Result<bool>.From(await AccountRepository.UpdateAsync(user, p => p.Mobile, mobile));

        }
        /// <summary>
        /// 解除当前手机绑定
        /// </summary>
        /// <param name="id"></param>
        /// <param name="smsCode"></param>
        /// <returns></returns>
        public async Task<Result<bool>> UnBindMobileAsync(string id, SmsCodeType smsCode)
        {
            #region 绑定手机号验证
            var user = await AccountRepository.GetAsync(id);
            if (user == null)
            {
                return Result<bool>.Fail("当前用户不存在");
            }

            var mobile = user.Mobile;
            if (!mobile.IsMobile())
            {
                return Result<bool>.Fail("当前用户尚未绑定手机，无需进行解绑");
            }

            var mobileValidate = await SmsService.ValidateAuthenticationAsync(mobile, smsCode);

            if (!mobileValidate)
            {
                return Result<bool>.Fail("短信验证码错误");
            }



            #endregion

            return Result<bool>.From(await AccountRepository.RemoveMobileAsync(id));

        }

        public Task<bool> ClearOrganizationUserAsync(string orgId)
        {
            return AccountRepository.DeleteAsync(p => p.OrgId == orgId);
        }

        public Task ChangeLoginIdAsync(string accountId, string newLoginId)
        {
            return AccountRepository.UpdateAsync(accountId, p => p.LoginId, newLoginId);
        }

        public Task<IEnumerable<Account>> FindRepeatMobileAccountAsync()
        {
            return AccountRepository.FindRepeatMobileAccountAsync();
        }

        /// <summary>
        /// 获取弱密码用户
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Account>> FindWeakPasswordAccountsAsync(string orgId)
        {
            var allAccounts = orgId.IsObjectId() ? await AccountRepository.FindAsync(p => p.OrgId == orgId) : await AccountRepository.FindAllAsync();

            return allAccounts.Where(p => p.LoginPwd.IsWeakPassword());
        }

        /// <summary>
        /// 强制指定用户必须在下次登录前修改密码
        /// </summary>
        /// <param name="userIds">用户ID数组</param>
        /// <param name="needChangePassword">强制修改密码标志</param>
        /// <returns></returns>
        public Task<bool> SetUserNeedChangePasswordAsync(IEnumerable<string> userIds, bool needChangePassword)
        {
            return AccountRepository.SetUserNeedChangePasswordAsync(userIds, needChangePassword);
        }

        /// <summary>
        /// 查找第一个手机号用户，按注册日期
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public Task<Account> FindFirstMobileAccountAsync(string mobile)
        {
            return AccountRepository.FirstAsync(p => p.Mobile == mobile);
        }

        /// <summary>
        /// 解除微信绑定
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="appId"></param>
        /// <returns></returns>d
        public async Task<Result<bool>> UnBindAccountWxAsync(string clientId, string appId)
        {
            var account = await FindAccountAsync(clientId);
            var accountIdentifiers = account.AppUserIdentifys ?? new List<AppOpenIdentify>();

            if (accountIdentifiers.Any(p => p.AppId == appId))
            {
                accountIdentifiers.RemoveAll(p => p.AppId == appId);
            }
            var result = await AccountRepository.UpdateAsync(clientId, p => p.AppUserIdentifys, accountIdentifiers);
            if (result)
            {
                return Result<bool>.Ok(true);
            }
            return Result<bool>.Failed;
        }

        /// <summary>
        /// 设置实名认证信息
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="cardCert"></param>
        /// <returns></returns>
        public Task SetCardCertValidateAsync(string clientId, IdCert cardCert)
        {
            return AccountRepository.UpdateAsync(clientId, p => p.Cert, cardCert);
        }

        /// <summary>
        /// 更新已征得车主授权
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="agree"></param>
        /// <returns></returns>
        public Task<bool> UpdateAgreementAsync(string clientId, bool agree)
        {
            return AccountRepository.UpdateAsync(clientId, p => p.Agreement, agree);
        }

        /// <summary>
        /// 使用openId进行登陆
        /// </summary>
        /// <param name="appId">APPID</param>
        /// <param name="openId">OPENID</param>
        /// <returns></returns>
        public async Task<Result<Account>> LoginAccountWithOpenIdAsync(string appId, string openId)
        {
            var ac = await AccountRepository.FirstAsync(p => p.AppUserIdentifys.Any(i => i.AppId == appId && i.OpenId == openId));
            if (ac != null)
            {
                return Result<Account>.Ok(ac);
            }

            return Result<Account>.Fail("此授权用户尚未创建账号，请先进行注册");
        }

        /// <summary>
        /// Removes the auth key async.
        /// </summary>
        /// <returns>The auth key async.</returns>
        /// <param name="authKey">Auth key.</param>
        public async Task RemoveAuthKeyAsync(string authKey)
        {
            await RedisKeyCache.KeyDeleteAsync(authKey);
        }
    }
}