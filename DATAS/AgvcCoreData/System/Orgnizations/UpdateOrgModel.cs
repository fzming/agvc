namespace AgvcCoreData.System.Orgnizations
{
    /// <summary>
    /// 更新机构模型
    /// </summary>
    public class UpdateOrgModel
    {
        /// <summary>
        /// 待修改的机构ID
        /// </summary>
        public string OrgId { get; set; }
        /// <summary>
        /// 机构名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 机构缩写
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// 机构包括模块 至少一种
        /// </summary>
        public ModuleType[] Modules { get; set; }
    }
}