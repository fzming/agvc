using System.Threading.Tasks;

namespace AgvcService
{
    public class LoginService : ILoginService
    {
        public Task<bool> LoginAsync()
        {
            return Task.FromResult(true);
        }
    }
}