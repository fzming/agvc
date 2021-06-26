using System.Linq;
using Utility.Extensions;

namespace AgvcService.System.Message.Address
{
    public class MailReceiver : IMessageReceiver
    {
        /// <summary>
        ///     接收者邮箱（多个用英文“,”号分割）
        /// </summary>
        public string Receiver { get; set; }

        /// <summary>
        ///     抄送人集合
        /// </summary>
        public string CC { get; set; }

        /// <summary>
        ///     回复地址
        /// </summary>
        public string Replay { get; set; }

        public bool Validate()
        {
            return Receiver.Split(',').All(p => p.IsEmail());
        }
    }
}