namespace AgvcService.System.Models
{
    /// <summary>
    /// 系统用户登录模型
    /// </summary>
    public class SystemUserLoginModel
    {
         public string LoginId { get; set; }
         public string LoginPwd { get; set; }
         public string VaptchaToken { get; set; }
         public string LoginType { get; set; }
    }
}