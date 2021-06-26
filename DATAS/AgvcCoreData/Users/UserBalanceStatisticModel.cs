using AgvcCoreData.Users;

namespace AgvcService.Users.Models
{
    public class UserBalanceStatisticModel
    {
        public string Id { get; set; }
        public string Nick { get; set; }

        /// <summary>
        ///     总额
        /// </summary>
        public double Total { get; set; }

        public BalancePaymentType PaymentType { get; set; }
    }
}