using System.Threading.Tasks;
using AgvcEntitys.System;
using AgvcRepository.System.Interfaces;
using CoreRepository;

namespace AgvcRepository.System
{
    public class OrganizationFeatureValueRepository : MongoRepository<OrganizationFeatureValue>,
        IOrganizationFeatureValueRepository
    {
        public OrganizationFeatureValueRepository(IMongoUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<bool> SetFeatureValueAsync(OrganizationFeatureValue ofv)
        {
            var exValue = await FindValueAsync(ofv.OrgId, ofv.SysFeatureId);
            if (exValue != null) return await UpdateAsync(exValue.Id, p => p.Value, ofv.Value);

            await InsertAsync(ofv);
            return true;
        }

        private Task<OrganizationFeatureValue> FindValueAsync(string orgId, string sysFeatureId)
        {
            return FirstAsync(p => p.OrgId == orgId && p.SysFeatureId == sysFeatureId);
        }
    }
}