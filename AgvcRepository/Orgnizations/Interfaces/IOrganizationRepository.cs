using System.Collections.Generic;
using System.Threading.Tasks;
using AgvcCoreData.System;
using AgvcEntitys.Organization;
using CoreRepository;

namespace AgvcRepository.Orgnizations.Interfaces
{
    /// <summary>
    ///     机构仓储接口
    /// </summary>
    public interface IOrganizationRepository : IRepository<Organization>
    {
        Task<List<string>> GroupAllIdsAsync();
        Task UpdateModulesAsync(ModuleType[] modules);
    }
}