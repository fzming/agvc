namespace CoreRepository
{
    /// <summary>
    /// Mongo工作单元
    /// </summary>
    public interface IMongoUnitOfWork:IUnitOfWork 
    {
        IMongoContext MongoContext { get;}
    }
}
