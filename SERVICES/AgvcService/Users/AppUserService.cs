using System.Threading.Tasks;
using AgvcEntitys.Users;
using AgvcRepository.Users.Interfaces;
using CoreService;

namespace AgvcService.Users
{
    public class AppUserService: AbstractCrudService<AppUser>,IAppUserService
    {
        #region IOC

        private IAppUserRepository AppUserRepository { get; }

        public AppUserService(IAppUserRepository appUserRepository) : base(appUserRepository)
        {
            AppUserRepository = appUserRepository;
        }

        #endregion

        public Task<AppUser> FindAppUserAsync(string appId, string openid)
        {
            return AppUserRepository.FirstAsync(p => p.AppId == appId && p.OpenId == openid);
        }
    }
}