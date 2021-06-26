using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utility;

namespace Cache.IRedis.Interfaces
{
    /// <summary>
    ///     带过期Redis哈希缓存
    /// </summary>
    public interface IRedisExpiresHashCache : ISingletonDependency
    {
        string GetExpireKey(string key);

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
        /// <param name="expires"></param>
        /// <returns></returns>
        Task<bool> HashSetAsync<T>(string key, string dataKey, T val, TimeSpan expires);


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
    }
}