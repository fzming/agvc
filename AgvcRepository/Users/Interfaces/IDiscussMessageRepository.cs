using System.Collections.Generic;
using System.Threading.Tasks;
using AgvcCoreData.Users;
using AgvcEntitys.Users;
using CoreData;
using CoreRepository;

namespace AgvcRepository.Users.Interfaces
{
    public interface IDiscussMessageRepository : IRepository<DiscussMessage>
    {
        Task<bool> SetFlagAsync(string messageId, string userId, MessageFlag flag);
        Task<PageResult<DiscussMessageModel>> QueryGroupMessagesAsync(string orgId, DiscussMessagePageQuery query);
        Task<Dictionary<string, int>> GetUnReadMessageCountAsync(string orgId, string clientId, string[] groups);
        Task<IdentityUser> GetSenderAsync(string clientId, bool isSys);
    }
}