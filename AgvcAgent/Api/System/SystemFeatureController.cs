using System.Threading.Tasks;
using AgvcAgent.Api.Kernel;
using AgvcEntitys.System;
using AgvcService.System;
using AgvcService.System.Models;
using CoreData;
using Microsoft.AspNetCore.Mvc;

namespace AgvcAgent.Api.System
{
    /// <summary>
    ///     功能差异配置接口
    /// </summary>
    [Route("api/sys/feature")]
    public class
        SystemFeatureController : CrudApiController<SystemFeature, SystemFeatureModel, UpdateSystemFeatureModel>
    {
        /// <summary>
        ///     当删除系统定义项后 同时删除所有机构的相关定义项目
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected override async Task OnAfterDeleteAsync(string id)
        {
            await SystemFeatureService.RemoveOrganizationFeaturesAsync(id);
        }

        /// <summary>
        ///     分页获取所有数据
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public override Task<PageResult<SystemFeature>> PageQueryAsync(PageQuery page)
        {
            return SystemFeatureService.PageQueryAsync(page, p => !p.Hidden, o => o.CreatedOn);
        }

        public class QrgFeatureQuery : PageQuery
        {
            public string OrgId { get; set; }
        }

        #region IOC

        private ISystemFeatureService SystemFeatureService { get; }

        public SystemFeatureController(ISystemFeatureService systemFeatureService) : base(systemFeatureService)
        {
            SystemFeatureService = systemFeatureService;
        }

        #endregion

        #region 机构功能定义

        /// <summary>
        ///     机构功能定义列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("org-features")]
        public Task<PageResult<OrgFeatureDto>> QueryOrgFeaturesAsync([FromBody] QrgFeatureQuery query)
        {
            return SystemFeatureService.QueryOrganizationFeaturesAsync(query.OrgId, query);
        }

        /// <summary>
        ///     设置机构值定义
        /// </summary>
        /// <param name="featureValue">ID不用传</param>
        /// <returns></returns>
        [HttpPost]
        [Route("set-org-feature")]
        public Task<bool> QueryOrgFeaturesAsync([FromBody] OrganizationFeatureValue featureValue)
        {
            return SystemFeatureService.SetOrganizationFeatureValueAsync(OrgId, featureValue);
        }

        public class ResetFeatureModel
        {
            public string OrgId { get; set; }
            public string FeatureId { get; set; }
        }

        /// <summary>
        ///     恢复初始化机构值初始定义
        /// </summary>
        /// <param name="resetFeatureModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("reset-org-feature")]
        public Task<bool> RemoveOrgFeaturesAsync([FromBody] ResetFeatureModel resetFeatureModel)
        {
            return SystemFeatureService.ResetOrganizationFeaturesAsync(resetFeatureModel.OrgId,
                resetFeatureModel.FeatureId);
        }

        #endregion
    }
}