using AgvcCoreData.System;
using AgvcCoreData.System.Orgnizations;
using CoreData.Core;

namespace AgvcEntitys.Organization
{
    /// <summary>
    /// 系统机构信息表
    /// </summary>
    public class Organization :MongoEntity
    {
        /// <summary>
        /// 机构名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 机构短称
        /// </summary>
        public string ShortName { get; set; }
        /// <summary>
        /// 机构类型
        /// </summary>
        public OrganizationType PrimaryType { get; set; }
     
        /// <summary>
        /// 父机构ID
        /// </summary>
        public string ParentOrgId { get; set; }
        /// <summary>
        /// 机构缩写（4位字母）
        /// </summary>
        public string Prefix { get; set; }
        /// <summary>
        /// 机构包括模块 至少一种
        /// </summary>
        public ModuleType[] Modules { get; set; }

    }
}