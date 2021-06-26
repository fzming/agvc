namespace AgvcCoreData.System
{
    public class MenuMeta
    {
        /// <summary>
        ///     图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        ///     菜单窗口标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     角色表 由权限表动态算出
        /// </summary>
        public string[] Roles { get; set; }

        /// <summary>
        ///     如果设置为true，则不会被 <keep-alive /> 缓存(默认 false)
        /// </summary>
        public bool NoCache { get; set; }

        /// <summary>
        ///     如果设置为false，则不会在breadcrumb面包屑中显示
        /// </summary>
        public bool BreadCrumb { get; set; }
    }
}