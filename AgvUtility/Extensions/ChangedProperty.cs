namespace Utility.Extensions
{
    public class ChangedProperty
    {

        /// <summary>
        /// 路径
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 跟踪字段
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// 原值
        /// </summary>
        public string OriginalValue { get; set; }

        /// <summary>
        /// 新值
        /// </summary>
        public string NewValue { get; set; }
    }
}