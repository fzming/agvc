using System;
using System.Collections.Concurrent;
using System.Reflection;
using CoreData.Core;
using CoreData.Core.Aggregate;
using CoreData.Core.Attributes;
using CoreRepository.Converters;
using CoreRepository.MapConventions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
namespace CoreRepository
{

    public class MongoContext : IMongoContext
    {
        private MongoConnection Connection { get; }
        private static readonly ConcurrentDictionary<Type, string> connectionNameDictionary;
        #region RegConventionRegistry

        static MongoContext()
        {
            connectionNameDictionary = new ConcurrentDictionary<Type, string>();
            RegConventionRegistry();

        }
        public MongoContext(IConfiguration configuration )
        {
            var section = configuration.GetSection("Mongo");
            this.MongoConfig = section.Get<MongoConfig>();
            Connection = MongoConnection.Singleton();//单例数据库连接
        }

        public MongoConfig MongoConfig { get; }

        static void RegConventionRegistry()
        {
            //注册Conventions
            var pack = new ConventionPack
            {
                //由于类定义变更，某个字段不需要了，但是如果不进行设定的话，发序列化会出错，某个数据库字段没有配置的实体字段（IgnoreExtraElementsConvention）
                new IgnoreExtraElementsConvention(true),
                //在序列化的时候，为了节省空间，希望字段为空的时候，不进行序列化。（IgnoreIfNullConvention）
                new IgnoreIfNullConvention(true),
                //日期型的数据，序列化的时候，希望可以指定时区（RegisterSerializer）
                new UseLocalDateTimeConvention()
            };
            //支持decimal和decimal? 类型 https://stackoverflow.com/questions/43473147/how-to-use-decimal-type-in-mongodb
            BsonSerializer.RegisterSerializer(typeof(decimal), new DecimalSerializer(BsonType.Decimal128));
            BsonSerializer.RegisterSerializer(typeof(decimal?), new NullableSerializer<decimal>(new DecimalSerializer(BsonType.Decimal128)));
            //https://kevsoft.net/2020/06/25/storing-guids-as-strings-in-mongodb-with-csharp.html
            BsonSerializer.RegisterSerializer(typeof(Guid), new GuidSerializer(BsonType.String));
            BsonSerializer.RegisterSerializer(typeof(Guid?), new GuidSerializer(BsonType.String));
            //解决double查询问题
            //BsonSerializer.RegisterSerializer(typeof(double), new CustomDoubleSerializer());
            //BsonSerializer.RegisterSerializer(typeof(double?), new CustomDoubleSerializer());
            ConventionRegistry.Register("CustomElementsConvention", pack, t => true);

            //解决插入emoji表情字符的问题
            BsonSerializer.RegisterSerializer(typeof(string), new IllegalityStringSerializer());
            //            ConventionRegistry.Register(
            //                "IgnoreManyProperties",
            //                new ConventionPack { new IgnoreManyPropertiesConvention() },
            //                type => true);
            //假如不采用UseLocalDateTimeConvention方案，可以使用下面方案进行日期的时区指定
            //DateTime Localize 
            //BsonSerializer.RegisterSerializer(typeof(DateTime), new DateTimeSerializer(DateTimeKind.Local));
            //第三种
            //BsonSerializer.RegisterSerializer(typeof(DateTime), new LocalTimeSerializer());

        }

        #endregion


        #region Collection Name

        /// <summary>
        /// Determines the collection name for T and assures it is not empty
        /// </summary>
        /// <returns>Returns the collection name for T.</returns>
        public string GetCollectionName<T>()
        {
            var type = typeof(T);
            if (connectionNameDictionary.TryGetValue(type, out var collectionName))
            {
                return collectionName;
            }

            collectionName = typeof(T).GetTypeInfo().BaseType == typeof(object) ?
                GetCollectionNameFromInterface<T>() :
                GetCollectionNameFromType<T>();

            if (string.IsNullOrEmpty(collectionName))
            {
                collectionName = typeof(T).Name;
            }

            connectionNameDictionary.TryAdd(type, collectionName);
            return collectionName;
        }

        /// <summary>
        /// Determines the collection name from the specified type.
        /// </summary>
        /// <returns>Returns the collection name from the specified type.</returns>
        public string GetCollectionNameFromInterface<T>()
        {
            // Check to see if the object (inherited from Entity) has a CollectionName attribute
            var att = typeof(T).GetTypeInfo().GetCustomAttribute<CollectionNameAttribute>();

            return att?.Name ?? typeof(T).Name;
        }

        /// <summary>
        /// Determines the collectionname from the specified type.
        /// </summary>
        /// <returns>Returns the collectionname from the specified type.</returns>
        public string GetCollectionNameFromType<T>()
        {
            var t = typeof(T);

            // Check to see if the object (inherited from Entity) has a CollectionName attribute
            var att = typeof(T).GetTypeInfo().GetCustomAttribute<CollectionNameAttribute>();
            var collectionName = att != null ? att.Name : t.Name;

            return collectionName;
        }

        #endregion  

        #region Connection Name

        /// <summary>
        /// Determines the connection name for T and assures it is not empty
        /// </summary>
        /// <returns>Returns the connection name for T.</returns>
        public string GetConnectionName<T>()
        {
            var connectionName = typeof(T).GetTypeInfo().BaseType == typeof(object) ?
                GetConnectionNameFromInterface<T>() :
                GetConnectionNameFromType<T>();

            if (string.IsNullOrEmpty(connectionName))
            {
                connectionName = typeof(T).Name;
            }
            return connectionName.ToLowerInvariant();
        }

        /// <summary>
        /// Determines the connection name from the specified type.
        /// </summary>
        /// <returns>Returns the connection name from the specified type.</returns>
        public string GetConnectionNameFromInterface<T>()
        {
            // Check to see if the object (inherited from Entity) has a ConnectionName attribute
            var att = typeof(T).GetTypeInfo().GetCustomAttribute<ConnectionNameAttribute>();
            return att?.Name ?? typeof(T).Name;
        }

        /// <summary>
        /// Determines the connection name from the specified type.
        /// </summary>
        /// <returns>Returns the connection name from the specified type.</returns>
        public string GetConnectionNameFromType<T>()
        {
            var entitytype = typeof(T);
            string connectionname;

            // Check to see if the object (inherited from Entity) has a ConnectionName attribute
            var att = typeof(T).GetTypeInfo().GetCustomAttribute<ConnectionNameAttribute>();
            if (att != null)
            {
                // It does! Return the value specified by the ConnectionName attribute
                connectionname = att.Name;
            }
            else
            {
                if (typeof(MongoEntity).GetTypeInfo().IsAssignableFrom(entitytype))
                {
                    // No attribute found, get the basetype
                    while (entitytype.GetTypeInfo().BaseType != typeof(MongoEntity))
                    {
                        entitytype = entitytype.GetTypeInfo().BaseType;
                    }
                }
                connectionname = entitytype?.Name;
            }

            return connectionname;
        }

        #endregion

        /// <summary>
        /// 获取集合
        /// </summary>
        /// <returns>The collection from URL.</returns>
        /// <typeparam name="T">The 1st type parameter.</typeparam>

        public IMongoCollection<T> GetCollection<T>() where T : AggregateRoot
        {
            Connection.MakeSureConnected(MongoConfig.MongoUrl,MongoConfig.DatabaseName);

            return Connection.Database.GetCollection<T>(GetCollectionName<T>());
        }
    }
}