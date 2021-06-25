using AgvcCoreData.System;
using AgvcCoreData.Users;
using CoreData.Core;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AgvcEntitys.Users
{
    /// <summary>
    /// 讨论组消息
    /// </summary>
    public class DiscussMessage:OEntity
    {
        /// <summary>
        /// 发送人员
        /// </summary>
        [BsonRepresentation(BsonType.ObjectId)]
        public string SenderId { get; set; }
        /// <summary>
        /// 讨论组名称
        /// </summary>
        public string Group { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 附件
        /// </summary>
        public AttachmentUrl Attachment { get; set; }
        /// <summary>
        /// 阅读或删除人员
        /// </summary>
        public UserMsgFlag[] UserFlags { get; set; }
        /// <summary>
        /// 是否是系统公告
        /// </summary>
        public bool IsAnnounce { get; set; }


    }
}