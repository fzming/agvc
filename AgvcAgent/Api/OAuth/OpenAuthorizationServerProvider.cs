using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json;
using WeiMan.Common.Interface;
using WeiMan.Common.Interface.Extensions;
using WeiMan.Common.Interface.Helpers;
using WeiMan.Common.Interface.Ioc;
using WeiMan.Datas.Core.Activitys;
using WeiMan.Datas.Core.Wechat;
using WeiMan.Datas.Dto.System;
using WeiMan.Datas.Dto.System.Authorization;
using WeiMan.Datas.Entitys.Mongo.Users;
using WeiMan.OpenApi.Providers;
using WeiMan.Service.Interface.System;

namespace WeiMan.OpenApi.OAuth
{
    /// <summary>
    /// OAUTH Authorization Server
    /// </summary>
    public class OpenAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {


        #region 注入依赖
        [Import(typeof(IOpenAuthorizationService))]
        private IOpenAuthorizationService OpenAuthorizationService { get; set; }
        /// <summary>
        /// OAuth授权服务提供者
        /// </summary>
        public OpenAuthorizationServerProvider()
        {
            Injector.SatisfyImportsOnce(this);
        }


        #endregion
        /// <summary>
        /// 验证调用端的clientId与clientSecret的合法性
        /// (clientId、clientSecret为约定好的字符串)。
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
            return Task.FromResult<object>(null);
            //return base.ValidateClientAuthentication(context);



            //            if (context.TryGetBasicCredentials(out var clientId, out var clientSecret))
            //            {
            //                if (await TokenService.ValidateClientSecretAsync(clientId, clientSecret))
            //                {
            //                    context.OwinContext.Set<string>("as:client_id", clientId);
            //                    context.OwinContext.Set<string>("as:client_secret", clientSecret);
            //                    context.Validated(clientId);
            //                }
            //            }
            //            /*
            //             * context结构：
            //             *   this.ClientId = clientId;
            //                  this.UserName = userName;
            //                  this.Password = password;
            //                  this.Scope = scope; [可选]
            //             */


        }


        /// <summary>
        ///  通过重载GrantResourceOwnerCredentials获取用户名和密码进行认证
        ///  验证通过后会颁发token。
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var parameters = await context.Request.ReadFormAsync();
            if (parameters == null)
            {
                context.SetError("invalid_grant", "Invalid request");
                return;
            }
            var loginType = parameters.Get("loginType");
            var loginDomain = parameters.Get("domain");
            var isSys = false;
            Result<Account> rs;
            switch (loginType)
            {
                //使用OpenId登陆，小程序，微信公众号
                case "openid":
                    {
                        rs = await OpenAuthorizationService.LoginOpenIdAsync(context.UserName, context.Password);
                        break;
                    }

                //机构用户-短信登陆
                case "smslogin":
                    {
                        var smsLoginModel = new SmsLoginModel
                        {
                            Mobile = context.UserName,
                            SmsCode = context.Password,
                            SmsKey = parameters.Get("smskey"),
                            JustLogin = parameters.Get("justLogin").ToBool(), //仅登录，不创建账户
                            Identify = GetAppIdentify(parameters),
                            AppUserInfo = GetAppUserInfo(parameters),
                            LoginDomain = loginDomain,
                            Referrer = new ActReferrer
                            {
                                ActId = parameters.GetString("actId"),
                                UserId = parameters.GetString("referrer")
                            }
                        };

                        rs = await OpenAuthorizationService.SmsLoginAsync(smsLoginModel);

                        break;
                    }
                //密码登陆
                case "pwdlogin":
                    {

                        var pwdModel = new PasswordLoginModel
                        {
                            UserName = context.UserName,
                            Password = context.Password,
                            VaptchaToken = parameters.GetString("vaptchaToken"),
                            Identify = GetAppIdentify(parameters),
                            AppUserInfo = GetAppUserInfo(parameters),
                            LoginType = loginType,
                            LoginDomain = loginDomain
                        };
                        rs = await OpenAuthorizationService.LoginAccountWithPasswordAsync(pwdModel);

                        break;
                    }
                //通过授权密钥进行登录
                case "authkey":
                    {
                        rs = await OpenAuthorizationService.LoginAccountWithAuthKeyAsync(context.UserName, context.Password);
                        break;
                    }
                //微信扫码登录
                case "wxlogin":
                    {
                        var openIdentify = new AppOpenIdentify
                        {
                            AppId = context.UserName,
                            OpenId = context.Password,
                            UnionId = parameters.Get("unionId")
                        };
                        rs = await OpenAuthorizationService.LoginAppAccountAsync(openIdentify, loginDomain);
                        break;

                    }
                //系统人员登录
                case "syslogin":
                    {
                        isSys = true;
                        rs = await OpenAuthorizationService.SysUserLoginAsync(new SystemUserLoginModel
                        {
                            LoginId = context.UserName,
                            LoginPwd = context.Password,
                            VaptchaToken = parameters.GetString("vaptchaToken"),
                            LoginType = loginType
                        });

                        break;
                    }

                default:
                    context.SetError("invalid_grant", "Invalid  loginType");
                    return;
            }
            var account = rs?.Data;

