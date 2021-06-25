using CoreData;

namespace AgvcCoreData.Users
{
    /// <summary>
    /// 分页查询讨论组消息
    /// </summary>
    public class DiscussMessagePageQuery:PageQuery
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 讨论组
        /// </summary>
        public string Group { get; set; }
        /// <summary>
        /// 仅查询新消息
        /// </summary>
        public bool OnlyNew { get; set; }
    }
}