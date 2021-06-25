using System.Collections.Generic;
using System.Threading.Tasks;
using AgvcCoreData.Users;
using AgvcEntitys.Users;
using CoreData;
using CoreRepository;

namespace AgvcRepository.Users.Interfaces
{
    public interface IAccountRepository : IRepository<Account>, IDynamicRepository
    {
        Task<PageResult<Account>> AdvQueryAccountUsersAsync(AccountUserPageQuery userPageQuery, string orgId);
        Task<List<Identity>> GroupIdentityUsersAsync(string orgId);
        Task<bool> RemoveMobileAsync(string id);
        Task<IEnumerable<Account>> FindRepeatMobileAccountAsync();
        Task<bool> SetUserNeedChangePasswordAsync(IEnumerable<string> userIds, bool needChangePassword);
    }
}