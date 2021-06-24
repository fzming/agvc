using CoreRepository.Core.Aggregate;

using MongoDB.Driver;

using Utility;

namespace CoreRepository
{
    /// <summary>
    /// Mongodb数据上下文接口
    /// </summary>
    public interface IMongoContext : ISingletonDependency
    {
        #region 获取集合名称
        string GetCollectionName<T>();
        string GetCollectionNameFromInterface<T>();
        string GetCollectionNameFromType<T>();
        #endregion
        #region 获取连接字符串
        string GetConnectionName<T>();
        string GetConnectionNameFromInterface<T>();
        string GetConnectionNameFromType<T>();

        #endregion
        IMongoCollection<T> GetCollection<T>() where T : AggregateRoot;
    }
}
