using AgvcEntitys.Organization;
using AgvcRepository.Orgnizations.Interfaces;
using CoreRepository;

namespace AgvcRepository.Orgnizations
{

    public class DepartmentRepository : MongoRepository<Department>, IDepartmentRepository
    {
        public DepartmentRepository(IMongoUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}