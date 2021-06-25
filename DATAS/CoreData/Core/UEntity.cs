using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CoreData.Core
{
    /// <summary>
    /// 用户关联实体基类
    /// </summary>
    public class UEntity : OEntity
    {
        /// <summary>
        /// 所属用户ID
        /// </summary>
        [BsonElement("_u", Order = 4)]
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }
        /// <summary>
        /// 公司ID
        /// 前端用户登录后可以通过某种渠道绑定往来单位
        /// </summary>
        public string CompanyId { get; set; }
    }
}