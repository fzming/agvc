using System.Linq;
using Cache.IRedis.Core;
using Cache.IRedis.Interfaces;

namespace Cache.IRedis
{
    /// <summary>
    /// Redis 服务器操作
    /// </summary>
    public class RedisServerCache:RedisCaching, IRedisServerCache
    {
        /// <summary>
        /// 数据库所有键值扫描
        /// 查询数据库里面存有哪些key
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public string[] KeyScan(string pattern = "*", int pageSize = 10)
        {

            return DoServerSync(
                (server, database) =>
                    server.Keys(database, pattern: pattern, pageSize: pageSize).Select(p => (string)p).ToArray());
        }
        /// <summary>
        /// 删除数据库所有键值
        /// </summary>
        public bool FlushDatabase()
        {
            return DoServerSync((server, database) => {
                server.FlushDatabase();
                return true;
            });
        }

        public RedisServerCache(IRedisConnectionMultiplexer redisRedisConnectionMultiplexer) : base(redisRedisConnectionMultiplexer)
        {

        }
    }
}