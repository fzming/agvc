using System.Threading.Tasks;
using CoreData;

namespace AgvcService.System.Message
{
    public interface IMessageSender
    {
        Task<Result<bool>> SendAsync<T>(T content, IMessageReceiver receiver) where T : IMessageContent;
    }
}