using Utility;

namespace CoreRepository
{
    /// <summary>
    /// Mongo unit of work.
    /// </summary>
    public class MongoUnitOfWork : IMongoUnitOfWork
    {
        public IMongoContext MongoContext { get; }

        public MongoUnitOfWork(IMongoContext mongoContext)
        {
            MongoContext = mongoContext;

        }
    }
}
