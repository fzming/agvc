using System.Collections.Generic;
using System.Threading.Tasks;
using Utility;

namespace Cache.IRedis.Interfaces
{
    /// <summary>
    /// Redis LIST 缓存接口
    /// https://www.cnblogs.com/sdgf/p/6244937.html
    /// </summary>
    public interface IRedisListCache : ISingletonDependency
    {
        #region 同步执行

        /// <summary>
        /// 移除List内部指定的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="val"></param>
        void Remove<T>(string key, T val);
      

        /// <summary>
        /// 获取指定Key的List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        List<T> Range<T>(string key);
      

        /// <summary>
        /// 插入（入队）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="val"></param>
        void RightPush<T>(string key, T val);
       

        /// <summary>
        /// 取出（出队）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T RightPop<T>(string key);
      

        /// <summary>
        /// 入栈
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="val"></param>
        void LeftPush<T>(string key, T val);
       

        /// <summary>
        /// 出栈
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T LeftPop<T>(string key);
       

        /// <summary>
        /// 获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        long GetLength(string key);
      
        #endregion

        #region 异步执行

        /// <summary>
        /// 异步移除List内部指定的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="val"></param>
        Task<long> RemoveAsync<T>(string key, T val);
      

        /// <summary>
        /// 异步获取指定Key的List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<List<T>> RangeAsync<T>(string key);
      

        /// <summary>
        /// 异步插入（入队）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="val"></param>
        Task<long> RightPushAsync<T>(string key, T val);
       

        /// <summary>
        /// 异步取出（出队）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<T> RightPopAsync<T>(string key);
      

        /// <summary>
        /// 异步入栈
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="val"></param>
        Task<long> LeftPushAsync<T>(string key, T val);
     

        /// <summary>
        /// 异步出栈
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<T> LeftPopAsync<T>(string key);


        /// <summary>
        /// 获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<long> GetLengthAsync(string key);

        #endregion
    }
}