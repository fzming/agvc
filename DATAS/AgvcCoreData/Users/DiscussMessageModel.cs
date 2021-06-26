using System;
using AgvcCoreData.System;
using CoreData;
using Newtonsoft.Json;

namespace AgvcCoreData.Users
{
    public class DiscussMessageModel
    {
        public string Id { get; set; }
        /// <summary>
        /// 发送人员
        /// </summary>
        public UserIdentity Sender { get; set; }
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
        /// 是否已读
        /// </summary>
        public bool IsRead { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [JsonProperty("_c")]
        public  DateTime CreatedOn { get; set; }
    }
}