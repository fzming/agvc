using System;
using Utility.Extensions;

namespace AgvcService.Users.Models.Messages
{
    /// <summary>
    ///     连接客户端
    /// </summary>
    public class ConnectionAgent
    {
        /// <summary>
        ///     客户ID
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        ///     连接ID
        /// </summary>
        public string ConnectionId { get; set; }

        /// <summary>
        ///     来源
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        ///     客户端Agent
        /// </summary>
        public string Agent { get; set; }

        /// <summary>
        ///     IP
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        ///     IP Addr
        /// </summary>
        public string IpAddr { get; set; }

        /// <summary>
        ///     连接时间
        /// </summary>
        public DateTime Time { get; set; }
    }

    public class ConnectionAgentUser : ConnectionAgent
    {
        public ConnectionAgentUser(ConnectionAgent p)
        {
            p.MapTo(this);
        }

        public ConnectionAgentUser()
        {
        }

        /// <summary>
        ///     用户名称
        /// </summary>
        public string Nick { get; set; }

        /// <summary>
        ///     头像
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        ///     所属机构
        /// </summary>
        public string OrgId { get; set; }
    }
}