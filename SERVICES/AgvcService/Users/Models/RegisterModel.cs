using AgvcService.System.Models;

namespace AgvcService.Users.Models
{
    public class RegisterModel
    {
        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 短信验证码
        /// </summary>
        public SmsCodeType SmsCode { get; set; }
        /// <summary>
        /// 注册来源设备
        /// </summary>
        public string Device { get; set; }
    }

   
}