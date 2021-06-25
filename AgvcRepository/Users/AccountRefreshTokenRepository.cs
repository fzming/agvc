using AgvcEntitys.Users;
using AgvcRepository.Users.Interfaces;
using CoreRepository;

namespace AgvcRepository.Users
{
    [Export(typeof(IAccountRefreshTokenRepository))]
    internal class AccountRefreshTokenRepository:MongoRepository<AccountRefreshToken>,IAccountRefreshTokenRepository
    {
        
    }
}