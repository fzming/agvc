namespace CoreRepository
{
    /// <summary>
    /// Mongo unit of work.
    /// </summary>
    internal class MongoUnitOfWork:IMongoUnitOfWork
    {
        public IMongoContext MongoContext { get; }

        public MongoUnitOfWork(IMongoContext mongoContext)
        {
            MongoContext = mongoContext;
           
        }
    }
}
