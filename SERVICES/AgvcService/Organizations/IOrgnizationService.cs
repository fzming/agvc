using System.Collections.Generic;
using System.Threading.Tasks;
using AgvcCoreData.System;
using AgvcEntitys.Organization;
using AgvcService.Organizations.Models;
using CoreService.Interfaces;

namespace AgvcService.Organizations
{
    /// <summary>
    /// 机构服务
    /// </summary>
    public interface IOrgnizationService:IService
    {
        /// <summary>
        /// 查询所有机构
        /// </summary>
        /// <param name="paOrgId">父机构ID</param>
        /// <returns></returns>
        Task<IEnumerable<Organization>> QueryOrgsAsync(string paOrgId);
        /// <summary>
        /// 创建机构
        /// </summary>
        /// <param name="model">创建机构模型</param>
        /// <param name="paOrgId">父机构ID</param>
        /// <returns></returns>
        Task<Organization> CreateOrgAsync(CreateOrgModel model, string paOrgId);
        /// <summary>
        /// 更新机构
        /// </summary>
        /// <param name="model">更新机构模型</param>
        /// <returns></returns>
        Task<bool> UpdateOrgAsync(UpdateOrgModel model);
        /// <summary>
        /// 删除机构
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        Task<bool> DeleteOrgAsync(string orgId);

        /// <summary>
        /// 获取机构
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        Task<Organization> GetOrgAsync(string orgId);
        /// <summary>
        /// 获取所有机构ID列表
        /// </summary>
        /// <returns></returns>
        Task<List<string>> GroupAllIdsAsync();

        Task UpdateModulesAsync(ModuleType[] modules);
        Task<Organization> GetOrgByDomainAsync(string domain);
    }
}