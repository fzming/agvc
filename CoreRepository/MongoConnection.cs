using System;
using MongoDB.Driver;
using Utility.Config;

namespace CoreRepository
{
    internal class MongoConnection
    {
        /// <summary>
        /// 单例锁
        /// </summary>
        private static readonly object Locker = new();
        /// <summary>
        /// 全局唯一连接
        /// </summary>
        private static MongoConnection _connection;
        /// <summary>
        /// Gets or sets the client.
        /// MongoClient是个线程安全的类，自带线程池,无需实例化多个
        /// </summary>
        /// <value>The client.</value>
        private static MongoClient Client { get; set; }
        /// <summary>
        /// Gets or sets the database.
        /// </summary>
        /// <value>The database.</value>
        public IMongoDatabase Database { get; set; }
        /// <summary>
        /// 禁止外部实例化调用
        /// </summary>
        private MongoConnection() { }
        public static MongoConnection Singleton()
        {

            //这里的lock其实使用的原理可以用一个词语来概括“互斥”这个概念也是操作系统的精髓
            //其实就是当一个进程进来访问的时候，其他进程便先挂起状态
            if (_connection == null)//区别就在这里
            {
                lock (Locker)
                {
                    // 如果类的实例不存在则创建，否则直接返回
                    _connection ??= new MongoConnection();
                }
            }
            return _connection;

        }
        /// <summary>
        /// Creates the mongo client.
        /// </summary>
        /// <param name="configManager">Config manager.</param>
        private void CreateMongoClient(IConfigManager configManager)
        {
            if (Client != null) return;
            var config = configManager.GetConfig<MongoConfig>("Mongo");
            var url = new MongoUrl(config.MongoUrl);
            var clientSettings = MongoClientSettings.FromUrl(url);

            clientSettings.DirectConnection = true;
            clientSettings.RetryReads = true;
            clientSettings.RetryWrites = true;
            clientSettings.ConnectTimeout = new TimeSpan(0, 0, 0, 30, 0); //30秒超时
            clientSettings.MinConnectionPoolSize = 50;//当链接空闲时,空闲线程池中最大链接数，默认0
            clientSettings.MaxConnectionPoolSize = 1000;//默认100*/
            
            // clientSettings.WriteConcern = WriteConcern.Acknowledged;
            Client = new MongoClient(clientSettings);
            Database = Client.GetDatabase(config.DatabaseName);
        }
        /// <summary>
        /// Makes the sure connected.
        /// </summary>
        /// <param name="configManager">Config manager.</param>
        public void MakeSureConnected(IConfigManager configManager)
        {
            if (Client != null) return;
            lock (Locker)
            {
                CreateMongoClient(configManager);
            }


        }
    }
}