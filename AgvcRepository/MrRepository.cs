using System.Threading.Tasks;
using AgvcEntitys.Agv;
using CoreRepository;

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

    public interface IMrRepository : IRepository<MrEntity>
    {
        Task CreateAsync(MrEntity mrEntity);
    }
}