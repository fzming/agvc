using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cache.IRedis.Core;
using Cache.IRedis.Interfaces;

namespace Cache.IRedis
{
    /// <summary>
    ///     Key操作服务
    ///     操作所有Redis存储类型的数据，可以删除和设置查询过期时间等操作
    /// </summary>
    public class RedisKeyCache : RedisCaching, IRedisKeyCache
    {
        public RedisKeyCache(IRedisConnectionMultiplexer redisRedisConnectionMultiplexer) : base(
            redisRedisConnectionMultiplexer)
        {
        }

        #region 同步执行

        /// <summary>
        ///     删除单个Key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool KeyDelete(string key)
        {
            key = AddPrefixKey(key);
            return DoSync(db => db.KeyDelete(key));
        }

        /// <summary>
        ///     删除多个Key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long KeyDelete(IEnumerable<string> key)
        {
            var keys = key.Select(AddPrefixKey).ToList();
            return DoSync(db => db.KeyDelete(ConvertRedisKeys(keys)));
        }

        /// <summary>
        ///     重命名Key
        /// </summary>
        /// <param name="key">old key name</param>
        /// <param name="newKey">new key name</param>
        /// <returns></returns>
        public bool KeyRename(string key, string newKey)
        {
            key = AddPrefixKey(key);
            return DoSync(db => db.KeyRename(key, newKey));
        }

        /// <summary>
        ///     设置Key的时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        public bool KeyExpire(string key, TimeSpan? exp = default)
        {
            key = AddPrefixKey(key);
            return DoSync(db => db.KeyExpire(key, exp));
        }

        /// <inheritdoc />
        public TimeSpan? KeyTimeToLive(string key)
        {
            key = AddPrefixKey(key);
            return DoSync(db => db.KeyTimeToLive(key));
        }

        /// <inheritdoc />
        public bool KeyPersist(string key)
        {
            key = AddPrefixKey(key);
            return DoSync(db => db.KeyPersist(key));
        }

        #endregion

        #region 异步执行

        /// <summary>
        ///     异步删除单个key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Task<bool> KeyDeleteAsync(string key)
        {
            key = AddPrefixKey(key);
            return DoAsync(db => db.KeyDeleteAsync(key));
        }

        /// <summary>
        ///     异步删除多个Key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Task<long> KeyDeleteAsync(IEnumerable<string> key)
        {
            var keys = key.Select(AddPrefixKey).ToList();
            return DoAsync(db => db.KeyDeleteAsync(ConvertRedisKeys(keys)));
        }

        /// <summary>
        ///     异步重命名Key
        /// </summary>
        /// <param name="key">old key name</param>
        /// <param name="newKey">new key name</param>
        /// <returns></returns>
        public Task<bool> KeyRenameAsync(string key, string newKey)
        {
            key = AddPrefixKey(key);
            return DoAsync(db => db.KeyRenameAsync(key, newKey));
        }

        /// <summary>
        ///     异步设置Key的时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        public Task<bool> KeyExpireAsync(string key, TimeSpan? exp = default)
        {
            key = AddPrefixKey(key);
            return DoAsync(db => db.KeyExpireAsync(key, exp));
        }

        /// <summary>
        ///     异步设置Key的时间
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        public async Task<bool> KeyExpireAsync(string[] keys, TimeSpan? exp = default)
        {
            keys = AddPrefixKey(keys);
            return await DoAsync(async db =>
            {
                var tasks = keys.Select(key => db.KeyExpireAsync(key, exp)).ToList();
                var count = tasks.Count;
                var rs = await Task.WhenAll(tasks);
                return rs.Count(p => p) == count;
            });
        }

        /// <inheritdoc />
        public Task<TimeSpan?> KeyTimeToLiveAsync(string key)
        {
            key = AddPrefixKey(key);
            return DoAsync(db => db.KeyTimeToLiveAsync(key));
        }

        /// <inheritdoc />
        public Task<bool> KeyPersistAsync(string key)
        {
            key = AddPrefixKey(key);
            return DoAsync(db => db.KeyPersistAsync(key));
        }

        public Task<bool> KeyExistsAsync(string key)
        {
            key = AddPrefixKey(key);
            return DoAsync(db => db.KeyExistsAsync(key));
        }

        #endregion
    }
}