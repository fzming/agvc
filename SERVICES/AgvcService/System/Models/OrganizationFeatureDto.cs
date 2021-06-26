namespace AgvcService.System.Models
{
    public class OrganizationFeatureDto
    {
        /// <summary>
        ///     功能ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///     功能名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     关键字
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        ///     功能描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     定义值
        /// </summary>
        public string Value { get; set; }
    }
}