using AgvcEntitys.System.Authority;
using AgvcRepository.System.Interfaces.Authority;
using CoreRepository;

namespace AgvcRepository.System.Authority
{
    /// <summary>
    /// 指令权限仓储实现
    /// </summary>
    public class AuthorityCodeRepository:MongoRepository<AuthorityCode>,IAuthorityCodeRepository
    {
        protected AuthorityCodeRepository(IMongoUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}