using CoreData.Core;

namespace AgvcEntitys.System
{
    /// <summary>
    ///     系统配置
    /// </summary>
    public class SystemFeature : MongoEntity
    {
        /// <summary>
        ///     功能名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     关键字
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        ///     功能分组名称
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        ///     功能描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     定义值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        ///     是否进行隐藏，不在列表中显示
        /// </summary>
        public bool Hidden { get; set; }

        /// <summary>
        ///     安全选项：标识此配置不会在客户端进行序列化输出
        /// </summary>
        public bool Safety { get; set; }
    }
}