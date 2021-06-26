using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgvcCoreData.Users;
using AgvcEntitys.Users;
using AgvcRepository.Users.Interfaces;
using AgvcService.Users.Models;
using CoreData;
using CoreRepository;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace AgvcRepository.Users
{
    public class AccountBalanceRepository : MongoRepository<AccountBalance>, IAccountBalanceRepository
    {
        public AccountBalanceRepository(IMongoUnitOfWork unitOfWork, IAccountRepository accountRepository) :
            base(unitOfWork)
        {
            AccountRepository = accountRepository;
        }

        private IAccountRepository AccountRepository { get; }

        public Task<IEnumerable<AccountBalance>> QueryAsync(string userid, BalanceType balanceType)
        {
            return FindAsync(p => p.UserId == userid && p.BalanceType == balanceType);
        }

        public Task<PageResult<AccountBalance>> AdvanceQueryAsync(string clientId, BalanceType balanceType,
            int paymentType, DateTime? btm, DateTime? etm,
            int pageSize, int pageIndex)
        {
            #region Build Query

            var query = Collection.AsQueryable().Where(p => p.UserId == clientId);
            if (paymentType > -1) query = query.Where(p => p.PaymentType == (BalancePaymentType) paymentType);


            if (btm.HasValue) query = query.Where(p => p.CreatedOn >= btm.Value);

            if (etm.HasValue) query = query.Where(p => p.CreatedOn <= etm.Value);

            #endregion

            return query.ToPageListAsync(pageIndex, pageSize, p => p.CreatedOn, true);
        }

        /// <inheritdoc />
        public async Task<double> GetTotalAsync(string userid, BalanceType balanceType)
        {
            var incomeTotal = GetIncomeTotalAsync(userid, balanceType);
            var expenseTotal = GetExpenseTotalAsync(userid, balanceType);
            await Task.WhenAll(incomeTotal, expenseTotal).ConfigureAwait(false);
            return incomeTotal.Result - expenseTotal.Result;
        }

        public Task<double> GetIncomeTotalAsync(string userid, BalanceType balanceType)
        {
            return SumAsync(p => p.UserId == userid && p.BalanceType == balanceType
                                                    && p.PaymentType == BalancePaymentType.Income &&
                                                    (p.ExpireTime == null || p.ExpireTime > DateTime.Now),
                p => p.BalanceValue);
        }

        public Task<double> GetExpenseTotalAsync(string userid, BalanceType balanceType)
        {
            return SumAsync(
                p => p.UserId == userid &&
                     (p.BalanceType == balanceType) & (p.PaymentType == BalancePaymentType.Expenses),
                p => p.BalanceValue);
        }

        /// <summary>
        ///     收入
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="balanceValue"></param>
        /// <param name="balanceType"></param>
        /// <param name="incomeSourceType"></param>
        /// <param name="incomeUniqueKey"></param>
        /// <param name="expireTime">具体过期时间，过期后将失效</param>
        /// <returns></returns>
        public async Task<bool> IncomeBalanceAsync(string userid, double balanceValue, BalanceType balanceType,
            IncomeSourceType incomeSourceType = IncomeSourceType.None,
            string incomeUniqueKey = "", DateTime? expireTime = null)
        {
            await InsertAsync(new AccountBalance
            {
                BalanceType = balanceType,
                UserId = userid,
                BalanceValue = balanceValue,
                PaymentType = BalancePaymentType.Income,
                IncomeSource = incomeSourceType,
                IncomeUniqueKey = incomeUniqueKey,
                ExpireTime = expireTime
            });
            return true;
        }

        /// <inheritdoc />
        /// <summary>
        ///     支出
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="balanceValue"></param>
        /// <param name="balanceType"></param>
        /// <returns></returns>
        public async Task<bool> ExpenseBalanceAsync(string userid, double balanceValue, BalanceType balanceType)
        {
            await InsertAsync(new AccountBalance
            {
                BalanceType = balanceType,
                UserId = userid,
                BalanceValue = balanceValue,
                PaymentType = BalancePaymentType.Expenses
            });
            return true;
        }

        public async Task<IEnumerable<UserBalanceStatisticModel>> UserBalanceStatisticAsync()
        {
            var accountCollection = AccountRepository.DynamicCollection as IMongoCollection<Account>;
            var query = Collection.AsQueryable()
                .Join(accountCollection,
                    p => p.UserId,
                    p => p.Id,
                    (balance, account) => new {balance, account})
                .Where(k => k.balance.ExpireTime == null || k.balance.ExpireTime > DateTime.Now)
                .GroupBy(p => new
                {
                    p.balance.UserId,
                    p.balance.PaymentType
                })
                .Select(p => new UserBalanceStatisticModel
                {
                    Id = p.Key.UserId,
                    Nick = p.First().account.Nick,
                    PaymentType = p.Key.PaymentType,
                    Total = p.Sum(k => k.balance.BalanceValue)
                });
            return await query.ToListAsync();
        }

        /// <summary>
        ///     清除收入的过期标记
        /// </summary>
        /// <param name="incomes"></param>
        /// <returns></returns>
        public async Task<bool> ClearIncomeExpiresTimeAsync(IEnumerable<string> incomes)
        {
            //filter
            var filter = Filter.And(Filter.In(x => x.Id, incomes),
                Filter.Eq(p => p.PaymentType, BalancePaymentType.Income),
                Filter.Gt(p => p.ExpireTime, DateTime.Now));
            //updates
            var update = Updater
                .Unset(p => p.ExpireTime)
                .CurrentDate(i => i.ModifiedOn);
            var rs = await Collection.UpdateManyAsync(filter, update);
            return rs.ModifiedCount > 0;
        }
    }
}