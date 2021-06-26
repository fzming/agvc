using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cache.IRedis.Core;
using Cache.IRedis.Interfaces;
using StackExchange.Redis;

namespace Cache.IRedis
{
    /// <summary>
    ///     Redis哈希缓存
    ///     Hash——字典
    ///     相当于一个key对于一个map，map中还有key-value
    /// </summary>
    public class RedisHashCache : RedisCaching, IRedisHashCache
    {
        private IRedisKeyCache RedisKeyCache { get; }

        #region 同步执行

        public RedisHashCache(IRedisConnectionMultiplexer redisConnectionMultiplexer, IRedisKeyCache redisKeyCache) :
            base(redisConnectionMultiplexer)
        {
            RedisKeyCache = redisKeyCache;
        }

        /// <summary>
        ///     是否被缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public bool HashExists(string key, string dataKey)
        {
            key = AddPrefixKey(key);
            return DoSync(db => db.HashExists(key, dataKey));
        }

        /// <summary>
        ///     存储数据到hash表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public bool HashSet<T>(string key, string dataKey, T val)
        {
            key = AddPrefixKey(key);
            return DoSync(db =>
            {
                var json = ConvertJson(val);
                return db.HashSet(key, dataKey, json);
            });
        }

        /// <summary>
        ///     从hash表中移除数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public bool HashDelete(string key, string dataKey)
        {
            key = AddPrefixKey(key);
            return DoSync(db => db.HashDelete(key, dataKey));
        }

        /// <summary>
        ///     移除hash中的多个值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public long HashRemove(string key, List<string> dataKey)
        {
            key = AddPrefixKey(key);
            return DoSync(db => db.HashDelete(key, dataKey.Select(p => (RedisValue) p).ToArray()));
        }

        /// <summary>
        ///     从hash表中获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public T HashGet<T>(string key, string dataKey)
        {
            key = AddPrefixKey(key);
            return DoSync(db =>
            {
                var val = db.HashGet(key, dataKey);
                return ConvertObj<T>(val);
            });
        }

        public Dictionary<string, T> HashAll<T>(string key)
        {
            key = AddPrefixKey(key);
            return DoSync(db =>
            {
                var hashEntries = db.HashGetAll(key);
                return hashEntries.ToDictionary(p => p.Name.ToString(), p => ConvertObj<T>(p.Value));
            });
        }

        public object HashGet(string key, string dataKey, Type type)
        {
            key = AddPrefixKey(key);
            return DoSync(db =>
            {
                var val = db.HashGet(key, dataKey);
                return ConvertObj(val, type);
            });
        }

        /// <summary>
        ///     为数字增长val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public double HashIncrement(string key, string dataKey, double val = 1)
        {
            key = AddPrefixKey(key);
            return DoSync(db => db.HashIncrement(key, dataKey, val));
        }

        /// <summary>
        ///     为数字减少val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public double HashDecrement(string key, string dataKey, double val = 1)
        {
            key = AddPrefixKey(key);
            return DoSync(db => db.HashDecrement(key, dataKey, val));
        }

        /// <summary>
        ///     查看所有key值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<T> HashKeys<T>(string key)
        {
            key = AddPrefixKey(key);
            return DoSync(db =>
            {
                var val = db.HashKeys(key);
                return ConvertList<T>(val);
            });
        }

        #endregion

        #region 异步执行

        /// <summary>
        ///     异步是否被缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public Task<bool> HashExistsAsync(string key, string dataKey)
        {
            key = AddPrefixKey(key);
            return DoAsync(db => db.HashExistsAsync(key, dataKey));
        }

        /// <summary>
        ///     异步存储数据到hash表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public Task<bool> HashSetAsync<T>(string key, string dataKey, T val)
        {
            key = AddPrefixKey(key);
            return DoAsync(db =>
            {
                var json = ConvertJson(val);
                return db.HashSetAsync(key, dataKey, json);
            });
        }

        /// <summary>
        ///     异步从hash表中移除数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public Task<bool> HashDeleteAsync(string key, string dataKey)
        {
            key = AddPrefixKey(key);
            return DoAsync(db => db.HashDeleteAsync(key, dataKey));
        }

        /// <summary>
        ///     删除整个hash
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Task<bool> DeleteAsync(string key)
        {
            key = AddPrefixKey(key);
            return DoAsync(db => RedisKeyCache.KeyDeleteAsync(key));
        }

        /// <summary>
        ///     异步移除hash中的多个值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public Task<long> HashRemoveAsync(string key, List<string> dataKey)
        {
            key = AddPrefixKey(key);
            return DoAsync(db => db.HashDeleteAsync(key, dataKey.Select(p => (RedisValue) p).ToArray()));
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
            key = AddPrefixKey(key);
            var val = await DoAsync(db => db.HashGetAsync(key, dataKey));
            return val.IsNull ? default : ConvertObj<T>(val);
        }

        public async Task<object> HashGetAsync(string key, string dataKey, Type type)
        {
            key = AddPrefixKey(key);
            var val = await DoAsync(db => db.HashGetAsync(key, dataKey));
            return ConvertObj(val, type);
        }

        public async Task<Dictionary<string, T>> HashAllAsync<T>(string key)
        {
            key = AddPrefixKey(key);
            var hashEntries = await DoAsync(db => db.HashGetAllAsync(key));
            return hashEntries.ToDictionary(p => p.Name.ToString(), p => ConvertObj<T>(p.Value));
        }

        /// <summary>
        ///     为数字增长val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public Task<double> HashIncrementAsync(string key, string dataKey, double val = 1)
        {
            key = AddPrefixKey(key);
            return DoAsync(db => db.HashIncrementAsync(key, dataKey, val));
        }

        /// <summary>
        ///     为数字减少val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public Task<double> HashDecrementAsync(string key, string dataKey, double val = 1)
        {
            key = AddPrefixKey(key);
            return DoAsync(db => db.HashDecrementAsync(key, dataKey, val));
        }

        /// <summary>
        ///     查看所有key值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<List<T>> HashKeysAsync<T>(string key)
        {
            key = AddPrefixKey(key);
            var val = await DoAsync(db => db.HashKeysAsync(key));
            return ConvertList<T>(val);
        }

        /// <summary>
        ///     获取或创建缓存（使用自定义哈希键名称）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataKey"></param>
        /// <param name="createAsync"></param>
        /// <returns></returns>
        public Task<T> GetOrCreateHashCacheAsync<T>(string dataKey, Func<Task<T>> createAsync)
        {
            var hashKey = typeof(T).Name;
            return GetOrCreateHashCacheAsync(hashKey, dataKey, createAsync);
        }

        /// <summary>
        ///     获取或创建缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hashKey">哈希键</param>
        /// <param name="dataKey">数据项键</param>
        /// <param name="createAsync">如果不存在则创建</param>
        /// <param name="expires">为整个hash创建过期时间</param>
        /// <returns></returns>
        public async Task<T> GetOrCreateHashCacheAsync<T>(string hashKey, string dataKey, Func<Task<T>> createAsync,
            TimeSpan? expires = null)
        {
            #region 检查Redis缓存

            var cacheExsits = await HashExistsAsync(hashKey, dataKey);
            if (cacheExsits) return await HashGetAsync<T>(hashKey, dataKey);

            #endregion

            //创建缓存
            var cache = await createAsync();
            if (cache == null) return default;
            await HashSetAsync(hashKey, dataKey, cache);
            if (expires.HasValue) await RedisKeyCache.KeyExpireAsync(hashKey, expires);
            return cache;
        }

        #endregion
    }
}