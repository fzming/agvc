using CoreData.Core;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AgvcEntitys.System
{
    /// <summary>
    ///     机构功能值定义
    /// </summary>
    public class OrganizationFeatureValue : OEntity
    {
        /// <summary>
        ///     系统功能ID
        /// </summary>
        [BsonRepresentation(BsonType.ObjectId)]
        public string SysFeatureId { get; set; }

        /// <summary>
        ///     机构定义值
        /// </summary>
        public string Value { get; set; }
    }
}