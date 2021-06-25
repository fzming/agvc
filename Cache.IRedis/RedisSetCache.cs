using System.Linq;
using System.Threading.Tasks;
using Cache.IRedis.Core;
using Cache.IRedis.Interfaces;
using StackExchange.Redis;

namespace Cache.IRedis
{
    /// <summary>
    /// Redis Set缓存
    /// Set——集合
    /// </summary>
    public class RedisSetCache : RedisCaching, IRedisSetCache
    {
        #region 同步执行

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public bool SetAdd(string key, string val)
        {
            key = AddPrefixKey(key);
            return DoSync(db => db.SetAdd(key, val));
        }

        public bool SetAdd(string key, string[] values)
        {
            key = AddPrefixKey(key);
            return DoSync(db => db.SetAdd(key, values.Select(p => (RedisValue)p).ToArray()) > 0);
        }

        /// <summary>
        /// 查询key值中所有value值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string[] SetMembers(string key)
        {
            key = AddPrefixKey(key);
            return DoSync(db => db.SetMembers(key).Select(p => p.ToString()).ToArray());
        }

        /// <summary>
        /// 获取长度
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long SetLength(string key)
        {
            key = AddPrefixKey(key);
            return DoSync(db => db.SetLength(key));
        }

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public bool SetContains(string key, string val)
        {
            key = AddPrefixKey(key);
            return DoSync(db => db.SetContains(key, val));
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public bool SetRemove(string key, string val)
        {
            key = AddPrefixKey(key);
            return DoSync(db => db.SetRemove(key, val));
        }
        #endregion

        #region 异步执行

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public Task<bool> SetAddAsync(string key, string val)
        {
            key = AddPrefixKey(key);
            return DoAsync(db => db.SetAddAsync(key, val));
        }

        public Task<bool> SetAddAsync(string key, string[] values)
        {
            key = AddPrefixKey(key);
            return DoAsync(async db => await db.SetAddAsync(key, values.Select(p => (RedisValue)p).ToArray()) > 0);
        }

        /// <summary>
        /// 查询key值中所有value值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Task<string[]> SetMembersAsync(string key)
        {
            key = AddPrefixKey(key);
            return DoAsync(async db => (await db.SetMembersAsync(key)).Select(p => p.ToString()).ToArray());
        }

        /// <summary>
        /// 获取长度
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Task<long> SetLengthAsync(string key)
        {
            key = AddPrefixKey(key);
            return DoAsync(db => db.SetLengthAsync(key));
        }

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public Task<bool> SetContainsAsync(string key, string val)
        {
            key = AddPrefixKey(key);
            return DoAsync(db => db.SetContainsAsync(key, val));
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public Task<bool> SetRemoveAsync(string key, string val)
        {
            key = AddPrefixKey(key);

            return DoAsync(db => db.SetRemoveAsync(key, val));
        }
        #endregion

        public RedisSetCache(IRedisConnectionMultiplexer redisRedisConnectionMultiplexer) : base(redisRedisConnectionMultiplexer)
        {
        }
    }
}