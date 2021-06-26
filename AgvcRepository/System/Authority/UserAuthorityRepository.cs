using System.Collections.Generic;
using System.Threading.Tasks;
using AgvcEntitys.System.Authority;
using AgvcRepository.System.Interfaces.Authority;
using CoreRepository;

namespace AgvcRepository.System.Authority
{
    /// <summary>
    /// 用户权限定义仓储实现
    /// </summary>
    public class UserAuthorityRepository : MongoRepository<UserAuthority>, IUserAuthorityRepository
    {
        public Task DenyAuthoritysAsync(IEnumerable<string> denyAuthoritys,bool deny)
        {
            return UpdateAsync(Filter.In(p => p.Id, denyAuthoritys), p => p.UserDeny, deny);
        }

        public UserAuthorityRepository(IMongoUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}