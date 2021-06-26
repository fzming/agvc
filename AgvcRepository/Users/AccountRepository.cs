using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgvcCoreData.Users;
using AgvcEntitys.Users;
using AgvcRepository.Users.Interfaces;
using CoreData;
using CoreRepository;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Utility.Extensions;

namespace AgvcRepository.Users
{
    public class AccountRepository : MongoRepository<Account>, IAccountRepository
    {
        public AccountRepository(IMongoUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public Task<PageResult<Account>> AdvQueryAccountUsersAsync(AccountUserPageQuery userPageQuery, string orgId)
        {
            #region Build Query

            if (userPageQuery == null) userPageQuery = new AccountUserPageQuery();
            var query = Collection.AsQueryable();
            query = query.Where(p => p.OrgId == orgId && !p.Ghost);
            if (userPageQuery.Mobile.IsNotNullOrEmpty()) query = query.Where(p => p.Mobile == userPageQuery.Mobile);

            if (userPageQuery.BranchCompanyId.IsObjectId())
                query = query.Where(p => p.BranchCompanyId == userPageQuery.BranchCompanyId);
            if (userPageQuery.DepartmentId.IsObjectId())
                query = query.Where(p => p.DepartmentId == userPageQuery.DepartmentId);

            #endregion

            return query.ToPageListAsync(userPageQuery.PageIndex, userPageQuery.PageSize,
                p => p.CreatedOn, true);
        }

        public Task<List<Identity>> GroupIdentityUsersAsync(string orgId)
        {
            return Collection.AsQueryable().Where(p => p.OrgId == orgId && !p.Ghost).Select(p => new Identity
            {
                Id = p.Id,
                Name = p.Nick
            }).ToListAsync();
        }

        public async Task<bool> RemoveMobileAsync(string id)
        {
            var update = Updater.Unset(p => p.Mobile);
            return (await Collection.UpdateOneAsync(p => p.Id == id, update.CurrentDate(i => i.ModifiedOn)))
                .ModifiedCount > 0;
        }

        public async Task<IEnumerable<Account>> FindRepeatMobileAccountAsync()
        {
            var users = await Collection.AsQueryable().GroupBy(p => p.Mobile).Select(p => new
            {
                p.Key,
                Count = p.Count(),
                Users = p.Select(k => k)
            }).Where(p => p.Count > 1).ToListAsync();

            return users.SelectMany(p => p.Users);
        }

        public async Task<bool> SetUserNeedChangePasswordAsync(IEnumerable<string> userIds, bool needChangePassword)
        {
            var update = Builders<Account>.Update
                .Set(p => p.NeedChangePassword, needChangePassword)
                .CurrentDate(i => i.ModifiedOn);
            var rs = await Collection.UpdateManyAsync(Filter.In(x => x.Id, userIds), update);
            return rs.ModifiedCount > 0;
        }
    }
}