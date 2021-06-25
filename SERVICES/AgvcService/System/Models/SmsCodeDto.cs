using System;
using Newtonsoft.Json;

namespace AgvcService.System.Models
{
    /// <summary>
    /// 详细短信验证码结构
    /// </summary>
    public class SmsCodeDto:SmsCodeType
    {
        public SmsCodeDto()
        {
            Content = "您的验证码为：{0}";
            Expires = TimeSpan.FromMinutes(5);
        }
        public string Mobile { get; set; }

        public string Content { get; set; }
     
        public TimeSpan? Expires { get; set; }
        /// <summary>
        /// 格式化后的短信内容
        /// </summary>
        [JsonIgnore]
        public string FormattedContent => string.Format(Content, Code);

        public override string ToString()
        {
            return $"{Key}{Mobile}";
        }
    }

}