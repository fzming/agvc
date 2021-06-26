using System.Threading.Tasks;
using AgvcCoreData.System;
using AgvcEntitys.System;
using CoreData;
using CoreRepository;

namespace AgvcRepository.System.Interfaces
{
    public interface ISystemUserRepository : IRepository<SystemUser>, IDynamicRepository
    {
        Task<PageResult<SystemUser>> AdvQuerySystemUsersAsync(SystemUserPageQuery userPageQuery);
    }
}