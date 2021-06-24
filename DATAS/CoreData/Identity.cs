namespace CoreData
{
    /// <summary>
    /// 带名字和ID的识别基类
    /// </summary>
    public class Identity
    {
        /// <summary>
        ///   初始化 <see cref="T:System.Object" /> 类的新实例。
        /// </summary>
        public Identity(string id, string name)
        {
            Id = id;
            Name = name;
        }

        /// <summary>
        ///   初始化 <see cref="T:System.Object" /> 类的新实例。
        /// </summary>
        public Identity()
        {
        }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// ID
        /// </summary>
        public string Id { get; set; }
    }
}