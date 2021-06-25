namespace AgvcService.System.Models
{
    /// <summary>
    /// 找回密码模型
    /// </summary>
    public class MobileValidateModel
    {
        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 短信验证码
        /// </summary>
        public SmsCodeType SmsCode { get; set; }
    }
    public class KickOffModel
    {
        public string ConnectionId { get; set; }
        public SmsCodeType SmsCode { get; set; }
    }
    /// <summary>
    /// 修改密码模型
    /// </summary>
    public class ReSetPasswordModel
    {
        /// <summary>
        /// 安全码
        /// </summary>
        public IdentSecurity Security { get; set; }
        /// <summary>
        /// 新密码
        /// </summary>
        public string NewPassword { get; set; }
    }
}