using AgvcEntitys.System.Authority;
using AgvcRepository.System.Interfaces.Authority;
using CoreRepository;

namespace AgvcRepository.System.Authority
{
    /// <summary>
    /// 角色仓储实现
    /// </summary>
    internal class RoleRepository : MongoRepository<Role>, IRoleRepository
    {
        protected RoleRepository(IMongoUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}