using System.Threading.Tasks;
using CoreData;
using Utility;

namespace AgvcService.System.Message
{
    public interface IMessageSender : IScopeDependency
    {
        MessageTransport Ttransport { get; }
        Task<Result<bool>> SendAsync<T>(T content, IMessageReceiver receiver) where T : IMessageContent;
    }
}