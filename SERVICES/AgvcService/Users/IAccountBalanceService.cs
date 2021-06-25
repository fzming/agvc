using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AgvcCoreData.Users;
using AgvcEntitys.Users;
using AgvcService.Users.Models;
using CoreData;
using CoreService.Interfaces;

namespace AgvcService.Users
{
    /// <summary>
    /// 账户余额服务
    /// </summary>
    public interface IAccountBalanceService:IService
    {
        /// <summary>
        /// 获取指定类型的余额总剩余
        /// </summary>
        /// <returns>The balance total async.</returns>
        /// <param name="userid">Userid.</param>
        /// <param name="balanceType">Balance type.</param>
        Task<double> GetBalanceTotalAsync(string userid, BalanceType balanceType);
        Task<double> GetIncomeTotalAsync(string userid, BalanceType balanceType);
        Task<double> GetExpenseTotalAsync(string userid, BalanceType balanceType);
        Task<IEnumerable<UserBalanceStatisticModel>> UserBalanceStatisticAsync();

        /// <summary>
        /// 增加余额
        /// </summary>
        /// <returns>The balance async.</returns>
        /// <param name="userid">Userid.</param>
        /// <param name="balanceValue">Balance value.</param>
        /// <param name="balanceType">Balance type.</param>
        /// <param name="incomeSourceType">充值渠道</param>
        /// <param name="incomeUniqueKey">防止重复充值KEY</param>
        /// <param name="expireTime">过期时间(可选)，指定到达过期时间后将自动删除</param>
        Task<bool> IncomeBalanceAsync(string userid, double balanceValue, BalanceType balanceType,
            IncomeSourceType incomeSourceType = IncomeSourceType.None, 
            string incomeUniqueKey = "", 
            DateTime? expireTime = null);
        /// <summary>
        /// 支出余额
        /// </summary>
        /// <returns>The balance async.</returns>
        /// <param name="userid">Userid.</param>
        /// <param name="balanceValue">Balance value.</param>
        /// <param name="balanceType">Balance type.</param>
        Task<bool> ExpenseBalanceAsync(string userid, double balanceValue, BalanceType balanceType);
        /// <summary>
        /// 查询余额收支记录
        /// </summary>
        /// <returns>The balance logs async.</returns>
        /// <param name="clientId">Client identifier.</param>
        /// <param name="balanceType">Balance type.</param>
        /// <param name="query">Query.</param>
        Task<PageResult<AccountBalance>> QueryBalanceLogsAsync(string clientId, BalanceType balanceType, BalanceLogQuery query);

        /// <summary>
        /// 是否包含指定充值渠道的充值记录
        /// </summary>
        /// <param name="clientId">客户ID</param>
        /// <param name="balanceType">资产类型</param>
        /// <param name="incomeSourceType">充值渠道类型</param>
        /// <returns></returns>
        Task<IEnumerable<AccountBalance>> QueryIncomeSourceAsync(string clientId, BalanceType balanceType, IncomeSourceType incomeSourceType);
    }
}