using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace CoreData.Core
{
    /// <summary>
    ///     mongo entity
    /// </summary>
    [BsonIgnoreExtraElements(Inherited = true)]
    public abstract class MongoEntity : PrimaryAggregateEntity<string>
    {
        private DateTime _createdOn;

        /// <summary>
        ///     Constructor, assigns new id
        /// </summary>
        protected MongoEntity()
        {
            Id = ObjectId.GenerateNewId().ToString();
        }

        /// <summary>
        ///     id in string format
        /// </summary>
        [BsonElement("_id", Order = 0)]
        [BsonId] //使用 [BsonId] 进行批注，以将此属性指定为文档的主键
        [BsonRepresentation(BsonType
            .ObjectId)] //使用 [BsonRepresentation(BsonType.ObjectId)] 进行批注，以允许将参数作为类型 string 而不是 ObjectId 结构进行传递。 Mongo 处理从 string 到 ObjectId 的转换。
        public sealed override string Id { get; set; }


        /// <summary>
        ///     create date
        /// </summary>
        [BsonElement("_c", Order = 1)]
        [JsonProperty("_c")]
        [BsonRepresentation(BsonType.DateTime)]
        public sealed override DateTime CreatedOn
        {
            get
            {
                if (_createdOn == DateTime.MinValue && !string.IsNullOrEmpty(Id))
                    _createdOn = ObjectId.CreationTime;
                return _createdOn;
            }
            set => _createdOn = value;
        }

        /// <summary>
        ///     modify date
        /// </summary>
        [BsonElement("_m", Order = 2)]
        [JsonProperty("_m")]
        [BsonRepresentation(BsonType.DateTime)]
        public sealed override DateTime ModifiedOn { get; set; }


        /// <summary>
        ///     id in objectId format
        /// </summary>
        [BsonIgnore]
        [JsonIgnore]
        public ObjectId ObjectId => ObjectId.TryParse(Id, out var obj) ? obj : ObjectId.Empty;
    }
}