namespace AgvcService.System.Message.Contents
{
    public class SmsContent : IMessageContent
    {
        /// <summary>
        ///     消息内容
        /// </summary>
        public string Content { get; set; }
    }
}