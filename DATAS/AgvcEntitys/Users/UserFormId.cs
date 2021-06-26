using System;
using CoreData.Core;

namespace AgvcEntitys.Users
{
    /// <summary>
    ///     微信用户FormId搜集
    /// </summary>
    public class UserFormId : MongoEntity
    {
        public string AppId { get; set; }
        public string OpenId { get; set; }
        public string FormId { get; set; }
        public DateTime Expires { get; set; }
    }
}