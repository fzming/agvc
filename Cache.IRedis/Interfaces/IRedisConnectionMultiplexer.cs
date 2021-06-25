using Cache.IRedis.Core;
using StackExchange.Redis;
using Utility;

namespace Cache.IRedis.Interfaces
{
    public interface IRedisConnectionMultiplexer : ISingletonDependency
    {
        public ConnectionMultiplexer ConnectionMultiplexer { get; set; }
        RedisConfig RedisConfig { get; set; }
        IServer Server { get; }
    
    }
}