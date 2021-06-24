using System.Threading.Tasks;

using AgvcRepository.Entitys;

using CoreRepository;

using Utility;

namespace AgvcRepository
{
    public class MrRepository : MongoRepository<MrEntity>, IMrRepository
    {
        public MrRepository(IMongoUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }

        #region Implementation of IMrRepository

        public Task CreateAsync(MrEntity mrEntity)
        {
            return InsertAsync(mrEntity);
        }

        #endregion
    }

    public interface IMrRepository : IMongoRepository<MrEntity>, ISingletonDependency
    {
        Task CreateAsync(MrEntity mrEntity);
    }

}