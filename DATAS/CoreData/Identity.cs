using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CoreData
{
    /// <summary>
    ///     带名字和ID的识别基类
    /// </summary>
    public class Identity
    {
        /// <summary>
        ///     初始化 <see cref="T:System.Object" /> 类的新实例。
        /// </summary>
        public Identity(string id, string name)
        {
            Id = id;
            Name = name;
        }

        /// <summary>
        ///     初始化 <see cref="T:System.Object" /> 类的新实例。
        /// </summary>
        public Identity()
        {
        }

        /// <summary>
        ///     名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     ID
        /// </summary>
        [BsonRepresentation(BsonType.String)]
        [BsonElement("_id", Order = 0)]
        public string Id { get; set; }
    }

    /// <summary>
    ///     人员标识类
    /// </summary>
    public class UserIdentity : Identity
    {
        public string Avatar { get; set; }
    }

    /// <summary>
    ///     关联实体
    /// </summary>
    public class RelativeIdentity
    {
        /// <summary>
        ///     名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     ID
        /// </summary>
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
    }
}