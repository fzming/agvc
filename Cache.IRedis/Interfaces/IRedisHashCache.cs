using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utility;

namespace Cache.IRedis.Interfaces
{
    /// <summary>
    ///     Redis 哈希表操作接口
    /// </summary>
    public interface IRedisHashCache : ISingletonDependency
    {
        /// <summary>
        ///     获取或创建哈希缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataKey">数据键名</param>
        /// <param name="createAsync">当缓存不存在时自动调用</param>
        /// <returns></returns>
        Task<T> GetOrCreateHashCacheAsync<T>(string dataKey, Func<Task<T>> createAsync);

        /// <summary>
        ///     获取或创建哈希缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hashKey">哈希键名</param>
        /// <param name="dataKey">数据键名</param>
        /// <param name="createAsync">当缓存不存在时自动调用</param>
        /// <param name="expires">过期时间</param>
        /// <returns></returns>
        Task<T> GetOrCreateHashCacheAsync<T>(string hashKey, string dataKey, Func<Task<T>> createAsync,
            TimeSpan? expires = null);

        #region 同步执行

        /// <summary>
        ///     是否被缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        bool HashExists(string key, string dataKey);


        /// <summary>
        ///     存储数据到hash表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        bool HashSet<T>(string key, string dataKey, T val);


        /// <summary>
        ///     从hash表中移除数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        bool HashDelete(string key, string dataKey);


        /// <summary>
        ///     移除hash中的多个值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        long HashRemove(string key, List<string> dataKey);


        /// <summary>
        ///     从hash表中获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        T HashGet<T>(string key, string dataKey);

        Dictionary<string, T> HashAll<T>(string key);
        object HashGet(string key, string dataKey, Type type);

        /// <summary>
        ///     为数字增长val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        double HashIncrement(string key, string dataKey, double val = 1);


        /// <summary>
        ///     为数字减少val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        double HashDecrement(string key, string dataKey, double val = 1);

        /// <summary>
        ///     获取hashkey所有Redis key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        List<T> HashKeys<T>(string key);

        #endregion

        #region 异步执行

        /// <summary>
        ///     异步是否被缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        Task<bool> HashExistsAsync(string key, string dataKey);


        /// <summary>
        ///     异步存储数据到hash表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        Task<bool> HashSetAsync<T>(string key, string dataKey, T val);

        /// <summary>
        ///     删除整个Hash
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<bool> DeleteAsync(string key);

        /// <summary>
        ///     异步从hash表中移除数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        Task<bool> HashDeleteAsync(string key, string dataKey);


        /// <summary>
        ///     异步移除hash中的多个值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        Task<long> HashRemoveAsync(string key, List<string> dataKey);


        /// <summary>
        ///     从hash表中获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        Task<T> HashGetAsync<T>(string key, string dataKey);

        Task<object> HashGetAsync(string key, string dataKey, Type type);
        Task<Dictionary<string, T>> HashAllAsync<T>(string key);

        /// <summary>
        ///     为数字增长val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        Task<double> HashIncrementAsync(string key, string dataKey, double val = 1);

        /// <summary>
        ///     为数字减少val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        Task<double> HashDecrementAsync(string key, string dataKey, double val = 1);


        /// <summary>
        ///     获取hashkey所有Redis key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<List<T>> HashKeysAsync<T>(string key);

        #endregion
    }
}