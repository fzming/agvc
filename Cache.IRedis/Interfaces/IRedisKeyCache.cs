using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utility;

namespace Cache.IRedis.Interfaces
{
    /// <summary>
    /// Redis Key操作接口
    /// </summary>
    public interface IRedisKeyCache : ISingletonDependency
    {
        #region 同步执行
       
        /// <summary>
        /// 删除单个Key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool KeyDelete(string key);


        /// <summary>
        /// 删除多个Key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        long KeyDelete(IEnumerable<string> key);
   

        /// <summary>
        /// 重命名Key
        /// </summary>
        /// <param name="key">old key name</param>
        /// <param name="newKey">new key name</param>
        /// <returns></returns>
        bool KeyRename(string key, string newKey);


        /// <summary>
        /// 设置Key的时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="exp">设置key的过期时间(秒)</param>
        /// <returns></returns>
        bool KeyExpire(string key, TimeSpan? exp = default);
        /// <summary>
        /// 查看key的有效期(ttl) 倒计时
        /// </summary>
        /// <param name="key"></param>
        /// <returns>
        /// >0，还剩余多少秒存活时间，
        /// -2, 不存在redis中，
        /// -1, 永久有效，持久化
        /// </returns>
        TimeSpan? KeyTimeToLive(string key);
        /// <summary>
        /// 清除key的过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool KeyPersist(string key);
        #endregion

        #region 异步执行

        /// <summary>
        /// 异步删除单个key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<bool> KeyDeleteAsync(string key);


        /// <summary>
        /// 异步删除多个Key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<long> KeyDeleteAsync(IEnumerable<string> key);
     

        /// <summary>
        ///  异步重命名Key
        /// </summary>
        /// <param name="key">old key name</param>
        /// <param name="newKey">new key name</param>
        /// <returns></returns>
        Task<bool> KeyRenameAsync(string key, string newKey);


        /// <summary>
        /// 异步设置Key的时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        Task<bool> KeyExpireAsync(string key, TimeSpan? exp = default);
        /// <summary>
        /// 异步设置Key的时间
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        Task<bool> KeyExpireAsync(string[] keys, TimeSpan? exp = default);
        /// <summary>
        /// 查看key的有效期(ttl) 倒计时
        /// </summary>
        /// <param name="key"></param>
        /// <returns>
        /// >0，还剩余多少秒存活时间，
        /// -2, 不存在redis中，
        /// -1, 永久有效，持久化
        /// </returns>
        Task<TimeSpan?> KeyTimeToLiveAsync(string key);
        /// <summary>
        /// 清除key的过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<bool> KeyPersistAsync(string key);
        /// <summary>
        /// 指定KEY是否存在
        /// </summary>
        /// <returns>The exists async.</returns>
        /// <param name="key">Key.</param>
        Task<bool> KeyExistsAsync(string key);
        #endregion
    }
}