using System.Threading.Tasks;
using Utility;

namespace Cache.IRedis.Interfaces
{
    /// <summary>
    ///     Redis SET 缓存接口
    /// </summary>
    /// <remarks>
    ///     Set：元素无顺序，不能重复（去重）
    /// </remarks>
    public interface IRedisSetCache : ISingletonDependency
    {
        /*
         *
//         *  Set 就是一个集合，集合的概念就是一堆不重复值的组合。利用 Redis 提供的 Set 数据结构，可以存储一些集合性的数据。
//
//            比如在微博应用中，可以将一个用户所有的关注人存在一个集合中，将其所有粉丝存在一个集合。
//
//            因为 Redis 非常人性化的为集合提供了求交集、并集、差集等操作，那么就可以非常方便的实现如共同关注、共同喜好、二度好友等功能，对上面的所有集合操作，你还可以使用不同的命令选择将结果返回给客户端还是存集到一个新的集合中。
//
//            1.共同好友、二度好友
//            2.利用唯一性，可以统计访问网站的所有独立 IP
//            3.好友推荐的时候，根据 tag 求交集，大于某个 threshold 就可以推荐
         */

        #region 同步执行

        /// <summary>
        ///     向key的添加value值 ，如果有重复数据会只保留一个
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        bool SetAdd(string key, string val);

        bool SetAdd(string key, string[] values);

        /// <summary>
        ///     查询key值中所有value值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string[] SetMembers(string key);

        /// <summary>
        ///     获取长度
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        long SetLength(string key);


        /// <summary>
        ///     是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        bool SetContains(string key, string val);


        /// <summary>
        ///     移除
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        bool SetRemove(string key, string val);

        #endregion

        #region 异步执行

        /// <summary>
        ///     向key的添加value值 ，如果有重复数据会只保留一个
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        Task<bool> SetAddAsync(string key, string val);

        Task<bool> SetAddAsync(string key, string[] values);

        Task<string[]> SetMembersAsync(string key);

        /// <summary>
        ///     获取长度
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<long> SetLengthAsync(string key);


        /// <summary>
        ///     是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        Task<bool> SetContainsAsync(string key, string val);


        /// <summary>
        ///     移除
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        Task<bool> SetRemoveAsync(string key, string val);

        #endregion
    }
}