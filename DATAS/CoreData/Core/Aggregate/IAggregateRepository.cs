using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CoreData.Core.Aggregate
{
    /// <summary>
    ///     聚合仓储接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IAggregateRepository<T> where T : AggregateRoot
    {
        #region Max

        Task<TResult> MaxAsync<TResult>(Expression<Func<T, TResult>> selector);
        Task<TResult> MaxAsync<TResult>(Expression<Func<T, bool>> filter, Expression<Func<T, TResult>> selector);

        TResult Max<TResult>(Expression<Func<T, TResult>> selector);
        TResult Max<TResult>(Expression<Func<T, bool>> filter, Expression<Func<T, TResult>> selector);

        #endregion

        #region Min

        Task<TResult> MinAsync<TResult>(Expression<Func<T, TResult>> selector);
        Task<TResult> MinAsync<TResult>(Expression<Func<T, bool>> filter, Expression<Func<T, TResult>> selector);

        TResult Min<TResult>(Expression<Func<T, TResult>> selector);
        TResult Min<TResult>(Expression<Func<T, bool>> filter, Expression<Func<T, TResult>> selector);

        #endregion
    }
}