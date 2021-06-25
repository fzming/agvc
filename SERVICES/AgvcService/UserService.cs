using System.Threading.Tasks;

namespace AgvcService
{
    public class UserService : IUserService
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public UserService()
        {
        }
        public Task<bool> LoginAsync()
        {
            return Task.FromResult(true);
        }
    }
}