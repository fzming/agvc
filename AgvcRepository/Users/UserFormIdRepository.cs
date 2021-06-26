using AgvcEntitys.Users;
using AgvcRepository.Users.Interfaces;
using CoreRepository;

namespace AgvcRepository.Users
{
    public class UserFormIdRepository : MongoRepository<UserFormId>, IUserFormIdRepository
    {
        public UserFormIdRepository(IMongoUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}