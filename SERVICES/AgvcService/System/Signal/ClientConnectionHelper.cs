using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using AgvcService.Users.Models.Messages;
using Microsoft.AspNet.SignalR;
using Utility.Extensions;
using UAParser;
namespace AgvcService.System.Signal
{
    public class ClientConnectionHelper
    {
        /// <summary>
        /// 存储用户ID和连接ID之间的关联
        /// </summary>
        private readonly ConnectionMapping<string, ConnectionAgent> Clients = new ConnectionMapping<string, ConnectionAgent>();
        /// <summary>
        /// 获取连接请求对象
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public RequestTuple GetRequestTuple(IRequest request)
        {
            #region ip

            var ip = GetIp(request);

            #endregion
            var qs = request.QueryString["qs"];
            var ns = HttpUtility.ParseQueryString(qs);
            var clientId = ns["client_id"];
            var clientNoKickoff = ns["client_no_kickoff"].EqualsIgnoreCase("true");
            var uaString = request.Headers.Get("User-Agent").ToStringEx();
            var source = ns["source"].ToStringEx();
            var uap = Parser.GetDefault();
            var userAgent = uap.ParseUserAgent(uaString);
            return new RequestTuple
            {
                client_id = clientId,
                agent = userAgent.Family,
                kickoff = !clientNoKickoff,
                ip = ip,
                source = source
            };
        }

        private string GetIp(IRequest request)
        {
            try
            {
                return request.GetRemoteIpAddress();
            }
            catch (Exception)
            {
                return string.Empty;
            }

        }
        public async Task SaveClientAsync(RequestTuple reqTuple, string connectionId,Action<string,string[]> prevConnectionAction)
        {
            var connection = new ConnectionAgent
            {
                Agent = reqTuple.agent,
                ConnectionId = connectionId,
                Time = DateTime.Now,
                ClientId = reqTuple.client_id,
                Ip = reqTuple.ip,
                Source = reqTuple.source,
                IpAddr = await NetworkHelper.IpLocationAsync(reqTuple.ip)
            };
            var prevConnections = Clients.GetConnections(reqTuple.client_id).ToList();
            if (prevConnections.Any(p => p.Source == reqTuple.source) && reqTuple.kickoff) //有同源旧客户端ID,并需要独占，踢掉其余客户端
            {
                //同源的同账号连接列表
                var prevConnectionIds = prevConnections.Where(p=>p.Source==reqTuple.source).Select(p => p.ConnectionId).ToArray();
                //await KickOffConnectionsAsync(prevConnectionIds);
                prevConnectionAction?.Invoke(reqTuple.client_id, prevConnectionIds);

            }

            Clients.Add(reqTuple.client_id, connection);

        }

        public void RemoveClientConnections(string clientId, string[] connectionIds)
        {
            Clients.Remove(clientId, connectionIds);
        }
        public void RemoveClientConnection(string clientId, string connectionId)
        {
            Clients.Remove(clientId, connectionId);
        }
        public IEnumerable<ConnectionAgent> GetConnections(IEnumerable<string> clients)
        {
            return Clients.GetConnections(clients.ToArray());
        } 
        public IEnumerable<ConnectionAgent> GetConnections(string clientId)
        {
            return Clients.GetConnections(clientId);
        }
        public string[] GetConnectionIds(IEnumerable<string> clients)
        {
            return Clients.GetConnections(clients.ToArray()).Select(p => p.ConnectionId).ToArray();
        }
        public string[] GetConnectionIds(string clientId)
        {
            var connections = Clients.GetConnections(clientId);
            return connections.Select(p => p.ConnectionId).ToArray();

        }
    }
}