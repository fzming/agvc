namespace Cache.IRedis.Core
{
    /// <summary>
    ///     redis配置
    /// </summary>
    public class RedisConfig
    {
        public string RedisServer { get; set; }
        public string RedisPassword { get; set; }
        public string RedisKeyPrex { get; set; }
        public int RedisDefaultDatabase { get; set; }
    }
}