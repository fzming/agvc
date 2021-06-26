using System;
using AgvcCoreData.Users;

namespace AgvcService.Users.Models.Messages
{
    /// <summary>
    ///     被服务器强制下线
    /// </summary>
    public class KickOutMessage : SignalMessageAggregate
    {
        /// <summary>
        ///     强制下线原因
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        ///     踢出时间
        /// </summary>
        public DateTime KickTime { get; set; } = DateTime.Now;

        public override LetterBox LetterBox => null; //服务器下线提示不需要进入收件箱
    }
}