using AgvcCoreData.Users;

namespace AgvcService.Users.Models
{
    /// <summary>
    ///     我推荐注册的用户模型
    /// </summary>
    public class ReferrerUserDto
    {
        /// <summary>
        ///     用户ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///     绑定邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        ///     绑定手机号
        /// </summary>
        public string Mobile { get; set; }

        public AppUserInfo AppUserInfo { get; set; }
    }
}