using System;
using CoreData;

namespace AgvcCoreData.Users
{
    public class LetterBoxPageQuery : PageQuery
    {
        /// <summary>
        ///     信件分类,多个类别以逗号隔开
        /// </summary>
        public string category { get; set; }

        /// <summary>
        ///     限制已读标记：0 不限制，1:未读 2:已读
        /// </summary>
        public ReadFlagType readflag { get; set; }

        public DateTime? btm { get; set; }
        public DateTime? etm { get; set; }
    }
}