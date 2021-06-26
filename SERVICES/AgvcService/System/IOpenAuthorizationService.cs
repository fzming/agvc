using System.Collections.Generic;
using System.Threading.Tasks;
using AgvcCoreData.Users;
using AgvcEntitys.System.Authority;
using AgvcEntitys.Users;
using AgvcService.System.Models;
using AgvcService.System.Models.Authorization;
using CoreData;
using CoreService.Interfaces;

namespace AgvcService.System
{
    public interface IOpenAuthorizationService : IService
    {
        Task<Result<Account>> LoginOpenIdAsync(string appId, string openId);
        Task<Result<Account>> LoginAccountWithPasswordAsync(PasswordLoginModel pwdLoginModel);
        Task<Result<Account>> SmsLoginAsync(SmsLoginModel smsLoginModel);
        Task<Result<Account>> LoginAccountWithAuthKeyAsync(string contextUserName, string contextPassword);
        Task<Result<Account>> LoginAppAccountAsync(AppOpenIdentify openIdentify, string loginDomain);
        Task<Result<Account>> SysUserLoginAsync(SystemUserLoginModel systemUserLoginModel);
        Task<IEnumerable<UserAuthority>> GetUserAuthoritysAsync(string roleId, string userId);

        Task CacheUserAuthoritysAsync(string orgId, string id, IEnumerable<string> menuIdArray,
            IEnumerable<string> codeArray);
    }
}