using AgvcEntitys.Organization;
using AgvcRepository.Orgnizations.Interfaces;
using CoreRepository;

namespace AgvcRepository.Orgnizations
{

    public class DepartmentRepository : MongoRepository<Department>, IDepartmentRepository
    {
        protected DepartmentRepository(IMongoUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}