using AgvcEntitys.System.Authority;
using AgvcRepository.System.Interfaces.Authority;
using CoreRepository;

namespace AgvcRepository.System.Authority
{
    /// <summary>
    /// 角色仓储实现
    /// </summary>
    public class RoleRepository : MongoRepository<Role>, IRoleRepository
    {
        public RoleRepository(IMongoUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}