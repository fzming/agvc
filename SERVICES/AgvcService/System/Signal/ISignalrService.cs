using System.Collections.Generic;
using System.Threading.Tasks;
using AgvcService.Users.Models.Messages;
using CoreService.Interfaces;

namespace AgvcService.System.Signal
{
    /// <summary>
    /// Signalr service Interface.
    /// </summary>
    public interface ISignalrService : IService
    {

        string[] GetConnectionIds(string userid);
        string[] GetConnectionIds(IEnumerable<string> clients);
        /// <summary>
        /// Sends to async.
        /// </summary>
        /// <returns>The to async.</returns>
        /// <param name="userid">Userid.</param>
        /// <param name="content">Content.</param>
        /// <typeparam name="TMessage">The 1st type parameter.</typeparam>

        Task<bool> SendMessageAsync<TMessage>(string userid, TMessage content) where TMessage : SignalMessageAggregate;
        Task<bool> SendMessageWithPolicyAsync<TMessage>(string userid, TMessage content) where TMessage : SignalMessageAggregate;

        /// <summary>
        /// Broadcasts the message async.
        /// </summary>
        /// <returns>The message async.</returns>
        /// <param name="content">Content.</param>
        /// <param name="excludeConnectionIds">排除广播的用户ID</param>
        /// <typeparam name="TMessage">The 1st type parameter.</typeparam>
        Task<bool> BroadcastMessageAsync<TMessage>(TMessage content, params string[] excludeConnectionIds) where TMessage : SignalMessageAggregate;

        /// <summary>
        /// 在机构范围内进行广播
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="content"></param>
        /// <param name="orgId">机构ID</param>
        /// <param name="clients">指定广播的用户ID</param>
        /// <returns></returns>
        Task<bool> BroadcastUsersMessageAsync<TMessage>(TMessage content, string orgId,
            IEnumerable<string> clients)
            where TMessage : SignalMessageAggregate;
        /// <summary>
        /// 在机构范围内进行广播
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="content"></param>
        /// <param name="orgId">机构ID</param>
        /// <param name="excludeClientIds">排除广播的用户ID</param>
        /// <returns></returns>
        Task<bool> BroadcastMessageAsync<TMessage>(TMessage content, string orgId, params string[] excludeClientIds) where TMessage : SignalMessageAggregate;

        /// <summary>
        /// 将连接ID强制下线
        /// </summary>
        /// <param name="connectionId"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        Task<bool> KickOffConnectionAsync(string connectionId,string message);
        Task<IEnumerable<ConnectionAgentUser>> QueryConnectionsAsync();
        Task<IEnumerable<ConnectionAgentUser>> QueryConnectionsAsync(string orgId);
        Task KickOffUserAsync(string userid, string message);
    }
}