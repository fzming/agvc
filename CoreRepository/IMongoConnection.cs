using MongoDB.Driver;
using Utility;

namespace CoreRepository
{
    public interface IMongoConnection : ISingletonDependency
    {
        IMongoDatabase Database { get; set; }

        void MakeSureConnected(string mongoUrl, string databaseName);
    }
}