using System;
using AgvcCoreData.Users;
using CoreData.Core;

namespace AgvcEntitys.Users
{
    /// <summary>
    /// 用户的资产流水记录
    /// 如：金豆，积分
    /// </summary>
    public class AccountBalance : UEntity
    {
        /// <summary>
        /// 流水值
        /// </summary>
        public double BalanceValue { get; set; }
        /// <summary>
        /// 资产类型
        /// </summary>
        public BalanceType BalanceType { get; set; }
        /// <summary>
        /// 收支类型
        /// </summary>
        public BalancePaymentType  PaymentType { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Extra { get; set; }
        /// <summary>
        /// 充值渠道
        /// </summary>
        public IncomeSourceType IncomeSource { get; set; }
        /// <summary>
        /// 充值唯一KEY，防止重复充值
        /// </summary>
        public string IncomeUniqueKey { get; set; }
        /// <summary>
        /// 具体过期时间，过期后将自动删除
        /// 本字段将在Mongodb中建立ttl索引后有效。
        /// </summary>
        public DateTime? ExpireTime { get; set; }
         
    }
}