using AgvcEntitys.Organization;
using AgvcRepository.Orgnizations.Interfaces;
using CoreRepository;

namespace AgvcRepository.Orgnizations
{
    public class BranchCompanyRepository : MongoRepository<BranchCompany>, IBranchCompanyRepository
    {
        public BranchCompanyRepository(IMongoUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}