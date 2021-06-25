namespace AgvcCoreData.Users
{
    /// <summary>
    /// 移动端用户授权后获取的信息
    /// </summary>
    public class AppUserInfo
    {
        /// <summary>
        /// 用户昵称
        /// </summary>
        public string nickName { get; set; }
        /// <summary>
        ///  头像
        /// </summary>
        public string avatarUrl { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public string gender { get; set; }
        /// <summary>
        /// 城市
        /// </summary>
        public string city { get; set; }
        /// <summary>
        /// 省份
        /// </summary>
        public string province { get; set; }
        /// <summary>
        /// 国家
        /// </summary>
        public string country { get; set; }
        public string unionId { get; set; }
    }
}