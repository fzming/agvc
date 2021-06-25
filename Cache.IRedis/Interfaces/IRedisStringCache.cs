using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utility;

namespace Cache.IRedis.Interfaces
{
    /// <summary>
    /// Redis String 缓存接口
    /// String——字符串
    /// </summary>
    public interface IRedisStringCache : ISingletonDependency
    {
        #region 同步执行
        /// <summary>
        /// 单个保存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val">值</param>
        /// <param name="exp">过期时间</param>
        /// <returns></returns>
        bool StringSet(string key, string val, TimeSpan? exp = default);


        /// <summary>
        /// 保存多个key value
        /// </summary>
        /// <param name="keyValues">键值对</param>
        /// <returns></returns>
        bool StringSet<T>(Dictionary<string, T> keyValues);


        /// <summary>
        /// 保存一个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        bool StringSet<T>(string key, T obj, TimeSpan? exp = default);
        

        /// <summary>
        /// 获取单个
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string StringGet(string key);

        /// <summary>
        /// 获取单个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T StringGet<T>(string key);


        /// <summary>
        /// 为数字增长val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val">可以为负数</param>
        /// <returns>增长后的值</returns>
        double StringIncrement(string key, double val = 1);

        /// <summary>
        /// 为数字减少val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val">可以为负数</param>
        /// <returns>增长后的值</returns>
        double StringDecrement(string key, double val = 1);

        #endregion

        #region 异步执行
        /// <summary>
        /// 异步保存单个
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        Task<bool> StringSetAsync(string key, string val, TimeSpan? exp = default);

        /// <summary>
        /// 异步保存多个key value
        /// </summary>
        /// <param name="keyValues">键值对</param>
        /// <returns></returns>
        Task<bool> StringSetAsync<T>(Dictionary<string, T> keyValues);


        /// <summary>
        /// 异步保存一个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        Task<bool> StringSetAsync<T>(string key, T obj, TimeSpan? exp = default);
        

        /// <summary>
        /// 异步获取单个
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<string> StringGetAsync(string key);


        /// <summary>
        /// 异步获取单个
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<T> StringGetAsync<T>(string key);


        /// <summary>
        /// 异步为数字增长val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val">可以为负数</param>
        /// <returns>增长后的值</returns>
        Task<double> StringIncrementAsync(string key, double val = 1);

        /// <summary>
        /// 为数字减少val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val">可以为负数</param>
        /// <returns>增长后的值</returns>
        Task<double> StringDecrementAsync(string key, double val = 1);

        #endregion
    }
}