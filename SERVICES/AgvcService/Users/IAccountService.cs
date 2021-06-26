using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AgvcCoreData.Users;
using AgvcEntitys.Users;
using AgvcService.System.Models;
using AgvcService.Users.Models;
using CoreData;
using CoreService.Interfaces;

namespace AgvcService.Users
{
    /// <summary>
    /// Account service.
    /// </summary>
    public interface IAccountService : IService
    {
        /// <summary>
        /// Regsiters the async.
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="registerModel">Register model.</param>

        Task<Result<bool>> RegsiterAsync(RegisterModel registerModel);
        /// <summary>
        /// Finds the account async.
        /// </summary>
        /// <returns>The account async.</returns>
        /// <param name="id">Identifier.</param>
        Task<Account> FindAccountAsync(string id);

        Task<IEnumerable<Account>> FindAppAccountsAsync(AppOpenIdentify openIdentify, string orgId);
        Task<Result<bool>> UpdateAccountInfo(AccountUserProfile accountUserProfile);
        Task<Account> LoginAppAccountAsync(AppOpenIdentify openIdentify, string domain);

        #region 支持各种登录方式

        /// <summary>
        /// 密码登陆
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="identify">[可选]移动设备登陆需要传递openid，appid</param>
        /// <param name="appUserInfo"></param>
        /// <param name="loginDomain"></param>
        /// <returns></returns>
        Task<Result<Account>> LoginAccountWithPasswordAsync(string userName, string password, AppOpenIdentify identify,
            AppUserInfo appUserInfo, string loginDomain);

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
        Task<Result<Account>> SmsLoginAsync(string mobile, string smsCode, string smsKey,
            AppOpenIdentify identify, AppUserInfo appUserInfo, string loginDomain);

        /// <summary>
        /// 短信验证码登陆并创建账户
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="smsCode"></param>
        /// <param name="smsKey"></param>
        /// <param name="identify">[可选]移动设备登陆需要传递openid，appid</param>
        /// <param name="appUserInfo"></param>
        /// <param name="loginDomain"></param>
        /// <returns></returns>
        Task<Result<Account>> SmsLoginAndCreateAccountAsync(string mobile, string smsCode, string smsKey,
            AppOpenIdentify identify, AppUserInfo appUserInfo, string loginDomain);

        /// <summary>
        /// Logins the account with auth key async.
        /// </summary>
        /// <returns>The account with auth key async.</returns>
        /// <param name="clientId"></param>
        /// <param name="authkey"></param>
        Task<Result<Account>> LoginAccountWithAuthKeyAsync(string clientId, string authkey);
        /// <summary>
        /// Logins the account with open identifier async.
        /// </summary>
        /// <returns>The account with open identifier async.</returns>
        /// <param name="appId">App identifier.</param>
        /// <param name="openId">Open identifier.</param>
        Task<Result<Account>> LoginAccountWithOpenIdAsync(string appId, string openId);
        #endregion

        /// <summary>
        /// Creates the auth key async.
        /// </summary>
        /// <returns>The auth key async.</returns>
        /// <param name="clientId">Client identifier.</param>
        /// <param name="expires">AuthKey过期时间</param>
        Task<string> CreateAuthKeyAsync(string clientId, TimeSpan expires);

        /// <summary>
        /// 查询所有用户信息
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Account>> QueryAllAsync();

        /// <summary>
        /// 绑定小程序账号信息
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="identify"></param>
        /// <returns></returns>
        Task<bool> UpdateAppOpenIdentifyAsync(string clientId, AppOpenIdentify identify);

