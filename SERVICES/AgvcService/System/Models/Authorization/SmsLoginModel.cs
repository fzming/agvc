using AgvcCoreData.Users;

namespace AgvcService.System.Models.Authorization
{
    public abstract class CoreLoginModel
    {
        public AppOpenIdentify Identify { get; set; }
        public AppUserInfo AppUserInfo { get; set; }
        public string LoginDomain { get; set; }
        public string LoginType { get; set; }
    }

    public class SmsLoginModel : CoreLoginModel
    {
        public string Mobile { get; set; }
        public string SmsCode { get; set; }
        public string SmsKey { get; set; }

        public bool JustLogin { get; set; }
    }

    public class PasswordLoginModel : CoreLoginModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string VaptchaToken { get; set; }
    }
}