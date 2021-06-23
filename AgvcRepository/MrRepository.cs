using AgvcRepository.Entitys;
using CoreRepository;

namespace AgvcRepository
{
    public class MrRepository : MongoRepository<MrEntity>, IMrRepository
    {

    }

    public interface IMrRepository : IRepository<MrEntity>
    {

    }
}