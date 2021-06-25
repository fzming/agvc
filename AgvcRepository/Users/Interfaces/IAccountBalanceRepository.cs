using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AgvcCoreData.Users;
using AgvcEntitys.Users;
using AgvcService.Users.Models;
using CoreData;
using CoreRepository;

namespace AgvcRepository.Users.Interfaces
{
    public interface IAccountBalanceRepository: IRepository<AccountBalance>
    {
       
        /// <summary>
        /// 查询资产日志
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="balanceType"></param>
        /// <returns></returns>
        Task<IEnumerable<AccountBalance>> QueryAsync(string userid, BalanceType balanceType);
        Task<PageResult<AccountBalance>> AdvanceQueryAsync(string clientId, BalanceType balanceType, int paymentType, DateTime? btm, DateTime? etm, int pageSize, int pageIndex);
        /// <summary>
        /// 获取指定类型的总和
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="balanceType"></param>
        /// <returns></returns>
        Task<double> GetTotalAsync(string userid,BalanceType balanceType);
        Task<double> GetIncomeTotalAsync(string userid,BalanceType balanceType);
        Task<double> GetExpenseTotalAsync(string userid,BalanceType balanceType);

        /// <summary>
        /// 收入
        /// </summary>
        /// <param name="userid">用户ID</param>
        /// <param name="balanceValue">收入数量</param>
        /// <param name="balanceType">资产类型</param>
        /// <param name="incomeSourceType">充值渠道</param>
        /// <param name="incomeUniqueKey"></param>
        /// <param name="expireTime">过期时间(可选)，指定到达过期时间后将自动删除</param>
        /// <returns></returns>
        Task<bool> IncomeBalanceAsync(string userid, double balanceValue, BalanceType balanceType,
            IncomeSourceType incomeSourceType = IncomeSourceType.None, string incomeUniqueKey="",DateTime? expireTime = null);

        /// <summary>
        /// 支出
        /// </summary>
        /// <param name="userid">用户ID</param>
        /// <param name="balanceValue">支出数量</param>
        /// <param name="balanceType">资产类型</param>
        /// <returns></returns>
        Task<bool> ExpenseBalanceAsync(string userid, double balanceValue, BalanceType balanceType);
        /// <summary>
        /// 用户油滴收支统计
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<UserBalanceStatisticModel>> UserBalanceStatisticAsync();

        /// <summary>
        /// 清除收入的过期标记
        /// </summary>
        /// <param name="incomes"></param>
        /// <returns></returns>
        Task<bool> ClearIncomeExpiresTimeAsync(IEnumerable<string> incomes);
    }
}