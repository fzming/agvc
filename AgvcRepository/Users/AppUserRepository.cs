using AgvcEntitys.Users;
using AgvcRepository.Users.Interfaces;
using CoreRepository;

namespace AgvcRepository.Users
{
    [Export(typeof(IAppUserRepository))]
    internal class AppUserRepository : MongoRepository<AppUser>, IAppUserRepository
    {

    }
}