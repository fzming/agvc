using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CoreData.Core
{
    /// <summary>
    ///     机构关联实体基类
    /// </summary>
    public class OEntity : MongoEntity
    {
        /// <summary>
        ///     所属机构ID
        /// </summary>
        [BsonElement("_o", Order = 3)]
        [BsonRepresentation(BsonType.ObjectId)]
        public string OrgId { get; set; }
    }
}