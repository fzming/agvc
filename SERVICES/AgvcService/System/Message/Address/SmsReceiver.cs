using System.Linq;
using Utility.Extensions;

namespace AgvcService.System.Message.Address
{
    public class SmsReceiver : IMessageReceiver
    {
        /// <summary>
        ///     多个手机号请使用逗号进行分割
        /// </summary>
        public string Mobile { get; set; }

        public bool Validate()
        {
            return Mobile.FindMobiles().Any();
        }
    }
}