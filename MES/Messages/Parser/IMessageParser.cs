using Messages.Transfers.Core;
using Utility;

namespace Messages.Parser
{
    public interface IMessageParser : ISingletonDependency
    {
        IMessage Parse(string message);
    }
}