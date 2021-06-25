using System.Collections.Generic;
using System.Threading.Tasks;
using AgvcCoreData.System;
using AgvcEntitys.System;
using CoreRepository;

namespace AgvcRepository.System.Interfaces
{
    public interface IMenuRepository:IRepository<Menu>
    {
        Task<bool> BatchUpdateMenusMetaAsync(List<MenuRoleUpdateSet> roleUpdateSets);
        Task<IEnumerable<Menu>> QueryModuleMenusAsync(ModuleType[] modules);
    }
}