using System.Collections.Generic;
using AgvcCoreData.Users;

namespace AgvcService.Users.Models.Messages
{
    public class AgentsChangeMessage : SignalMessageAggregate
    {
        /// <summary>
        ///     是否将消息存入收件箱
        /// </summary>
        public override LetterBox LetterBox { get; } = null;

        public List<ConnectionAgent> ConnectionAgents { get; set; }
    }
}