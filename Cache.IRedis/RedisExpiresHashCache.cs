using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cache.IRedis.Core;
using Cache.IRedis.Interfaces;
using StackExchange.Redis;
using Utility.Extensions;

namespace Cache.IRedis
{
    /// <summary>
    ///     带过期Redis哈希缓存
    ///     Hash——字典
    ///     相当于一个key对于一个map，map中还有key-value
    /// </summary>
    public class RedisExpiresHashCache : RedisCaching, IRedisExpiresHashCache
    {
        public RedisExpiresHashCache(IRedisConnectionMultiplexer redisConnectionMultiplexer,
            IRedisHashCache redisHashCache) : base(redisConnectionMultiplexer)
        {
            RedisHashCache = redisHashCache;
        }

        private IRedisHashCache RedisHashCache { get; }

        public string GetExpireKey(string key)
        {
            return $"{key}-expires-set";
        }

        /// <summary>
        ///     异步是否被缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public async Task<bool> HashExistsAsync(string key, string dataKey)
        {
            var expKey = GetExpireKey(key);
            var expHash = await RedisHashCache.HashGetAsync<DateTime?>(expKey, dataKey);
            if (expHash.HasValue)
            {
                var expires = expHash.Value;
                if (expires.IsExpires())
                {
                    await HashDeleteAsync(key, dataKey);
                    return false;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        ///     异步存储数据到hash表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="val"></param>
        /// <param name="expires"></param>
        /// <returns></returns>
        public async Task<bool> HashSetAsync<T>(string key, string dataKey, T val, TimeSpan expires)
        {
            var json = ConvertJson(val);
            await HashDeleteAsync(key, dataKey);

            var taskSet = RedisHashCache.HashSetAsync(key, dataKey, json);
            var taskSetExpires = RedisHashCache.HashSetAsync(GetExpireKey(key), dataKey, DateTime.Now.Add(expires));
            var result = await Task.WhenAll(taskSet, taskSetExpires);
            return result.All(p => p);
        }

        /// <summary>
        ///     异步从hash表中移除数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public async Task<bool> HashDeleteAsync(string key, string dataKey)
        {
            var taskSet = RedisHashCache.HashDeleteAsync(key, dataKey);
            var taskSetExpires = RedisHashCache.HashDeleteAsync(GetExpireKey(key), dataKey);
            var result = await Task.WhenAll(taskSet, taskSetExpires);
            return result.All(p => p);
        }

        /// <summary>
        ///     异步移除hash中的多个值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public async Task<long> HashRemoveAsync(string key, List<string> dataKey)
        {
            var dataKeys = dataKey.Select(p => (RedisValue) p).ToArray();

            var taskSet = DoAsync(db => db.HashDeleteAsync(key, dataKeys));
            var taskSetExpires = DoAsync(db => db.HashDeleteAsync(GetExpireKey(key), dataKeys));
            var result = await Task.WhenAll(taskSet, taskSetExpires);
            return result.Sum();
        }

        /// <summary>
        ///     从hash表中获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public async Task<T> HashGetAsync<T>(string key, string dataKey)
        {
            var hasKey = await HashExistsAsync(key, dataKey);
            if (!hasKey) return default;

            return await RedisHashCache.HashGetAsync<T>(key, dataKey);
        }

        public async Task<object> HashGetAsync(string key, string dataKey, Type type)
        {
            var hasKey = await HashExistsAsync(key, dataKey);
            if (!hasKey) return default;

            return await RedisHashCache.HashGetAsync(key, dataKey, type);
        }
    }
}