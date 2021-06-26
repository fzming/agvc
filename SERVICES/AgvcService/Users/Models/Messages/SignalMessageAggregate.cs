using AgvcCoreData.Users;
using Newtonsoft.Json;

namespace AgvcService.Users.Models.Messages
{
    public abstract class SignalMessageAggregate
    {
        protected SignalMessageAggregate()
        {
            MessageId = GetType().Name;
        }

        /// <summary>
        ///     ��Ϣ����
        /// </summary>
        [JsonProperty("_message_title")]
        public string Title { get; set; }

        /// <summary>
        ///     ��Ϣ����
        /// </summary>
        [JsonProperty("_message_content")]
        public string Content { get; set; }

        /// <summary>
        ///     �Զ��رպ�����
        /// </summary>
        [JsonProperty("_message_duration")]
        public int Duration { get; set; }

        /// <summary>
        ///     ��ϢID
        /// </summary>
        [JsonProperty("_message_id")]
        private string MessageId { get; }

        /// <summary>
        ///     �Ƿ���Ϣ�����ռ���
        /// </summary>
        public abstract LetterBox LetterBox { get; }
    }
}