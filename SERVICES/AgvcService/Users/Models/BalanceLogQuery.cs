using System;
using CoreData;

namespace AgvcService.Users.Models
{
    /// <summary>
    ///     余额收支记录查询
    /// </summary>
    public class BalanceLogQuery : PageQuery
    {
        public int paymentType { get; set; }
        public DateTime? btm { get; set; }
        public DateTime? etm { get; set; }
    }
}