using AgvcCoreData.Users;

namespace AgvcService.Users.Models.Messages
{
    /// <summary>
    /// 支付成功消息通知
    /// </summary>
    public class PayPaySucceedMessage : SignalMessageAggregate
    {
        public string OrderId { get; set; }
        public string Message { get; set; }

        public override LetterBox LetterBox=>new LetterBox
        {
            Title = "订单支付成功提示",
            Content = Message,
            Category = "充值支付",
            Icon = "pay"
        };
    }
}