using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AgvcEntitys.System;
using AgvcRepository.System.Interfaces;
using AgvcService.System.Models;
using CoreData;
using CoreService;
using Utility.Extensions;

namespace AgvcService.System
{
    public class SystemFeatureService : AbstractCrudService<SystemFeature>, ISystemFeatureService
    {
        #region IOC

        private ISystemFeatureRepository SystemFeatureRepository { get; }
        private IOrganizationFeatureValueRepository OrganizationFeatureValueRepository { get; }

        public SystemFeatureService(ISystemFeatureRepository systemFeatureRepository,
            IOrganizationFeatureValueRepository organizationFeatureValueRepository) :
            base(systemFeatureRepository)
        {
            SystemFeatureRepository = systemFeatureRepository;
            OrganizationFeatureValueRepository = organizationFeatureValueRepository;
        }

        #endregion

        public Task<bool> SetOrganizationFeatureValueAsync(string orgId, OrganizationFeatureValue value)
        {
            if(value.OrgId.IsNullOrEmpty()) value.OrgId = orgId;
            return OrganizationFeatureValueRepository.SetFeatureValueAsync(value);
        }

        public async Task<PageResult<OrgFeatureDto>> QueryOrganizationFeaturesAsync(string orgId, PageQuery query, Expression<Func<SystemFeature, bool>> filter = null)
        {
            var ps = await PageQueryAsync(query, filter, order => order.CreatedOn, false);
            var sysFeatures = ps.Datas.ToList();

            IEnumerable<OrganizationFeatureValue> orgFeatureValues;
            if (query.PageSize > 0 && query.PageIndex > 0)
            {
                var featureIds = sysFeatures.Select(p => p.Id);
                orgFeatureValues =
                  await OrganizationFeatureValueRepository.FindAsync(p => p.OrgId == orgId && featureIds.Contains(p.SysFeatureId));
            }
            else
            {
                orgFeatureValues = await OrganizationFeatureValueRepository.FindAsync(p => p.OrgId == orgId);
            }

            var datas = sysFeatures.Select(feature =>
             {
                 var dto = feature.MapTo(new OrgFeatureDto());
                 dto.DefaultValue = feature.Value;
                 dto.Value = feature.Value;
                 var orgFeature = orgFeatureValues.FirstOrDefault(p => p.SysFeatureId == feature.Id);
                 if (orgFeature == null) return dto;
                 dto.Value = orgFeature.Value;
                 return dto;
             });


            var dtoRs = new PageResult<OrgFeatureDto> { Datas = datas, PageCount = ps.PageCount, Total = ps.Total };
            return dtoRs;

        }

        public async Task<Dictionary<string, string>> GetOrganizationFeaturesAsync(string orgId)
        {
            var features = await QueryOrganizationFeaturesAsync(orgId, new PageQuery());
            return features.Datas.ToDictionary(key => key.Key, value => value.Value);
        }       
        public async Task<Dictionary<string, string>> GetOrganizationFeaturesNoSafetyAsync(string orgId)
        {
            var features = await QueryOrganizationFeaturesAsync(orgId, new PageQuery());
            return features.Datas.Where(p=>p.Safety==false&&p.Hidden==false).ToDictionary(key => key.Key, value => value.Value);
        }

        public async Task<string> GetFeatureValueAsync(string orgId, string key)
        {
            // var features = await QueryOrgFeaturesAsync(orgId, new PageQuery(), query => query.Where(p => p.Key == key));
            // var datas = features.Datas;
            // return datas.FirstOrDefault();
            var features = await GetOrganizationFeaturesAsync(orgId);
            if (features.TryGetValue(key, out var value))
            {
                return value;
            }

            return string.Empty;
        }

        public Task<bool> ResetOrganizationFeaturesAsync(string orgId, string featureId)
        {
            return OrganizationFeatureValueRepository.DeleteAsync(p => p.OrgId == orgId && p.SysFeatureId == featureId);
        }

        public Task<bool> RemoveOrganizationFeaturesAsync(string featureId)
        {
            return OrganizationFeatureValueRepository.DeleteAsync(p => p.SysFeatureId == featureId);
        }

        public Task<SystemFeature> GetFeatureByKeyAsync(string key)
        {
            return SystemFeatureRepository.FirstAsync(p => p.Key == key);
        }

        public async Task<bool> SetFeatureAsync(SystemFeatureModel featureModel)
        {
            var feature = await GetFeatureByKeyAsync(featureModel.Key);
            if (feature == null)
            {
                await base.CreateAsync(null, featureModel);

            }
            else
            {
                await base.UpdateAsync(featureModel.MapTo(new UpdateSystemFeatureModel
                {
                    Id = feature.Id
                }));
            }

            return true;
        }
    }
}