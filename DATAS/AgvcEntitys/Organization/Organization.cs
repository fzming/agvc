using AgvcCoreData.System;
using CoreData.Core;

namespace AgvcEntitys.Organization
{
    /// <summary>
    ///     机构的主性质
    /// </summary>
    public enum OrganizationType
    {
        /// <summary>
        ///     系统机构
        /// </summary>
        System = 0,

        /// <summary>
        ///     货代
        /// </summary>
        Agency = 10,

        /// <summary>
        ///     堆场
        /// </summary>
        Stump = 20,

        /// <summary>
        ///     车队
        /// </summary>
        Truck = 30,

        /// <summary>
        ///     船公司
        /// </summary>
        Ship = 40,

        /// <summary>
        ///     铁路代理
        /// </summary>
        Train = 50
    }

    /// <summary>
    ///     系统机构信息表
    /// </summary>
    public class Organization : MongoEntity
    {
        /// <summary>
        ///     机构名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     机构短称
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        ///     机构类型
        /// </summary>
        public OrganizationType PrimaryType { get; set; }

        /// <summary>
        ///     父机构ID
        /// </summary>
        public string ParentOrgId { get; set; }

        /// <summary>
        ///     机构缩写（4位字母）
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        ///     机构包括模块 至少一种
        /// </summary>
        public ModuleType[] Modules { get; set; }
    }
}