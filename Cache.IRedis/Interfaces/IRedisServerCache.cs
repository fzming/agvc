using Utility;

namespace Cache.IRedis.Interfaces
{
    public interface IRedisServerCache : ISingletonDependency
    {
        /// <summary>
        /// 数据库所有键值扫描
        /// 查询数据库里面存有哪些key
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        string[] KeyScan(string pattern = "*", int pageSize = 10);

    }
}