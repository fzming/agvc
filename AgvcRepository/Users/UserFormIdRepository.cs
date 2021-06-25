using AgvcEntitys.Users;
using AgvcRepository.Users.Interfaces;
using CoreRepository;

namespace AgvcRepository.Users
{
    [Export(typeof(IUserFormIdRepository))]
    internal class UserFormIdRepository : MongoRepository<UserFormId>, IUserFormIdRepository
    {
        
    }
}