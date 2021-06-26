using CoreData.Core;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AgvcEntitys.Organization
{
    /// <summary>
    ///     部门
    /// </summary>
    public class Department : OEntity
    {
        /// <summary>
        ///     分公司ID
        /// </summary>
        [BsonRepresentation(BsonType.ObjectId)]
        public string BranchCompanyId { get; set; }

        public string Name { get; set; }
    }
}