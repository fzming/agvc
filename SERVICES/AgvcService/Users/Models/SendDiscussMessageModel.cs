using System;
using CoreData;

namespace AgvcService.Users.Models
{
    /// <summary>
    /// 发送讨论组消息模型
    /// </summary>
    public class SendDiscussMessageModel
    {
        /// <summary>
        /// 讨论组名称
        /// </summary>
        public string Group { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 是否发送公告,
        /// 注意：只有系统机构人员才可以发送公告
        /// </summary>
        public bool IsAnnounce { get; set; }


    }
    /// <summary>
    /// 发送附件消息模型
    /// </summary>
    public class PostDiscussAttachmentModel : SendDiscussMessageModel
    {
        /// <summary>
        /// 附件
        /// </summary>
        public HttpContentFile File { get; set; }
        /// <summary>
        /// 上传配置
        /// </summary>
        public UploadOption Option { get; set; }
    }
   
}