using System.Collections.Generic;
using System.Threading.Tasks;
using Cache.IRedis.Core;
using Cache.IRedis.Interfaces;

namespace Cache.IRedis
{

    /// <summary>
    /// Redis SortedSet缓存
    /// 有顺序，不能重复 （服务器消耗最高，要排序还要去重，尽量少用，）
    /// </summary>
    public class RedisSortedSetCache : RedisCaching, IRedisSortedSetCache
    {
        #region 同步执行
        /// <summary>
        /// 无序添加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        public bool SortedSetAdd<T>(string key, T val, double score)
        {
            key = AddPrefixKey(key);
            return DoSync(db => db.SortedSetAdd(key, ConvertJson(val), score));
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public bool SortedSetRemove<T>(string key, T val)
        {
            key = AddPrefixKey(key);
            return DoSync(db => db.SortedSetRemove(key, ConvertJson(val)));
        }

        /// <summary>
        /// 获取全部
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<T> SortedSetRangeByRank<T>(string key)
        {
            key = AddPrefixKey(key);
            return DoSync(db =>
            {
                var val = db.SortedSetRangeByRank(key);
                return ConvertList<T>(val);
            });
        }

        /// <summary>
        ///  获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long SortedSetLength(string key)
        {
            key = AddPrefixKey(key);
            return DoSync(db => db.SortedSetLength(key));

        }
        #endregion

        #region 异步执行
        /// <summary>
        /// 异步无序添加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        public Task<bool> SortedSetAddAsync<T>(string key, T val, double score)
        {
            key = AddPrefixKey(key);
            return DoAsync(db => db.SortedSetAddAsync(key, ConvertJson(val), score));
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public Task<bool> SortedSetRemoveAsync<T>(string key, T val)
        {
            key = AddPrefixKey(key);
            return DoAsync(db => db.SortedSetRemoveAsync(key, ConvertJson(val)));
        }

        /// <summary>
        /// 获取全部
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<List<T>> SortedSetRangeByRankAsync<T>(string key)
        {
            key = AddPrefixKey(key);
            var val = await DoAsync(db => db.SortedSetRangeByRankAsync(key));
            return ConvertList<T>(val);
        }

        /// <summary>
        ///  获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Task<long> SortedSetLengthAsync(string key)
        {
            key = AddPrefixKey(key);
            return DoAsync(db => db.SortedSetLengthAsync(key));

        }
        #endregion

        public RedisSortedSetCache(IRedisConnectionMultiplexer redisRedisConnectionMultiplexer) : base(redisRedisConnectionMultiplexer)
        {
        }
    }
}