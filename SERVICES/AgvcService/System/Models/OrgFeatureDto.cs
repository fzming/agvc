using AgvcEntitys.System;

namespace AgvcService.System.Models
{
    public class OrgFeatureDto : SystemFeature
    {
        /// <summary>
        ///     系统默认值
        /// </summary>
        public string DefaultValue { get; set; }
    }
}