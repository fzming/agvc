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
    ///     Redis String缓存
    /// </summary>
    public class RedisStringCache : RedisCaching, IRedisStringCache
    {
        public RedisStringCache(IRedisConnectionMultiplexer redisRedisConnectionMultiplexer) : base(
            redisRedisConnectionMultiplexer)
        {
        }


        #region 同步执行

        /// <summary>
        ///     单个保存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val">值</param>
        /// <param name="exp">过期时间</param>
        /// <returns></returns>
        public bool StringSet(string key, string val, TimeSpan? exp = default)
        {
            key = AddPrefixKey(key);
            return DoSync(db => db.StringSet(key, val, exp));
        }

        /// <summary>
        ///     保存多个key value
        /// </summary>
        /// <param name="keyValues">键值对</param>
        /// <returns></returns>
        public bool StringSet<T>(Dictionary<string, T> keyValues)
        {
            var keyValuePairs = keyValues
                .Select(k => new KeyValuePair<RedisKey, RedisValue>(AddPrefixKey(k.Key), ConvertJson(k.Value)))
                .ToArray();
            return DoSync(db => db.StringSet(keyValuePairs));
        }

        /// <summary>
        ///     保存一个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        public bool StringSet<T>(string key, T obj, TimeSpan? exp = default)
        {
            key = AddPrefixKey(key);
            var json = ConvertJson(obj);
            return DoSync(db => db.StringSet(key, json, exp));
        }

        /// <summary>
        ///     获取单个
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string StringGet(string key)
        {
            key = AddPrefixKey(key);
            return DoSync(db => db.StringGet(key));
        }

        /// <summary>
        ///     获取单个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T StringGet<T>(string key)
        {
            key = AddPrefixKey(key);
            var val = DoSync(db => db.StringGet(key));
            return ConvertObj<T>(val);
        }

        /// <summary>
        ///     为数字增长val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val">可以为负数</param>
        /// <returns>增长后的值</returns>
        public double StringIncrement(string key, double val = 1)
        {
            key = AddPrefixKey(key);
            return DoSync(db => db.StringIncrement(key, val));
        }

        /// <summary>
        ///     为数字减少val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val">可以为负数</param>
        /// <returns>增长后的值</returns>
        public double StringDecrement(string key, double val = 1)
        {
            key = AddPrefixKey(key);
            return DoSync(db => db.StringDecrement(key, val));
        }

        #endregion

        #region 异步执行

        /// <summary>
        ///     异步保存单个
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        public Task<bool> StringSetAsync(string key, string val, TimeSpan? exp = default)
        {
            key = AddPrefixKey(key);
            return DoAsync(db => db.StringSetAsync(key, val, exp));
        }

        /// <summary>
        ///     异步保存多个key value
        /// </summary>
        /// <param name="keyValues">键值对</param>
        /// <returns></returns>
        public Task<bool> StringSetAsync<T>(Dictionary<string, T> keyValues)
        {
            var keyValuePairs = keyValues
                .Select(k => new KeyValuePair<RedisKey, RedisValue>(AddPrefixKey(k.Key), ConvertJson(k.Value)))
                .ToArray();
            return DoAsync(db => db.StringSetAsync(keyValuePairs));
        }

        /// <summary>
        ///     异步保存一个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        public Task<bool> StringSetAsync<T>(string key, T obj, TimeSpan? exp = default)
        {
            key = AddPrefixKey(key);
            var json = ConvertJson(obj);
            return DoAsync(db => db.StringSetAsync(key, json, exp));
        }

        /// <summary>
        ///     异步获取单个
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<string> StringGetAsync(string key)
        {
            key = AddPrefixKey(key);
            return await DoAsync(db => db.StringGetAsync(key));
        }

        /// <summary>
        ///     异步获取单个
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T> StringGetAsync<T>(string key)
        {
            key = AddPrefixKey(key);
            var val = await DoAsync(db => db.StringGetAsync(key));
            return ConvertObj<T>(val);
        }

        /// <summary>
        ///     异步为数字增长val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val">可以为负数</param>
        /// <returns>增长后的值</returns>
        public Task<double> StringIncrementAsync(string key, double val = 1)
        {
            key = AddPrefixKey(key);
            return DoAsync(db => db.StringIncrementAsync(key, val));
        }

        /// <summary>
        ///     为数字减少val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val">可以为负数</param>
        /// <returns>增长后的值</returns>
        public Task<double> StringDecrementAsync(string key, double val = 1)
        {
            key = AddPrefixKey(key);
            return DoAsync(db => db.StringDecrementAsync(key, val));
        }

        #endregion
    }
}