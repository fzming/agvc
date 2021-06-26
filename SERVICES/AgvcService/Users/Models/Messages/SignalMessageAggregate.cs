using AgvcCoreData.Users;
using Newtonsoft.Json;

namespace AgvcService.Users.Models.Messages
{
    public abstract class SignalMessageAggregate
    {
        /// <summary>
        /// 消息标题
        /// </summary>
        [JsonProperty("_message_title")]
        public string Title { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary>
        [JsonProperty("_message_content")]
        public string Content { get; set; }
        /// <summary>
        /// 自动关闭毫秒数
        /// </summary>
        [JsonProperty("_message_duration")]
        public int Duration { get; set; }
        protected SignalMessageAggregate()
        {
            MessageId = GetType().Name;
        }
        /// <summary>
        /// 消息ID
        /// </summary>
        [JsonProperty("_message_id")]
        private string MessageId { get; }

        /// <summary>
        /// 是否将消息存入收件箱
        /// </summary>
        public abstract LetterBox LetterBox { get; }

    }
}