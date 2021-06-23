namespace CoreRepository
{
    internal class MongoConfig
    {
        public string MongoUrl { get; set; }
        /// <summary>
        /// 数据库名称：可选
        /// </summary>
        public string DatabaseName { get; set; }

    }
}