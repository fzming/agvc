using System;
using AgvcCoreData.System;
using CoreData.Core;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AgvcEntitys.Users
{
    /// <summary>
    ///     消息标记
    /// </summary>
    public enum MessageFlag
    {
        已读 = 1,
        已删 = 2
    }

    /// <summary>
    ///     用户阅读或删除标记
    /// </summary>
    public class UserMsgFlag
    {
        public string Id { get; set; }
        public DateTime Time { get; set; }
        public MessageFlag Flag { get; set; }
    }

    /// <summary>
    ///     讨论组消息
    /// </summary>
    public class DiscussMessage : OEntity
    {
        /// <summary>
        ///     发送人员
        /// </summary>
        [BsonRepresentation(BsonType.ObjectId)]
        public string SenderId { get; set; }

        /// <summary>
        ///     讨论组名称
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        ///     内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        ///     附件
        /// </summary>
        public AttachmentUrl Attachment { get; set; }

        /// <summary>
        ///     阅读或删除人员
        /// </summary>
        public UserMsgFlag[] UserFlags { get; set; }

        /// <summary>
        ///     是否是系统公告
        /// </summary>
        public bool IsAnnounce { get; set; }
    }
}