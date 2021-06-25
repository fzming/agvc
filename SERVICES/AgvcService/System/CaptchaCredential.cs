namespace AgvcService.System
{
    /// <summary>
    /// 腾讯云验证码
    /// </summary>
    public class CaptchaCredential
    {
        /// <summary>
        /// AppId
        /// </summary>
        public long AppId { get; set; }
        public long CaptchaAppId { get; set; }
        /// <summary>
        /// SecretId
        /// </summary>
        public string SecretId { get; set; }
        public string SecretKey { get; set; }

        public string AppSecretKey { get; set; }
    }
}