namespace AgvcService.Users.Models
{
    public class CoreUserInfo
    {
        /// <summary>
        /// 人员ID
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 绑定邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 绑定手机号
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string ContactPhone { get; set; }
        /// <summary>
        /// 用户昵称
        /// </summary>
        public string Nick { get; set; }
        /// <summary>
        /// 头像地址
        /// </summary>
        public string Avatar { get; set; }
    }
}