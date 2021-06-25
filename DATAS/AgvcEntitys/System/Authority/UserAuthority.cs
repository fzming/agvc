using CoreData.Core;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AgvcEntitys.System.Authority
{
    /// <summary>
    /// 用户或角色权限表
    /// </summary>
    public class UserAuthority:OEntity
    {
        /// <summary>
        /// 授权对象ID
        /// 如：角色ID，用户ID 根据AuthorizedType来定
        /// </summary>
        [BsonRepresentation(BsonType.ObjectId)]
        public string AuthorId{ get; set; }
        /// <summary>
        /// 授权类型
        /// </summary>
        public AuthorizedType AuthorType { get; set; }
        /// <summary>
        /// 授权菜单ID
        /// </summary>
        [BsonRepresentation(BsonType.ObjectId)]
        public string MenuId { get; set; }
        /// <summary>
        /// 授权码
        /// 来至：AuthorityCode.Code
        /// </summary>
        public string AuthCode { get; set; }
        /// <summary>
        /// 授权类型为用户时，用户明确禁止的权限
        /// </summary>
        public bool UserDeny { get; set; }
      
    }
}