using System.Threading.Tasks;

using CoreService.Interfaces;

using Utility;

namespace AgvcService
{
    public interface IUserService : IService, ISingletonDependency
    {
        Task<bool> LoginAsync();
    }
}