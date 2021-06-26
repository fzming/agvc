using AgvcService.System.Message;
using CoreService.Interfaces;

namespace AgvcService.System
{
    public interface IMessageSendService : IService
    {
        IMessageSender GetSender(MessageTransport transport);
    }
}