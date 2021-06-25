using System.Threading.Tasks;
using AgvcEntitys.Users;
using CoreService.Interfaces;

namespace AgvcService.Users
{
    public interface IAppUserService : ICrudService<AppUser>
    {
        Task<AppUser> FindAppUserAsync(string appId, string openid);
        Task<bool> UpdateCompanyAsync(string clientId, Company company);
    }
}