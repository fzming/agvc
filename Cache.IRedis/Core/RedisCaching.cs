using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cache.IRedis.Interfaces;
using Newtonsoft.Json;
using Polly;
using StackExchange.Redis;
using Utility;
using Utility.Extensions;

namespace Cache.IRedis.Core
{
    /// <summary>
    /// Redis Caching基类
    /// </summary>
    public abstract class RedisCaching
    {
        private IRedisConnectionMultiplexer RedisConnectionMultiplexer { get; }
        private readonly string KeyPrefix = null;
        private const int RetryMaxTimes = 3;//REDIS操作失败时,最大重试次数

        public RedisCaching(IRedisConnectionMultiplexer redisRedisConnectionMultiplexer)
        {
            RedisConnectionMultiplexer = redisRedisConnectionMultiplexer;
        }

        #region 辅助方法
        /// <summary>
        /// 添加键名称的前缀
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string AddPrefixKey(string key)
        {
            var prefix = KeyPrefix ?? RedisConnectionMultiplexer.RedisConfig.RedisKeyPrex;
            return prefix + key;
        }
        public string[] AddPrefixKey(string[] keys)
        {
            return keys.Select(AddPrefixKey).ToArray();

        }
        /// <summary>
        /// 异步执行Redis数据库读写操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="asyncFunc">异步方法</param>
        /// <returns></returns>
        public Task<T> DoAsync<T>(Func<IDatabase, Task<T>> asyncFunc)
        {

            var retryPolicy = Policy.Handle<RedisException>().WaitAndRetryAsync(RetryMaxTimes, count => TimeSpan.FromSeconds(count));
            var database = RedisConnectionMultiplexer.ConnectionMultiplexer.GetDatabase();
            return retryPolicy.ExecuteAsync(() => asyncFunc(database));
        }
        /// <summary>
        /// 同步执行Redis数据库读写操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="syncFunc">同步方法</param>
        /// <returns></returns>
        public T DoSync<T>(Func<IDatabase, T> syncFunc)
        {

            var retryPolicy = Policy.Handle<RedisException>().WaitAndRetry(RetryMaxTimes, count => TimeSpan.FromSeconds(count));
            var database = RedisConnectionMultiplexer.ConnectionMultiplexer.GetDatabase();
            return retryPolicy.Execute(() => syncFunc(database));

        }
        public T DoServerSync<T>(Func<IServer, int, T> syncFunc)
        {
            return syncFunc(
                RedisConnectionMultiplexer.Server,
                RedisConnectionMultiplexer.RedisConfig.RedisDefaultDatabase);
        }
        /// <summary>
        /// 对象转json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="val"></param>
        /// <returns></returns>
        public string ConvertJson<T>(T val)
        {
            return val.ToJson();
        }
        /// <summary>
        /// 值转对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="val"></param>
        /// <returns></returns>
        public T ConvertObj<T>(RedisValue val)
        {
            if (!val.HasValue || val.IsNull || val.IsNullOrEmpty)
            {
                return default;
            }

            if (typeof(T) == typeof(string))
            {
                return (T)Convert.ChangeType(val, typeof(T));
            }

            if (typeof(T).IsValueType) //int 值类型
            {

                var t = typeof(T);
                t = Nullable.GetUnderlyingType(t) ?? t;
                return (T)Convert.ChangeType(val, t);

            }
            try
            {
                return JsonConvert.DeserializeObject<T>(val);
            }
            catch
            {
                return default;
            }

        }
        /// <summary>
        /// 根据类型进行转换
        /// </summary>
        /// <param name="val"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public object ConvertObj(RedisValue val, Type type)
        {
            if (!val.HasValue || val.IsNull || val.IsNullOrEmpty)
            {
                return null;
            }

            if (type.IsValueType || type == typeof(string))
            {
                return SafeCasting.CastTo(type, val);
            }
            return JsonConvert.DeserializeObject(val, type);
        }

        /// <summary>
        /// 集合值转集合对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        public List<T> ConvertList<T>(RedisValue[] values)
        {
            return values.Select(ConvertObj<T>).ToList();

        }

        /// <summary>
        /// 集合转key
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public RedisKey[] ConvertRedisKeys(IEnumerable<string> values)
        {
            return values.Select(k => (RedisKey)k).ToArray();
        }

        #endregion
    }
}
