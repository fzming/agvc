namespace CoreRepository
{
    /// <summary>
    ///     Mongo unit of work.
    /// </summary>
    public class MongoUnitOfWork : IMongoUnitOfWork
    {
        public MongoUnitOfWork(IMongoContext mongoContext)
        {
            MongoContext = mongoContext;
        }

        public IMongoContext MongoContext { get; }
    }
}