        /// <summary>
        /// 创建机构帐户
        /// </summary>
        /// <param name="createAccountModel"></param>
        /// <param name="orgId"></param>
        /// <returns></returns>
        Task<Result<Account>> CreateAccountAsync(CreateAccountModel createAccountModel, string orgId);
        /// <summary>
        /// 查询机构用户
        /// </summary>
        /// <param name="userPageQuery"></param>
        /// <param name="orgId"></param>
        /// <returns></returns>
        Task<PageResult<Account>> QueryAccountUsersAsync(AccountUserPageQuery userPageQuery, string orgId);
       /// <summary>
       /// 查询机构所有用户列表
       /// </summary>
       /// <param name="orgId"></param>
       /// <returns></returns>
        Task<IEnumerable<Account>> QueryAccountUsersAsync(string orgId);
       /// <summary>
       /// 获取某角色的用户列表
       /// </summary>
       /// <param name="orgId"></param>
       /// <param name="roleId"></param>
       /// <returns></returns>
        Task<IEnumerable<Account>> QueryRoleAccountsAsync(string orgId, string roleId);

        /// <summary>
        /// 更新机构用户
        /// </summary>
        /// <param name="userUpdateModel"></param>
        /// <returns></returns>
        Task<UserUpdateResult<Account>> UpdateAccountUserAsync(AccountUserUpdateModel userUpdateModel);
        /// <summary>
        /// 删除机构用户
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        Task<bool> DeleteAccountUserAsync(string userid);
        /// <summary>
        /// 机构人员搜索建议提示分组
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        Task<List<Identity>> GroupIdentityUsersAsync(string orgId);
        /// <summary>
        /// 模糊查询机构用户
        /// </summary>
        /// <param name="orgId">机构ID</param>
        /// <param name="kw">昵称关键字</param>
        /// <returns></returns>
        Task<IEnumerable<Identity>> IdentitySuggestsAsync(string orgId, string kw);
        /// <summary>
        /// 更新用户档案
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool> UpdateUserProfileAsync(string clientId, UpdateProfileModel model);
        /// <summary>
        /// 修改用户密码
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool> ChangeUserPasswordAsync(string clientId, ChangePasswordModel model);

        /// <summary>
        /// 找回用户密码
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<Result<IdentSecurity>> FindUserPasswordAsync(MobileValidateModel model);
        /// <summary>
        /// 找回密码之重设密码
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<Result<bool>> ReSetPasswordAsync(ReSetPasswordModel model);

        /// <summary>
        /// 绑定手机号
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <param name="mobile">手机号</param>
        /// <param name="smsCode">短信验证码</param>
        /// <returns></returns>
        Task<Result<bool>> BindMobileAsync(string id, string mobile, SmsCodeType smsCode);
        /// <summary>
        /// 解除绑定手机号
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <param name="smsCode">短信验证码</param>
        /// <returns></returns>
        Task<Result<bool>> UnBindMobileAsync(string id, SmsCodeType smsCode);

        /// <summary>
        /// 清除机构所有人员
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        Task<bool> ClearOrganizationUserAsync(string orgId);

        Task ChangeLoginIdAsync(string accountId, string newLoginId);
        Task<IEnumerable<Account>> FindRepeatMobileAccountAsync();

        /// <summary>
        /// 获取弱密码用户
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        Task<IEnumerable<Account>> FindWeakPasswordAccountsAsync(string orgId);

        /// <summary>
        /// 强制指定用户必须在下次登录前修改密码
        /// </summary>
        /// <param name="userIds">用户ID数组</param>
        /// <param name="needChangePassword">强制修改密码标志</param>
        /// <returns></returns>
        Task<bool> SetUserNeedChangePasswordAsync(IEnumerable<string> userIds, bool needChangePassword);
        /// <summary>
        /// 查找第一个手机号用户，按注册日期
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        Task<Account> FindFirstMobileAccountAsync(string mobile);
        /// <summary>
        /// 解除微信绑定
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        Task<Result<bool>> UnBindAccountWxAsync(string clientId, string appId);
        /// <summary>
        /// 设置实名认证信息
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="cardCert"></param>
        /// <returns></returns>
        Task SetCardCertValidateAsync(string clientId, IdCert cardCert);
        /// <summary>
        /// 更新已征得车主授权
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="agree"></param>
        /// <returns></returns>
        Task<bool> UpdateAgreementAsync(string clientId, bool agree);
    }
}