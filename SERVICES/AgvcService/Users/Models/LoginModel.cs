namespace AgvcService.Users.Models
{
    public class LoginModel
    {
        /// <summary>
        /// 手机号或邮件
        /// </summary>
        public string LoginId { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string LoginPassword { get; set; }
        /// <summary>
        /// 登录设备
        /// </summary>
        public string Device { get; set; }
    }
}