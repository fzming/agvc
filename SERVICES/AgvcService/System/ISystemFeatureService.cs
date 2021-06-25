using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AgvcEntitys.System;
using AgvcService.System.Models;
using CoreData;
using CoreService.Interfaces;

namespace AgvcService.System
{
    public interface ISystemFeatureService : ICrudService<SystemFeature>
    {
        Task<bool> SetOrganizationFeatureValueAsync(string orgId, OrganizationFeatureValue value);
        Task<PageResult<OrgFeatureDto>> QueryOrganizationFeaturesAsync(string orgId, PageQuery query, Expression<Func<SystemFeature, bool>> filter = null);
        Task<Dictionary<string, string>> GetOrganizationFeaturesAsync(string orgId);
        Task<Dictionary<string, string>> GetOrganizationFeaturesNoSafetyAsync(string orgId);
        Task<string> GetFeatureValueAsync(string orgId, string key);
        Task<bool> ResetOrganizationFeaturesAsync(string orgId, string featureId);
        Task<bool> RemoveOrganizationFeaturesAsync(string featureId);
        Task<SystemFeature> GetFeatureByKeyAsync(string key);
        Task<bool> SetFeatureAsync(SystemFeatureModel featureModel);
    }
}