using CoreData.Core;

namespace AgvcEntitys.Users
{
    /// <summary>
    ///     用户收件箱
    /// </summary>
    public class UserLetterBox : UEntity
    {
        /// <summary>
        ///     信箱标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     信件内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        ///     分类
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        ///     图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        ///     已读标记
        /// </summary>
        public bool Read { get; set; }
    }
}