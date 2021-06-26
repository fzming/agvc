using System.Collections.Generic;
using System.Threading.Tasks;
using AgvcCoreData.System;
using AgvcEntitys.Organization;
using AgvcRepository.Orgnizations.Interfaces;
using CoreRepository;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace AgvcRepository.Orgnizations
{
    public class OrganizationRepository : MongoRepository<Organization>, IOrganizationRepository
    {
        public OrganizationRepository(IMongoUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public Task<List<string>> GroupAllIdsAsync()
        {
            return Collection.AsQueryable().GroupBy(p => p.Id).Select(p => p.Key).ToListAsync();
        }

        public Task UpdateModulesAsync(ModuleType[] modules)
        {
            var update = Updater.AddToSetEach(p => p.Modules, modules);
            return Collection.UpdateManyAsync(FilterDefinition<Organization>.Empty, update);
        }
    }
}