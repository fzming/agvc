using System.Threading.Tasks;
using CoreRepository;

namespace AgvcRepository.Users.Interfaces
{
    public interface IAccountTokenRepository:IRepository<AccountToken>
    {
        Task SaveTokenAsync(ClientIdSecret client, AccessToken tk);
        Task<AccountToken> FindAccessToken(ClientIdSecret client);
    }
}