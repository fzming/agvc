using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgvcService.Users.Models.Messages;
using Microsoft.AspNet.SignalR;
using Utility.Extensions;

namespace AgvcService.System.Signal
{
    /// <summary>
    /// Signalr 持久连接类.
    /// </summary>
    public class SignalrConnection : PersistentConnection
    {
        /// <summary>
        /// 存储用户ID和连接ID之间的关联
        /// </summary>
        public static readonly ClientConnectionHelper ClientHelper = new ClientConnectionHelper();

        // 用户安全验证： https://docs.microsoft.com/en-us/aspnet/signalr/overview/security/persistent-connection-authorization
        // protected override bool AuthorizeRequest(IRequest request)
        // {
        //     return request.User.Identity.IsAuthenticated;
        // }
        protected override async Task OnConnected(IRequest request, string connectionId)
        {
        
            var p = ClientHelper.GetRequestTuple(request);
#if DEBUG
            Debug(p.client_id, connectionId, " is OnConnected");
#endif
            await _SaveClientAsync(p, connectionId);
            await base.OnConnected(request, connectionId);

            await NotifyAgentChangeAsync(p.client_id);
        }

        private void Debug(params string[] messages)
        {
            global::System.Diagnostics.Trace.WriteLine(messages.JoinToString(" "));
        }
        /// <summary>
        /// 通知客户端连接数改变
        /// </summary>
        /// <param name="clientId">客户端ID</param>
        /// <param name="excludeConnectionIds">要排除发送的连接ID</param>
        /// <returns></returns>
        private Task NotifyAgentChangeAsync(string clientId, params string[] excludeConnectionIds)
        {
            var connectionIds = ClientHelper
                .GetConnectionIds(clientId)
                .Except(excludeConnectionIds)
                .ToArray();
            if (connectionIds.AnyNullable())
            {
                return Connection.Send(connectionIds, new AgentsChangeMessage
                {
                    Title = "AgentsChange",
                    ConnectionAgents = ClientHelper.GetConnections(clientId).ToList()
                });
            }
            return Task.CompletedTask;

        }

        protected override async Task OnDisconnected(IRequest request, string connectionId, bool stopCalled)
        {

            var p = ClientHelper.GetRequestTuple(request);
#if DEBUG
            Debug(p.client_id, connectionId, " is OnDisconnected");
#endif
            ClientHelper.RemoveClientConnection(p.client_id, connectionId);

            await base.OnDisconnected(request, connectionId, stopCalled);

            await NotifyAgentChangeAsync(p.client_id, connectionId);
        }

        protected override async Task OnReconnected(IRequest request, string connectionId)
        {
            var requestTuple = ClientHelper.GetRequestTuple(request);
#if DEBUG
            Debug(requestTuple.client_id, connectionId, " is OnReconnected");
#endif
            if (ClientHelper.GetConnections(requestTuple.client_id).Any(p => p.ConnectionId != connectionId))
            {
                await _SaveClientAsync(requestTuple, connectionId);
            }

            await base.OnReconnected(request, connectionId);
            await NotifyAgentChangeAsync(requestTuple.client_id);
        }

        private async Task _SaveClientAsync(RequestTuple reqTuple, string connectionId)
        {
            await ClientHelper.SaveClientAsync(reqTuple, connectionId, async (clientId, prevConnections) =>
            {
                await KickOffConnectionsAsync(prevConnections);
                ClientHelper.RemoveClientConnections(clientId, prevConnections);
            });
        }


        /// <summary>
        /// 强制下线
        /// </summary>
        /// <param name="connectionIds"></param>
        /// <returns></returns>
        public async Task KickOffConnectionsAsync(IEnumerable<string> connectionIds)
        {
            var kickOffTasks = connectionIds.Select(prevId => Connection.Send(prevId,
                new KickOutMessage
                {
                    Title = "强制下线",
                    Message = "您的账户已在别处登录,此登陆被服务器强制下线，如果不是您的操作，请及时修改密码！"
                }));
            await Task.WhenAll(kickOffTasks);

        }

    }
}