using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AgvcEntitys.Users;
using AgvcRepository.Users.Interfaces;
using CoreService;

namespace AgvcService.Users
{
    [Export(typeof(IAppUserService))]
    internal class AppUserService: AbstractCrudService<AppUser>,IAppUserService
    {
        #region IOC

        private IAppUserRepository AppUserRepository { get; }

        [ImportingConstructor]
        public AppUserService(IAppUserRepository appUserRepository) : base(appUserRepository)
        {
            AppUserRepository = appUserRepository;
        }

        #endregion

        public Task<AppUser> FindAppUserAsync(string appId, string openid)
        {
            return AppUserRepository.FirstAsync(p => p.AppId == appId && p.OpenId == openid);
        }

        public Task<bool> UpdateCompanyAsync(string clientId, Company company)
        {
            IDictionary<Expression<Func<AppUser, object>>, object> updates = new Dictionary<Expression<Func<AppUser, object>>, object>
            {
                { p => p.OrgId, company.OrgId },
                { p => p.ConsignorId, company.Id }
            };
            return AppUserRepository.UpdateAsync(clientId, updates);
        }
    }
}