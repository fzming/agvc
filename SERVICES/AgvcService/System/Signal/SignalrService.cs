using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AgvcService.System.Signal;
using AgvcService.Users;
using AgvcService.Users.Models.Messages;
using CoreService;
using Microsoft.AspNet.SignalR;
using MongoDB.Driver.Core.Connections;
using Polly;
using Utility.Extensions;
using WeiMan.Common.Interface.Extensions;
using WeiMan.Datas.Core.Messages;
using WeiMan.Service.Exports.Core;
using WeiMan.Service.Interface.Com.Signal;
using WeiMan.Service.Interface.Users;

namespace WeiMan.Service.Exports.Com.Signal
{
    /// <summary>
    /// Signalr service.
    /// </summary>
    public class SignalrService : AbstractService, ISignalrService
    {
        #region 注入依赖

        private IUserLetterBoxService UserLetterBoxService { get; }
        private IAccountService AccountService { get; }

        [ImportingConstructor]
        public SignalrService(IUserLetterBoxService userLetterBoxService, IAccountService accountService)
        {
            UserLetterBoxService = userLetterBoxService;
            AccountService = accountService;
        }

        #endregion

        private IConnection Connection => GlobalHost.ConnectionManager.GetConnectionContext<SignalrConnection>().Connection;

        public async Task<bool> SendMessageWithPolicyAsync<TMessage>(string userid, TMessage content) where TMessage : SignalMessageAggregate
        {
            var retryPolicy = Policy
                 .Handle<ConnectionIdNotFoundException>()
                 .WaitAndRetryAsync(1,
                     retryAttempt => TimeSpan.FromSeconds(1),
                     (exception, timespan) =>
                 {
                     Trace.WriteLine($"重试消息发送(间隔：{timespan})");
                 });
            try
            {
                return await retryPolicy.ExecuteAsync(async () =>
                {
                    //获取用户所有连接
                    var connectionIds = SignalrConnection.ClientHelper.GetConnectionIds(userid);
                    if (!connectionIds.Any())
                    {
                        throw new ConnectionIdNotFoundException();
                    }

                    bool hasSend;
                    try
                    {
                        //Sends an object that will be JSON serialized asynchronously over the connection.
                        var tasks = connectionIds.Select(p => Connection.Send(p, content));
                        await Task.WhenAll(tasks);
                        hasSend = true;
                    }
                    catch (Exception)
                    {
                        hasSend = false;
                    }

                    #region 同时将信息加入收件箱

                    var letter = content.LetterBox;
                    if (letter != null)
                    {
                        await UserLetterBoxService.SendUserLetterBoxAsync(userid, letter, hasSend);
                    }

                    #endregion

                    return hasSend;

                }).ConfigureAwait(false);
            }
            catch (Exception)
            {
                //ig
            }

            return false;
        }

        public string[] GetConnectionIds(string userid)
        {
            //获取用户所有连接
            return SignalrConnection.ClientHelper.GetConnectionIds(userid);

        }
        public string[] GetConnectionIds(IEnumerable<string> clients)
        {
            return SignalrConnection.ClientHelper.GetConnectionIds(clients);
        }
        public async Task<bool> SendMessageAsync<TMessage>(string userid, TMessage content) where TMessage : SignalMessageAggregate
        {

            try
            {
                //获取用户所有连接
                var connections = SignalrConnection.ClientHelper.GetConnectionIds(userid);
                var hasSend = false;
                if (connections.Any())
                {
                    try
                    {
                        //Sends an object that will be JSON serialized asynchronously over the connection.
                        await Connection.Send(connections, content);

                        hasSend = true;
                    }
                    catch (Exception)
                    {
                        hasSend = false;
                    }
                }

                #region 同时将信息加入收件箱

                var letter = content.LetterBox;
                if (letter != null)
                {
                    await UserLetterBoxService.SendUserLetterBoxAsync(userid, letter);
                }

                #endregion

                return hasSend;


            }
            catch (Exception)
            {
                //ig
            }

            return false;
        }

        public async Task<bool> BroadcastMessageAsync<TMessage>(TMessage content, params string[] excludeConnectionIds) where TMessage : SignalMessageAggregate
        {
            try
            {
                await Connection.Broadcast(content, excludeConnectionIds);
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
        /// <summary>
        /// 在机构范围内进行广播
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="content"></param>
        /// <param name="orgId">机构ID</param>
        /// <param name="clients">指定广播的用户ID</param>
        /// <returns></returns>
        public async Task<bool> BroadcastUsersMessageAsync<TMessage>(TMessage content, string orgId,
            IEnumerable<string> clients)
            where TMessage : SignalMessageAggregate
        {
            var connectionIds = GetConnectionIds(clients);

            if (connectionIds.Any())
            {
                try
                {
                    await Connection.Send(connectionIds, content);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// 在机构范围内进行广播
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="content"></param>
        /// <param name="orgId">机构ID</param>
        /// <param name="excludeClientIds">排除广播的用户ID</param>
        /// <returns></returns>
        public async Task<bool> BroadcastMessageAsync<TMessage>(TMessage content, string orgId, params string[] excludeClientIds)
        where TMessage : SignalMessageAggregate
        {
            #region 非机构广播

            if (orgId.IsNullOrEmpty())
            {
                throw new Exception("机构ID不能为空");
            }

            #endregion
            var users = await AccountService.QueryAccountUsersAsync(orgId);
            var clients = users.GroupBy(p => p.Id).Select(p => p.Key);
            if (excludeClientIds.Any())
            {
                clients = clients.Except(excludeClientIds);
            }

            return await BroadcastUsersMessageAsync(content, orgId, clients);
        }

        /// <summary>
        /// 将连接ID强制下线
        /// </summary>
        /// <param name="connectionId"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<bool> KickOffConnectionAsync(string connectionId, string message)
        {
            var kickOffMessage = new KickOutMessage
            {
                Title = "强制下线",
                Message = message
            };

            try
            {
                await Connection.Send(connectionId, kickOffMessage);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public Task<IEnumerable<ConnectionAgentUser>> QueryConnectionsAsync()
        {
            return QueryConnectionsAsync(string.Empty);
        }

        public async Task<IEnumerable<ConnectionAgentUser>> QueryConnectionsAsync(string orgId)
        {
            var accounts = (await AccountService.QueryAccountUsersAsync(orgId)).ToList();
            var clientIds = accounts.Select(p => p.Id);
            var agents = SignalrConnection.ClientHelper.GetConnections(clientIds);
            return agents.Select(p =>
            {
                var ac = accounts.FirstOrDefault(k => k.Id == p.ClientId);
                if (ac == null)
                {
                    return null;
                }

                return new ConnectionAgentUser(p)
                {
                    Nick = ac.Nick,
                    Avatar = ac.Avatar,
                    OrgId = ac.OrgId
                };
            });
        }

        /// <summary>
        /// 将用户强制下线
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task KickOffUserAsync(string userid, string message)
        {
            var connectionIds = GetConnectionIds(userid);
            if (connectionIds.AnyNullable())
            {
                var kickOffTasks = connectionIds.Select(id => KickOffConnectionAsync(id, message));
                await Task.WhenAll(kickOffTasks);
            }
        }
    }
}