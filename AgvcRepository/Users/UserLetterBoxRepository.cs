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
    /// <summary>
    ///     用户收件箱仓储实现
    /// </summary>
    public class UserLetterBoxRepository : MongoRepository<UserLetterBox>, IUserLetterBoxRepository
    {
        public UserLetterBoxRepository(IMongoUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public Task<PageResult<UserLetterBox>> AdvanceQueryUserLetterBoxAsync(string clientId,
            LetterBoxPageQuery condition)
        {
            #region Build Query

            var query = Collection.AsQueryable().Where(p => p.UserId == clientId);

            if (condition.category.IsNotNullOrEmpty())
            {
                var categorys = condition.category.Split(',');
                query = query.Where(p => categorys.Contains(p.Category));
            }

            if (condition.readflag != ReadFlagType.不限)
            {
                var read = condition.readflag == ReadFlagType.已读;
                query = query.Where(p => p.Read == read);
            }


            if (condition.btm.HasValue) query = query.Where(p => p.CreatedOn >= condition.btm.Value);

            if (condition.etm.HasValue) query = query.Where(p => p.CreatedOn <= condition.etm.Value);

            #endregion

            return query.ToPageListAsync(condition.PageIndex, condition.PageSize,
                p => p.CreatedOn, true);
        }

        public async Task<bool> UserReadAllAsync(string clientId)
        {
            var rs = await Collection.UpdateManyAsync(p => p.UserId == clientId && p.Read == false,
                Updater.Set(p => p.Read, true));
            return rs.IsAcknowledged;
        }
    }
}