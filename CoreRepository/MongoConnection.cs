using System;
using MongoDB.Driver;

namespace CoreRepository
{
    public class MongoConnection : IMongoConnection
    {
        /// <summary>
        ///     单例锁
        /// </summary>
        private readonly object Locker = new();

        /// <summary>
        ///     Gets or sets the client.
        ///     MongoClient是个线程安全的类，自带线程池,无需实例化多个
        /// </summary>
        /// <value>The client.</value>
        private static MongoClient Client { get; set; }

        /// <summary>
        ///     Gets or sets the database.
        /// </summary>
        /// <value>The database.</value>
        public IMongoDatabase Database { get; set; }

        /// <summary>
        ///     Makes the sure connected.
        /// </summary>
        /// <param name="mongoUrl"></param>
        /// <param name="databaseName"></param>
        public void MakeSureConnected(string mongoUrl, string databaseName)
        {
            if (Client != null) return;
            lock (Locker)
            {
                CreateMongoClient(mongoUrl, databaseName);
            }
        }

        /// <summary>
        ///     Creates the mongo client.
        /// </summary>
        private void CreateMongoClient(string mongoUrl, string databaseName)
        {
            if (Client != null) return;
            var url = new MongoUrl(mongoUrl);
            var clientSettings = MongoClientSettings.FromUrl(url);

            clientSettings.DirectConnection = true;
            clientSettings.RetryReads = true;
            clientSettings.RetryWrites = true;
            clientSettings.ConnectTimeout = new TimeSpan(0, 0, 0, 30, 0); //30秒超时
            clientSettings.MinConnectionPoolSize = 50; //当链接空闲时,空闲线程池中最大链接数，默认0
            clientSettings.MaxConnectionPoolSize = 1000; //默认100*/

            // clientSettings.WriteConcern = WriteConcern.Acknowledged;
            Client = new MongoClient(clientSettings);
            Database = Client.GetDatabase(databaseName);
        }
    }
}