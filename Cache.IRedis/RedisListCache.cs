using System.Collections.Generic;
using System.Threading.Tasks;
using Cache.IRedis.Core;
using Cache.IRedis.Interfaces;

namespace Cache.IRedis
{
    /// <summary>
    /// Redis 集合缓存
    /// List——列表 有顺序可重复（处理不完时,排队相关处理）
    /// </summary>
    public class RedisListCache : RedisCaching, IRedisListCache
    {
        #region 同步执行
        /// <summary>
        /// 移除List内部指定的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="val"></param>
        public void Remove<T>(string key, T val)
        {
            key = AddPrefixKey(key);
            DoSync(db => db.ListRemove(key, ConvertJson(val)));
        }

        /// <summary>
        /// 获取指定Key的List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<T> Range<T>(string key)
        {
            key = AddPrefixKey(key);
            return DoSync(db =>
            {
                var val = db.ListRange(key);
                return ConvertList<T>(val);
            });
        }

        /// <summary>
        /// 插入（入队）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="val"></param>
        public void RightPush<T>(string key, T val)
        {
            key = AddPrefixKey(key);
            DoSync(db => db.ListRightPush(key, ConvertJson(val)));
        }

        /// <summary>
        /// 取出（出队）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T RightPop<T>(string key)
        {
            key = AddPrefixKey(key);
            return DoSync(db =>
            {
                var val = db.ListRightPop(key);
                return ConvertObj<T>(val);
            });
        }

        /// <summary>
        /// 入栈
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="val"></param>
        public void LeftPush<T>(string key, T val)
        {
            key = AddPrefixKey(key);
            DoSync(db => db.ListLeftPush(key, ConvertJson(val)));
        }

        /// <summary>
        /// 出栈
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T LeftPop<T>(string key)
        {
            key = AddPrefixKey(key);
            return DoSync(db =>
            {
                var val = db.ListLeftPop(key);
                return ConvertObj<T>(val);
            });
        }

        /// <summary>
        /// 获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long GetLength(string key)
        {
            key = AddPrefixKey(key);
            return DoSync(db => db.ListLength(key));
        }
        #endregion

        #region 异步执行
        /// <summary>
        /// 异步移除List内部指定的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="val"></param>
        public Task<long> RemoveAsync<T>(string key, T val)
        {
            key = AddPrefixKey(key);
            return DoAsync(db => db.ListRemoveAsync(key, ConvertJson(val)));
        }

        /// <summary>
        /// 异步获取指定Key的List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<List<T>> RangeAsync<T>(string key)
        {
            key = AddPrefixKey(key);
            var val = await DoAsync(db => db.ListRangeAsync(key));
            return ConvertList<T>(val);
        }

        /// <summary>
        /// 异步插入（入队）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="val"></param>
        public Task<long> RightPushAsync<T>(string key, T val)
        {
            key = AddPrefixKey(key);
            return DoAsync(db => db.ListRightPushAsync(key, ConvertJson(val)));
        }

        /// <summary>
        /// 异步取出（出队）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T> RightPopAsync<T>(string key)
        {
            key = AddPrefixKey(key);
            var val = await DoAsync(db => db.ListRightPopAsync(key));
            return ConvertObj<T>(val);
        }

        /// <summary>
        /// 异步入栈
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="val"></param>
        public Task<long> LeftPushAsync<T>(string key, T val)
        {
            key = AddPrefixKey(key);
            return DoAsync(db => db.ListLeftPushAsync(key, ConvertJson(val)));
        }

        /// <summary>
        /// 异步出栈
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T> LeftPopAsync<T>(string key)
        {
            key = AddPrefixKey(key);
            var val = await DoAsync(db => db.ListLeftPopAsync(key));
            return ConvertObj<T>(val);
        }

        /// <summary>
        /// 获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Task<long> GetLengthAsync(string key)
        {
            key = AddPrefixKey(key);
            return DoAsync(db => db.ListLengthAsync(key));
        }
        #endregion

        public RedisListCache(IRedisConnectionMultiplexer redisRedisConnectionMultiplexer) : base(redisRedisConnectionMultiplexer)
        {
        }
    }
}