            //调用后台的登录服务验证用户名与密码
            if (account == null)
            {
                context.SetError("invalid_grant", "用户名或密码不正确");
                return;
            }

            var menuIdArray = Enumerable.Empty<string>();
            var codeArray = Enumerable.Empty<string>();
            if (account.RoleId.IsNotNullOrEmpty())
            {
                var authoritys = await OpenAuthorizationService.GetUserAuthoritysAsync(account.RoleId, account.Id);
                menuIdArray = authoritys.Where(p => p.MenuId.IsNotNullOrEmpty()).GroupBy(p => p.MenuId)
                    .Select(p => p.Key);
                codeArray = authoritys.Where(p => p.AuthCode.IsNotNullOrEmpty()).GroupBy(p => p.AuthCode)
                    .Select(p => p.Key);
                await OpenAuthorizationService.CacheUserAuthoritysAsync(account.OrgId, account.Id, menuIdArray, codeArray);
            }

            var properties = new Dictionary<string, string>()
            {
                { ClaimTypes.NameIdentifier, account.Id },
                { ClaimTypes.Name, context.UserName },
                {"Mobile",account.Mobile.ToStringEx() },
                {"Phone",account.ContactPhone.ToStringEx() },
                {"OrgId",account.OrgId.IsNullOrEmpty()?"":account.OrgId },
                {"RoleId",account.RoleId.IsNullOrEmpty()?"":account.RoleId },
                {"ClientName",account.Nick.IsNullOrEmpty()?"":account.Nick },
                {"IsSys",isSys.ToString()},
                //拥有的菜单权限ID
                {"Menus",menuIdArray.JoinToString(",")},

                //拥有的指令权限ID
                {"Codes",codeArray.JoinToString(",")},
                {"DeviceIds",account.DeviceId},
            };

            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            properties.ToListEx().ForEach(c => identity.AddClaim(new Claim(c.Key, c.Value)));

            var ticket = new AuthenticationTicket(identity, new AuthenticationProperties(properties));

            context.Validated(ticket);
            context.Request.Context.Authentication.SignIn(identity);

        }
        /// <summary>
        /// 获取设备识别对象
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private AppOpenIdentify GetAppIdentify(IFormCollection parameters)
        {
            var openId = parameters.GetString("openid");
            var appId = parameters.GetString("appid");
            var unionId = parameters.GetString("unionid");
            AppOpenIdentify identify = null;
            if (appId.IsNotNullOrEmpty())
            {
                identify = new AppOpenIdentify
                {
                    AppId = appId,
                    OpenId = openId,
                    UnionId = unionId
                };
            }

            return identify;
        }
        /// <summary>
        /// 获取移动设备用户资料
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private AppUserInfo GetAppUserInfo(IFormCollection parameters)
        {
            var userInfo = parameters.GetString("userInfo");
            if (userInfo.IsNullOrEmpty())
            {
                return null;
            }
            return JsonConvert.DeserializeObject<AppUserInfo>(userInfo);
        }


    }
}