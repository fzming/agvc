using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgvcCoreData.Users;
using AgvcEntitys.Users;
using AgvcRepository.Users.Interfaces;
using AgvcService.Users.Models;
using CoreData;
using CoreService;
using Utility.Extensions;

namespace AgvcService.Users
{
    /// <summary>
    /// 账户余额服务
    /// </summary>
    public class AccountBalanceService : AbstractService, IAccountBalanceService
    {

        private IAccountBalanceRepository AccountBalanceRepository { get; }
      //  private ISignalrService SignalrService { get; }

        public AccountBalanceService(IAccountBalanceRepository accountBalanceRepository)
        {
            AccountBalanceRepository = accountBalanceRepository;
           // SignalrService = signalrService;

        }

        public Task<double> GetBalanceTotalAsync(string userid, BalanceType balanceType)
        {
            return AccountBalanceRepository.GetTotalAsync(userid, balanceType);
        }

        public Task<double> GetIncomeTotalAsync(string userid, BalanceType balanceType)
        {
            return AccountBalanceRepository.GetIncomeTotalAsync(userid, balanceType);
        }

        public Task<double> GetExpenseTotalAsync(string userid, BalanceType balanceType)
        {
            return AccountBalanceRepository.GetExpenseTotalAsync(userid, balanceType);
        }

        public Task<IEnumerable<UserBalanceStatisticModel>> UserBalanceStatisticAsync()
        {
            return AccountBalanceRepository.UserBalanceStatisticAsync();
        }

        public async Task<bool> IncomeBalanceAsync(string userid, double balanceValue, BalanceType balanceType,
            IncomeSourceType incomeSourceType = IncomeSourceType.None,
            string incomeUniqueKey = "",
            DateTime? expireTime = null)
        {

            using (await _mutex.LockAsync())
            {

                //防止重复充值
                if (incomeUniqueKey.IsNotNullOrEmpty() && IsIncomeKeyExits(userid, incomeUniqueKey))
                {
                    return false;
                }
                var success =
                    await AccountBalanceRepository.IncomeBalanceAsync(userid, balanceValue, balanceType,
                        incomeSourceType, incomeUniqueKey, expireTime);
                if (!success) return false;
                var beanTotal = await GetBalanceTotalAsync(userid, BalanceType.Bean);
                // try
                // {
                //     //发送余额充值成功消息
                //     await SignalrService.SendMessageAsync(userid,
                //         new BalanceIncomeMessage
                //         {
                //             Bean = balanceValue,
                //             BeanTotal = beanTotal
                //         });
                // }
                // catch
                // {
                //     // ignored
                // }


                return true;
            }

        }

        private bool IsIncomeKeyExits(string userid, string key)
        {
            return AccountBalanceRepository.Any(p => p.UserId == userid && p.IncomeUniqueKey == key);
        }

        public async Task<bool> ExpenseBalanceAsync(string userid, double balanceValue, BalanceType balanceType)
        {

            using (await _mutex.LockAsync())
            {
                // 需预先进行余额判断
                var balanceTotal = await this.GetBalanceTotalAsync(userid, balanceType);
                if (balanceTotal >= balanceValue && balanceTotal > 0)
                {
                    //需要对现有的临时收入进行消除处理
                    await SetTemporaryIncomeBalanceAsync(userid, balanceValue, balanceType, DateTime.Now);

                    return await AccountBalanceRepository.ExpenseBalanceAsync(userid, balanceValue, balanceType);
                }

            }

            return false;

        }

        private async Task SetTemporaryIncomeBalanceAsync(string userid, double balanceValue, BalanceType balanceType, DateTime expireTime)
        {
            if (balanceValue <= 0)
            {
                return;
            }
            //目前尚未过期的临时收入
            var temporaryIncomes = (await AccountBalanceRepository.FindAsync(p => p.UserId == userid
                && p.BalanceType == balanceType &&
                p.PaymentType == BalancePaymentType.Income
                && p.ExpireTime != null &&
                p.ExpireTime > expireTime)).OrderBy(p => p.BalanceValue).ToList();
            var temporaryTotal = temporaryIncomes.Sum(p => p.BalanceValue);//未过期的总额
            if (temporaryTotal == 0)
            {
                return;
            }
            var clears = new Dictionary<string, double>();
            var splits = new Dictionary<AccountBalance, double>();//待拆分的费用

            if (balanceValue >= temporaryTotal) //将有效的临时收入提升为正式收入
            {
                clears = temporaryIncomes.ToDictionary(k => k.Id, v => v.BalanceValue);
            }
            else
            {
                var value = balanceValue;//待提升余额
                foreach (var temporary in temporaryIncomes)
                {
                    if (value == 0)
                    {
                        break; //跳出循环
                    }
                    if (value >= temporary.BalanceValue)
                    {
                        value -= temporary.BalanceValue;
                        clears.Add(temporary.Id, temporary.BalanceValue);
                    }
                    else //不够减：单次临时收入过大，对temporary进行拆分
                    {
                        value = 0;
                        splits.Add(temporary, value);
                        break;
                    }
                }
            }

            if (clears.Any())
            {
                await AccountBalanceRepository.ClearIncomeExpiresTimeAsync(clears.Select(p => p.Key));
            }

            if (splits.Any()) //拆分原始收入
            {
                var sps = splits.First();
                var spTempIncome = sps.Key;
                var spTempValue = sps.Value;
                var spNewIncome = spTempIncome.MapTo(new AccountBalance());
                spNewIncome.Id = string.Empty;
                spNewIncome.ExpireTime = null;
                spNewIncome.BalanceValue = spTempValue;
                //更改大额收入
                await AccountBalanceRepository.UpdateAsync(spTempIncome.Id, p => p.BalanceValue,
                    spTempIncome.BalanceValue - spTempValue);
                //增加补充的收入
                await AccountBalanceRepository.InsertAsync(spNewIncome);
            }
        }

        public Task<PageResult<AccountBalance>> QueryBalanceLogsAsync(string clientId, BalanceType balanceType, BalanceLogQuery query)
        {
            return AccountBalanceRepository.AdvanceQueryAsync(clientId, balanceType, query.paymentType, query.btm, query.etm,
                query.PageSize, query.PageIndex);

        }

        public Task<IEnumerable<AccountBalance>> QueryIncomeSourceAsync(string clientId, BalanceType balanceType,
            IncomeSourceType incomeSourceType)
        {
            return AccountBalanceRepository.FindAsync(p =>
                p.UserId == clientId && p.BalanceType == balanceType && p.PaymentType == BalancePaymentType.Income &&
                p.IncomeSource == incomeSourceType);
        }
    }
}