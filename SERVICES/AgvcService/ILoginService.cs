using System.Threading.Tasks;

using CoreService.Interfaces;

using Utility;

namespace AgvcService
{
    public interface ILoginService : IService, ISingletonDependency
    {
        Task<bool> LoginAsync();
    }
}