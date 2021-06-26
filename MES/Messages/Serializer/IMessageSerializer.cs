using Messages.Transfers.Core;
using Utility;

namespace Messages.Parser
{
    public interface IMessageSerializer : ISingletonDependency
    {
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        IMessage Deserialize(string message);
        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        string Serialize(IMessage message);
    }
}