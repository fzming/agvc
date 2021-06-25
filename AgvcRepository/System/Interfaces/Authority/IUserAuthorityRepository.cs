using System.Collections.Generic;
using System.Threading.Tasks;
using AgvcEntitys.System.Authority;
using CoreRepository;

namespace AgvcRepository.System.Interfaces.Authority
{
    public interface IUserAuthorityRepository:IRepository<UserAuthority>
    {
        Task DenyAuthoritysAsync(IEnumerable<string> denyAuthoritys, bool deny);
    }
}