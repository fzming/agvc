using Utility;

namespace CoreRepository
{
    /// <summary>
    /// Mongo工作单元
    /// </summary>
    public interface IMongoUnitOfWork : ISingletonDependency
    {
        IMongoContext MongoContext { get; }
    }
}
