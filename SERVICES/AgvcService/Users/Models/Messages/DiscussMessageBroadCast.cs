using AgvcCoreData.Users;

namespace AgvcService.Users.Models.Messages
{
    public class DiscussMessageBroadCast: SignalMessageAggregate
    {
        public AgvcCoreData.Users.DiscussMessageModel Message { get; set; }
        public override LetterBox LetterBox { get; } = null;
    }
}