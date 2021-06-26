using System;
using System.ComponentModel;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AgvcService.System.Message.Address;
using AgvcService.System.Message.Contents;
using CoreData;
using Microsoft.Extensions.Configuration;
using Utility.Extensions;

namespace AgvcService.System.Message.Senders.Mail
{
    public class MailSender : IMessageSender
    {
        public MailSender(IConfiguration configuration)
        {
            Config = configuration.GetSection("Mail").Get<MailConfig>();
        }

        private MailConfig Config { get; }

        public MessageTransport Ttransport => MessageTransport.Mail;

        public async Task<Result<bool>> SendAsync<T>(T content, IMessageReceiver receiver) where T : IMessageContent
        {
            using var message = new MailMessage();
            var m = content as MailContent;
            message.From = new MailAddress(Config.From, Config.DisplayName);
            message.Subject = m?.Subject;

            #region Encoding&Priority

            message.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
            message.BodyEncoding = Encoding.UTF8;
            message.SubjectEncoding = Encoding.UTF8;
            //message.Priority = MailPriority.High;

            #endregion

            if (!(receiver is MailReceiver mailReceiver)) throw new ArgumentNullException(nameof(mailReceiver));
            if (!string.IsNullOrEmpty(mailReceiver.Replay))
                message.ReplyToList.Add(new MailAddress(mailReceiver.Replay));
            if (m != null)
            {
                message.Body = m.Body;
                message.IsBodyHtml = m.IsBodyHtml;
            }

            //邮件附件
            if (m.Attachments.AnyNullable())
                foreach (var attachment in m.Attachments)
                    message.Attachments.Add(attachment);
            //抄送
            if (!string.IsNullOrEmpty(mailReceiver.CC))
            {
                var ccs = mailReceiver.CC.Split(',');
                foreach (var cc in ccs)
                {
                    if (!cc.IsEmail()) continue;
                    message.CC.Add(cc);
                }
            }

            var receivers = mailReceiver.Receiver.Split(',');
            foreach (var r in receivers) message.To.Add(new MailAddress(r));

            try
            {
                var smtpClient = new SmtpClient
                {
                    UseDefaultCredentials = false,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Host = Config.Host,
                    Port = Config.Port,
                    Credentials = new NetworkCredential(Config.User, Config.Password),
                    EnableSsl = Config.EnableSsl
                };

                using var smtpClientWrapper = new SmtpClientWrapper(smtpClient);
                await smtpClientWrapper.SendMailAsync(message, new CancellationToken());
                // smtpClient.SendCompleted += Client_SendCompleted;
                // await smtpClient.SendMailAsync(message).ConfigureAwait(false);


                return Result<bool>.Successed;
            }
            catch (Exception e)
            {
                /*
                 *如果: e.StatusCode是GeneralFailure

                  解释是：事务未能发生。 当未能找到指定的 SMTP 主机时，会收到此错误。
                 *
                 */
                return Result<bool>.Fail((e.InnerException ?? e).ToJson() + "\n[Mail Config]\n" + Config.ToJson());
            }
        }

        private void Client_SendCompleted(object sender, AsyncCompletedEventArgs e)
        {
            try
            {
                var callbackClient = sender as SmtpClient;
                var callbackMailMessage = e.UserState as MailMessage;
                callbackClient?.Dispose();
                callbackMailMessage?.Dispose();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}