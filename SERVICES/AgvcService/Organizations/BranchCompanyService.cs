using AgvcEntitys.Organization;
using AgvcRepository.Orgnizations.Interfaces;
using CoreService;

namespace AgvcService.Organizations
{

    public class BranchCompanyService : AbstractCrudService<BranchCompany>, IBranchCompanyService
    {
        #region IOC

        private IBranchCompanyRepository BranchCompanyRepository { get; }

        public BranchCompanyService(IBranchCompanyRepository branchCompanyRepository) : base(branchCompanyRepository)
        {
            BranchCompanyRepository = branchCompanyRepository;
        }

        #endregion
    }
}