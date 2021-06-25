using System.Collections.Generic;
using System.Threading.Tasks;
using AgvcCoreData.Users;
using AgvcEntitys.System.Authority;
using AgvcEntitys.Users;
using AgvcService.System.Models;
using AgvcService.System.Models.Authorization;
using AgvcService.Users;
using CoreData;
using CoreService;
using Newtonsoft.Json;
using Utility.Extensions;

namespace AgvcService.System
{
    public class OpenAuthorizationService : AbstractService, IOpenAuthorizationService

    {
        #region IOC

        private IAccountService AccountService { get; }
        private IPassportProtectService PassportProtectService { get; }
        // private ICaptchaService CaptchaService { get; }
        private IAuthorityService AuthorityService { get; }
        private ISystemUserService SystemUserService { get; }
        // private IAppUserService AppUserService { get; }
        // private IWxAppService WxAppService { get; }
        public OpenAuthorizationService(
            IAccountService accountService,
            IPassportProtectService passportProtectService,
         //   ICaptchaService captchaService,
            IAuthorityService authorityService,
            ISystemUserService systemUserService)
        {
            AccountService = accountService;
            PassportProtectService = passportProtectService;
          //  CaptchaService = captchaService;
            AuthorityService = authorityService;
            SystemUserService = systemUserService;
            // AppUserService = appUserService;
            // WxAppService = wxAppService;
        }

        #endregion

        #region Implementation of IOpenAuthorizationService

        public Task<Result<Account>> LoginOpenIdAsync(string appId, string openId)
        {
            return AccountService.LoginAccountWithOpenIdAsync(appId, openId);

        }

        public async Task<Result<Account>> LoginAccountWithPasswordAsync(PasswordLoginModel pwdLoginModel)
        {
            // if (await CheckSafetyValidationAsync(pwdLoginModel.VaptchaToken,
            //     pwdLoginModel.UserName,
            //     pwdLoginModel.LoginType))
            // {
            //     return Result<Account>.Fail("NeedSafetyValidation");
            // }

            var rs = await AccountService.LoginAccountWithPasswordAsync(pwdLoginModel.UserName, pwdLoginModel.Password,
                pwdLoginModel.Identify, pwdLoginModel.AppUserInfo, pwdLoginModel.LoginDomain);

            await SetFailureCountAsync(pwdLoginModel.UserName,pwdLoginModel.LoginType,rs.Success);

            return rs;

        }

        private async Task SetFailureCountAsync(string userName, string loginType, bool success)
        {
            if (success == false)
            {
                await PassportProtectService.IncreaseLoginFailedAsync(userName, loginType);
            }
            else
            {
                await PassportProtectService.ClearLoginFailedAsync(userName, loginType);

            }
        }

        public Task<Result<Account>> SmsLoginAsync(SmsLoginModel smsLoginModel)
        {
            return smsLoginModel.JustLogin
                  ? AccountService.SmsLoginAsync(smsLoginModel.Mobile,
                      smsLoginModel.SmsCode,
                      smsLoginModel.SmsKey,
                      smsLoginModel.Identify,
                      smsLoginModel.AppUserInfo,
                      smsLoginModel.LoginDomain)
                  : AccountService.SmsLoginAndCreateAccountAsync(smsLoginModel.Mobile,
                      smsLoginModel.SmsCode,
                      smsLoginModel.SmsKey,
                      smsLoginModel.Identify,
                      smsLoginModel.AppUserInfo,
                      smsLoginModel.Referrer,
                      smsLoginModel.LoginDomain);
        }

        public Task<Result<Account>> LoginAccountWithAuthKeyAsync(string clientId, string authkey)
        {
            return AccountService.LoginAccountWithAuthKeyAsync(clientId, authkey);
        }

        public async Task<Result<Account>> LoginAppAccountAsync(AppOpenIdentify openIdentify, string loginDomain)
        {
            var account = await AccountService.LoginAppAccountAsync(openIdentify, loginDomain);
            return Result<Account>.From(account);
        }

        public async Task<Result<Account>> SysUserLoginAsync(SystemUserLoginModel systemUserLoginModel)
        {
            /*if (await CheckSafetyValidationAsync(systemUserLoginModel.VaptchaToken,
                systemUserLoginModel.LoginId,
                systemUserLoginModel.LoginType))
            {
                return Result<Account>.Fail("NeedSafetyValidation");
            }*/

            var rs = await SystemUserService.LoginAsync(systemUserLoginModel);
            await SetFailureCountAsync(systemUserLoginModel.LoginId, systemUserLoginModel.LoginType, rs.Success);
            var account = rs.Data.MapTo(new Account());
            return Result<Account>.From(account);
        }

        public Task<IEnumerable<UserAuthority>> GetUserAuthoritysAsync(string roleId, string userId)
        {
            return AuthorityService.GetUserAuthoritysAsync(roleId,userId);
        }

        public Task CacheUserAuthoritysAsync(string orgId, string id, IEnumerable<string> menuIdArray, IEnumerable<string> codeArray)
        {
            return AuthorityService.CacheUserAuthoritysAsync(orgId,id,menuIdArray,codeArray);
        }

        #endregion
        /*private async Task<bool> CheckSafetyValidationAsync(string vaptchaToken, string contextUserName, string scope)
        {
            if (vaptchaToken.IsNotNullOrEmpty())
            {
                var vaptchaValidated = await VaptchaValidateAsync(vaptchaToken);
                return !vaptchaValidated;
            }
            var failureCount = await PassportProtectService.GetLoginFailedCountAsync(contextUserName, scope);
            return failureCount >= 3;//超过3次出现拼图
        }*/
        /*private async Task<bool> VaptchaValidateAsync(string vaptchaToken)
        {
            if (vaptchaToken.IsNotNullOrEmpty())
            {
                //腾讯云验证码
                if (vaptchaToken.ContainsAll("appid", "randstr"))
                {
                    var req = JsonConvert.DeserializeObject<CaptchaRequest>(vaptchaToken);

                    return await PassportProtectService.ValidateCaptchaResult(req);
                }

                //vaptcha手势验证码
                var token = new VaptchaToken
                {
                    id = "5f0d5b7c8d41fe366eb1e618",
                    secretkey = "7af11c738af0490aa5687b0b6c3e2a91",
                    token = vaptchaToken,
                    ip = NetworkHelper.RealIpAddress,
                    scene = 0
                };

                return await CaptchaService.VaptchaValidateAsync(token);

            }

            return false;
        }*/
    }
}