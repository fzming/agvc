using System.Threading.Tasks;
using AgvcEntitys.System;
using CoreRepository;

namespace AgvcRepository.System.Interfaces
{
    public interface IOrganizationFeatureValueRepository : IRepository<OrganizationFeatureValue>
    {
        Task<bool> SetFeatureValueAsync(OrganizationFeatureValue ofv);
    }
}