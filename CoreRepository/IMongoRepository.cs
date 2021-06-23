using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CoreRepository.Core;
using CoreRepository.Core.Aggregate;
using MongoDB.Driver;

namespace CoreRepository
{
    /// <summary>
    /// 针对Mongodb的仓储接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal interface IMongoRepository<T> : IRepository<T>,IAggregateRepository<T> where T : MongoEntity
    {
        #region UnitOfWork
        IMongoUnitOfWork UnitOfWork{get; set; }
        #endregion
        #region MongoSpecific

        /// <summary>
        /// mongo collection
        /// </summary>
        IMongoCollection<T> Collection { get; }

        /// <summary>
        /// filter for collection
        /// </summary>
        FilterDefinitionBuilder<T> Filter { get; }
        SortDefinitionBuilder<T> Sorter { get; }
        /// <summary>
        /// projector for collection
        /// </summary>
        ProjectionDefinitionBuilder<T> Projection { get; }

        /// <summary>
        /// updater for collection
        /// </summary>
        UpdateDefinitionBuilder<T> Updater { get; }

        #endregion MongoSpecific


        /// <summary>
        /// find entities
        /// </summary>
        /// <param name="filter">filter definition</param>
        /// <returns>collection of entity</returns>
        IEnumerable<T> Find(FilterDefinition<T> filter);

        Task<IEnumerable<T>> FindAsync(FilterDefinition<T> filter,
            Expression<Func<T, object>> order, int pageIndex, int size, bool isDescending);

        Task<IEnumerable<T>> FindAsync(string json,
            Expression<Func<T, object>> order, int pageIndex, int size, bool isDescending);
        /// <summary>
        /// get first item in query
        /// </summary>
        /// <param name="filter">filter definition</param>
        /// <returns>entity of <typeparamref name="T"/></returns>
        T First(FilterDefinition<T> filter);

        /// <summary>
        /// get last item in query
        /// </summary>
        /// <param name="filter">filter definition</param>
        /// <returns>entity of <typeparamref name="T"/></returns>
        T Last(FilterDefinition<T> filter);
        /// <summary>
        /// update an entity with updated fields
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="updates">updated field(s)</param>
        /// <returns>true if successful, otherwise false</returns>
        bool Update(string id, params UpdateDefinition<T>[] updates);

        /// <summary>
        /// update an entity with updated fields
        /// </summary>
        /// <param name="entity">entity</param>
        /// <param name="updates">updated field(s)</param>
        /// <returns>true if successful, otherwise false</returns>
        bool Update(T entity, params UpdateDefinition<T>[] updates);

        /// <summary>
        /// update found entities by filter with updated fields
        /// </summary>
        /// <param name="filter">collection filter</param>
        /// <param name="updates">updated field(s)</param>
        /// <returns>true if successful, otherwise false</returns>
        bool Update(FilterDefinition<T> filter, params UpdateDefinition<T>[] updates);

        /// <summary>
        /// update found entities by filter with updated fields
        /// </summary>
        /// <param name="filter">collection filter</param>
        /// <param name="updates">updated field(s)</param>
        /// <returns>true if successful, otherwise false</returns>
        bool Update(Expression<Func<T, bool>> filter, params UpdateDefinition<T>[] updates);
        /// <summary>
        /// update a property field in entities
        /// </summary>
        /// <typeparam name="TField">field type</typeparam>
        /// <param name="filter">filter</param>
        /// <param name="field">field</param>
        /// <param name="value">new value</param>
        /// <returns>true if successful, otherwise false</returns>
        bool Update<TField>(FilterDefinition<T> filter, Expression<Func<T, TField>> field, TField value);
        /// <summary>
        /// update an entity with updated fields
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="updates">updated field(s)</param>
        Task<bool> UpdateAsync(string id, params UpdateDefinition<T>[] updates);

        /// <summary>
        /// update an entity with updated fields
        /// </summary>
        /// <param name="entity">entity</param>
        /// <param name="updates">updated field(s)</param>
        Task<bool> UpdateAsync(T entity, params UpdateDefinition<T>[] updates);

        /// <summary>
        /// update a property field in entities
        /// </summary>
        /// <typeparam name="TField">field type</typeparam>
        /// <param name="filter">filter</param>
        /// <param name="field">field</param>
        /// <param name="value">new value</param>
        Task<bool> UpdateAsync<TField>(FilterDefinition<T> filter, Expression<Func<T, TField>> field, TField value);

        /// <summary>
        /// update found entities by filter with updated fields
        /// </summary>
        /// <param name="filter">collection filter</param>
        /// <param name="updates">updated field(s)</param>
        Task<bool> UpdateAsync(FilterDefinition<T> filter, params UpdateDefinition<T>[] updates);

        /// <summary>
        /// update found entities by filter with updated fields
        /// </summary>
        /// <param name="filter">collection filter</param>
        /// <param name="updates">updated field(s)</param>
        Task<bool> UpdateAsync(Expression<Func<T, bool>> filter, params UpdateDefinition<T>[] updates);

       
    }
}