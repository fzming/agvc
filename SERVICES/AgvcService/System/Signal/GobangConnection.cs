using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Utility.Extensions;

namespace AgvcService.System.Signal
{
    /// <summary>
    /// 五子棋连接服务
    /// </summary>
    public class GobangConnection : PersistentConnection
    {
        /// <summary>
        /// 存储用户ID和连接ID之间的关联
        /// </summary>
        private static readonly ClientConnectionHelper ClientHelper = new ClientConnectionHelper();
        protected override async Task OnConnected(IRequest request, string connectionId)
        {
            var p = ClientHelper.GetRequestTuple(request);
#if DEBUG
            Debug(p.client_id, connectionId, " is OnConnected");
#endif
            await _SaveClientAsync(p, connectionId);
            await base.OnConnected(request, connectionId);
        }

        private void Debug(params string[] messages)
        {
            global::System.Diagnostics.Trace.WriteLine(messages.JoinToString(" "));
        }

        /// <summary>Called when data is received from a connection.</summary>
        /// <param name="request">The <see cref="T:Microsoft.AspNet.SignalR.IRequest" /> for the current connection.</param>
        /// <param name="connectionId">The id of the connection sending the data.</param>
        /// <param name="data">The payload sent to the connection.</param>
        /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> that completes when the receive operation is complete.</returns>
        protected override async Task OnReceived(IRequest request, string connectionId, string data)
        {
            var requestTuple = ClientHelper.GetRequestTuple(request);
            var json = JsonConvert.DeserializeAnonymousType(data, new
            {
                fromId = "",
                toId = ""
            });

            if (requestTuple.client_id == json.toId) //自己发给自己，不允许
            {
                return;
            }
            var toConnections = ClientHelper.GetConnectionIds(json.toId);
            var sendTask = toConnections.Select(id => Connection.Send(id,
                data));
            await Task.WhenAll(sendTask);

        }

        protected override async Task OnDisconnected(IRequest request, string connectionId, bool stopCalled)
        {

            var p = ClientHelper.GetRequestTuple(request);
#if DEBUG
            Debug(p.client_id, connectionId, " is OnDisconnected");
#endif
            ClientHelper.RemoveClientConnection(p.client_id, connectionId);

            await base.OnDisconnected(request, connectionId, stopCalled);
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

        }

        private async Task _SaveClientAsync(RequestTuple reqTuple, string connectionId)
        {
            await ClientHelper.SaveClientAsync(reqTuple, connectionId, null);
        }

    }

}