using System;

namespace AgvcEntitys.Users
{
    /// <summary>
    ///     客户端连接
    /// </summary>
    public class ClientConnection
    {
        /// <summary>
        ///     连接ID
        /// </summary>
        /// <value>The connection identifier.</value>
        public string ConnectionId { get; set; }

        /// <summary>
        ///     是否已连接
        /// </summary>
        /// <value><c>true</c> if connected; otherwise, <c>false</c>.</value>
        public bool Connected { get; set; }

        /// <summary>
        ///     连接的设备标识
        /// </summary>
        /// <value>The agent.</value>
        public string Agent { get; set; }

        /// <summary>
        ///     连接时间
        /// </summary>
        /// <value>The connection time.</value>
        public DateTime ConnectStateChangeTime { get; set; }
    }
}