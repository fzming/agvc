namespace AgvcService.System.Message.Senders.Mail
{
    /// <summary>
    ///     邮件配置
    /// </summary>
    internal class MailConfig
    {
        /// <summary>
        ///     发送者显示名
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        ///     来源
        /// </summary>
        public string From { get; set; }

        /// <summary>
        ///     主机名 如：smtp.163.com
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        ///     端口号 如：25
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        ///     账户：用户名
        /// </summary>
        public string User { get; set; }

        /// <summary>
        ///     账户：密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        ///     是否启用SSL 默认：false
        ///     如果启用 端口号要改为加密方式发送的
        /// </summary>
        public bool EnableSsl { get; set; }
    }
}