using System.Threading.Tasks;

namespace AgvcService
{
    public class LoginService : ILoginService
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public LoginService()
        {
        }
        public Task<bool> LoginAsync()
        {
            return Task.FromResult(true);
        }
    }
}