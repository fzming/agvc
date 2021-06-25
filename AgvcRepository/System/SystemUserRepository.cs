using System.Threading.Tasks;
using AgvcCoreData.System;
using AgvcEntitys.System;
using AgvcRepository.System.Interfaces;
using CoreData;
using CoreRepository;
using MongoDB.Driver;

namespace AgvcRepository.System
{
    public class SystemUserRepository:MongoRepository<SystemUser>,ISystemUserRepository
    {
        public Task<PageResult<SystemUser>> AdvQuerySystemUsersAsync(SystemUserPageQuery userPageQuery)
        {
            #region Build Query

            var query = Collection.AsQueryable();

            
            #endregion

            return query.ToPageListAsync(userPageQuery.PageIndex, userPageQuery.PageSize,
                p => p.CreatedOn, true);
        }

        protected SystemUserRepository(IMongoUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}