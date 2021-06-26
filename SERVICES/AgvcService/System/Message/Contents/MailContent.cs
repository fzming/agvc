using System.Net.Mail;

namespace AgvcService.System.Message.Contents
{
    /// <summary>
    /// 邮件实体
    /// </summary>
    public class MailContent : IMessageContent
    {
        /// <summary>
        /// 主题行
        /// </summary>
        private string _subject;

        /// <summary>
        /// 邮件的主题行
        /// </summary>
        public string Subject
        {
            get
            {
                if (string.IsNullOrEmpty(_subject) && _subject.Length > 15)
                {
                    return Body.Substring(0, 15);
                }
                return _subject;
            }
            set => _subject = value;
        }

        /// <summary>
        /// 正文内容
        /// </summary>
        public string Body { get; set; }
        /// <summary>
        /// 正文是否是HTML格式
        /// </summary>
        public bool IsBodyHtml { get; set; }
        /// <summary>
        /// 邮件附件
        /// </summary>
        public Attachment[] Attachments { get; set; }
    }

}