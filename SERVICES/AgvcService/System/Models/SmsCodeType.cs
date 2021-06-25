namespace AgvcService.System.Models
{
    /// <summary>
    /// 基本短信验证码结构
    /// </summary>
    public class SmsCodeType
    {
        /// <summary>
        /// 初始化短信验证码
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="code">验证码</param>
        public SmsCodeType(string key, string code)
        {
            Code = code;
            Key = key;
        }
        /// <summary>
        /// 无参数构造函数
        /// </summary>
        public SmsCodeType()
        {
        }
        /// <summary>
        /// 短信验证码KEY
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 短信验证码[获取验证码时无需输入]
        /// </summary>

        public string Code { get; set; }


    }
}