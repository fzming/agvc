using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Sockets;
using CoreRepository.Core;
using CoreRepository.Kernel;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Polly;

namespace CoreRepository
{
    /// <summary>
    /// repository implementation for mongo
    /// </summary>
    /// <typeparam name="T"></typeparam>

    //[InheritedExport]
    public partial class MongoRepository<T> : DisposableRepository, IMongoRepository<T>
        where T : MongoEntity
    {
        public MongoRepository(IMongoUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        public IMongoUnitOfWork UnitOfWork { get; set; }
        public dynamic DynamicCollection => Collection;
        #region MongoSpecific

        /// <summary>
        /// mongo collection
        /// </summary>
        public IMongoCollection<T> Collection =>
            _collection ??= UnitOfWork.MongoContext.GetCollection<T>();


        private IMongoCollection<T> _collection;

        /// <summary>
        /// filter for collection
        /// </summary>
        public FilterDefinitionBuilder<T> Filter => Builders<T>.Filter;

        public SortDefinitionBuilder<T> Sorter => Builders<T>.Sort;

        /// <summary>
        /// projector for collection
        /// </summary>
        public ProjectionDefinitionBuilder<T> Projection { get; } = Builders<T>.Projection;

        /// <summary>
        /// updater for collection
        /// </summary>
        public UpdateDefinitionBuilder<T> Updater => Builders<T>.Update;



        private IFindFluent<T, T> Query(FilterDefinition<T> filter)
        {
            return Collection.Find(filter);
        }

        private IFindFluent<T, T> Query(Expression<Func<T, bool>> filter)
        {
            return Collection.Find(filter);
        }



        private IFindFluent<T, T> Query()
        {
            return Collection.Find(Filter.Empty);
        }

        #endregion MongoSpecific

        #region CRUD

        #region Delete

        /// <summary>
        /// delete entity
        /// </summary>
        /// <param name="entity">entity</param>
        public virtual bool Delete(T entity)
        {
            return Delete(entity.Id);
        }


        /// <summary>
        /// delete by selector
        /// </summary>
        /// <param name="id">selector</param>
        public virtual bool Delete(string id)
        {
            return Retry(() =>
            {
                return Collection.DeleteOne(i => i.Id == id).DeletedCount > 0;
            });
        }

        /// <summary>
        /// delete items with filter
        /// </summary>
        /// <param name="filter">expression filter</param>
        public virtual bool Delete(Expression<Func<T, bool>> filter)
        {
            return Retry(() => Collection.DeleteMany(filter).DeletedCount > 0);
        }


        /// <summary>
        /// delete all documents
        /// </summary>
        public virtual bool Clear()
        {
            return Retry(() => Collection.DeleteMany(Filter.Empty).DeletedCount > 0);
        }


        #endregion Delete

        #region Find
        /// <summary>
        /// find entities
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <returns>collection of entity</returns>
        public virtual IEnumerable<T> Find(FilterDefinition<T> filter)
        {
            return Query(filter).ToEnumerable();
        }



        /// <summary>
        /// find entities
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <returns>collection of entity</returns>
        public virtual IEnumerable<T> Find(Expression<Func<T, bool>> filter)
        {
            return Query(filter).ToEnumerable();
        }

        /// <summary>
        /// find entities with paging
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <param name="pageIndex">page index, based on 1</param>
        /// <param name="size">number of items in page</param>
        /// <returns>collection of entity</returns>
        public virtual IEnumerable<T> Find(Expression<Func<T, bool>> filter, int pageIndex, int size)
        {
            return Find(filter, i => i.Id, pageIndex, size);
        }

        /// <summary>
        /// find entities with paging and ordering
        /// default ordering is descending
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <param name="order">ordering parameters</param>
        /// <param name="pageIndex">page index, based on 1</param>
        /// <param name="size">number of items in page</param>
        /// <returns>collection of entity</returns>
        public virtual IEnumerable<T> Find(Expression<Func<T, bool>> filter, Expression<Func<T, object>> order, int pageIndex, int size)
        {
            return Find(filter, order, pageIndex, size, true);
        }

        /// <summary>
        /// find entities with paging and ordering in direction
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <param name="order">ordering parameters</param>
        /// <param name="pageIndex">page index, based on 1</param>
        /// <param name="size">number of items in page</param>
        /// <param name="isDescending">ordering direction</param>
        /// <returns>collection of entity</returns>
        public virtual IEnumerable<T> Find(Expression<Func<T, bool>> filter, Expression<Func<T, object>> order, int pageIndex, int size, bool isDescending)
        {
            return Retry(() =>
            {
                var query = Query(filter).Skip(((Math.Max(pageIndex,1) - 1)) * size).Limit(size);
                return (isDescending ? query.SortByDescending(order) : query.SortBy(order)).ToEnumerable();
            });
        }

        #endregion Find

        #region FindAll

        /// <summary>
        /// fetch all items in collection
        /// </summary>
        /// <returns>collection of entity</returns>
        public virtual IEnumerable<T> FindAll()
        {
            return Retry(() => Query().ToEnumerable());
        }

        /// <summary>
        /// fetch all items in collection with paging
        /// </summary>
        /// <param name="pageIndex">page index, based on 0</param>
        /// <param name="size">number of items in page</param>
        /// <returns>collection of entity</returns>
        public virtual IEnumerable<T> FindAll(int pageIndex, int size)
        {
            return FindAll(i => i.Id, pageIndex, size);
        }

        /// <summary>
        /// fetch all items in collection with paging and ordering
        /// default ordering is descending
        /// </summary>
        /// <param name="order">ordering parameters</param>
        /// <param name="pageIndex">page index, based on 0</param>
        /// <param name="size">number of items in page</param>
        /// <returns>collection of entity</returns>
        public virtual IEnumerable<T> FindAll(Expression<Func<T, object>> order, int pageIndex, int size)
        {
            return FindAll(order, pageIndex, size, true);
        }

        /// <summary>
        /// fetch all items in collection with paging and ordering in direction
        /// </summary>
        /// <param name="order">ordering parameters</param>
        /// <param name="pageIndex">page index, based on 1</param>
        /// <param name="size">number of items in page</param>
        /// <param name="isDescending">ordering direction</param>
        /// <returns>collection of entity</returns>
        public virtual IEnumerable<T> FindAll(Expression<Func<T, object>> order, int pageIndex, int size, bool isDescending)
        {
            return Retry(() =>
            {
                var query = Query().Skip(((Math.Max(pageIndex,1) - 1)) * size).Limit(size);
                return (isDescending ? query.SortByDescending(order) : query.SortBy(order)).ToEnumerable();
            });
        }

        #endregion FindAll


        #region First

        /// <summary>
        /// get first item in collection
        /// </summary>
        /// <returns>entity of <typeparamref name="T"/></returns>
        public virtual T First()
        {
            return FindAll(i => i.Id, 1, 1, false).FirstOrDefault();
        }

        /// <summary>
        /// get first item in query
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <returns>entity of <typeparamref name="T"/></returns>
        public virtual T First(FilterDefinition<T> filter)
        {
            return Find(filter).FirstOrDefault();
        }

        /// <summary>
        /// get first item in query
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <returns>entity of <typeparamref name="T"/></returns>
        public virtual T First(Expression<Func<T, bool>> filter)
        {
            return First(filter, i => i.Id);
        }


        /// <summary>
        /// get first item in query with order
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <param name="order">ordering parameters</param>
        /// <returns>entity of <typeparamref name="T"/></returns>
        public virtual T First(Expression<Func<T, bool>> filter, Expression<Func<T, object>> order)
        {
            return First(filter, order, false);
        }

        /// <summary>
        /// get first item in query with order and direction
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <param name="order">ordering parameters</param>
        /// <param name="isDescending">ordering direction</param>
        /// <returns>entity of <typeparamref name="T"/></returns>
        public virtual T First(Expression<Func<T, bool>> filter, Expression<Func<T, object>> order, bool isDescending)
        {
            return Find(filter, order, 1, 1, isDescending).FirstOrDefault();
        }

        #endregion First
        #region Get

        /// <summary>
        /// get by selector
        /// </summary>
        /// <param name="id">selector value</param>
        /// <returns>entity of <typeparamref name="T"/></returns>
        public virtual T Get(string id)
        {
            return Retry(() =>
            {
                return Find(i => i.Id == id).FirstOrDefault();
            });
        }

        #endregion Get

        #region Insert

        /// <summary>
        /// insert entity
        /// </summary>
        /// <param name="entity">entity</param>
        public virtual void Insert(T entity)
        {
            Retry(() =>
            {
                Collection.InsertOne(entity);
            });
        }



        /// <summary>
        /// insert entity collection
        /// </summary>
        /// <param name="entities">collection of entities</param>
        public virtual void Insert(IEnumerable<T> entities)
        {
            Retry(() =>
            {
                Collection.InsertMany(entities);
            });
        }


        #endregion Insert

        #region Last

        /// <summary>
        /// get first item in collection
        /// </summary>
        /// <returns>entity of <typeparamref name="T"/></returns>
        public virtual T Last()
        {
            return FindAll(i => i.Id, 1, 1, true).FirstOrDefault();
        }

        /// <summary>
        /// get last item in query
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <returns>entity of <typeparamref name="T"/></returns>
        public virtual T Last(FilterDefinition<T> filter)
        {
            return Query(filter).SortByDescending(i => i.Id).FirstOrDefault();
        }

        /// <summary>
        /// get last item in query
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <returns>entity of <typeparamref name="T"/></returns>
        public virtual T Last(Expression<Func<T, bool>> filter)
        {
            return Last(filter, i => i.Id);
        }

        /// <summary>
        /// get last item in query with order
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <param name="order">ordering parameters</param>
        /// <returns>entity of <typeparamref name="T"/></returns>
        public virtual T Last(Expression<Func<T, bool>> filter, Expression<Func<T, object>> order)
        {
            return Last(filter, order, false);
        }

        /// <summary>
        /// get last item in query with order and direction
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <param name="order">ordering parameters</param>
        /// <param name="isDescending">ordering direction</param>
        /// <returns>entity of <typeparamref name="T"/></returns>
        public virtual T Last(Expression<Func<T, bool>> filter, Expression<Func<T, object>> order, bool isDescending)
        {
            return First(filter, order, !isDescending);
        }

        #endregion Last


        #region Update

        public virtual bool Replace(string id, T entity)
        {
            entity.Id = id;
            return Retry(() => { return Collection.ReplaceOne(p => p.Id == id, entity).IsAcknowledged; });
        }

        public virtual bool Update(T entity)
        {
            return Retry(() => { return Collection.ReplaceOne(p => p.Id == entity.Id, entity).IsAcknowledged; });

        }

        /// <summary>
        /// update a property field in an entity
        /// </summary>
        /// <typeparam name="TField">field type</typeparam>
        /// <param name="entity">entity</param>
        /// <param name="field">field</param>
        /// <param name="value">new value</param>
        /// <returns>true if successful, otherwise false</returns>
        public virtual bool Update<TField>(T entity, Expression<Func<T, TField>> field, TField value)
        {
            return Update(entity, Updater.Set(field, value));
        }

        /// <summary>
        /// 根据表达式更新
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="lambda"></param>
        /// <returns></returns>
        public long Update(Expression<Func<T, bool>> predicate, Expression<Func<T, T>> lambda)
        {
            var updateDefinitionList = MongoExpression<T>.GetUpdateDefinition(lambda);

            var updates = new UpdateDefinitionBuilder<T>().Combine(updateDefinitionList);

            var result = Collection.UpdateMany<T>(predicate, updates);

            return result.ModifiedCount;
        }

        /// <summary>
        /// update an entity with updated fields
        /// </summary>
        /// <param name="id">selector</param>
        /// <param name="updates">updated field(s)</param>
        /// <returns>true if successful, otherwise false</returns>
        public virtual bool Update(string id, params UpdateDefinition<T>[] updates)
        {
            return Update(Filter.Eq(i => i.Id, id), updates);
        }


        /// <summary>
        /// update an entity with updated fields
        /// </summary>
        /// <param name="entity">entity</param>
        /// <param name="updates">updated field(s)</param>
        /// <returns>true if successful, otherwise false</returns>
        public virtual bool Update(T entity, params UpdateDefinition<T>[] updates)
        {

            return Update(entity.Id, updates);
        }


        /// <summary>
        /// update a property field in entities
        /// </summary>
        /// <typeparam name="TField">field type</typeparam>
        /// <param name="filter">filter</param>
        /// <param name="field">field</param>
        /// <param name="value">new value</param>
        /// <returns>true if successful, otherwise false</returns>
        public virtual bool Update<TField>(FilterDefinition<T> filter, Expression<Func<T, TField>> field, TField value)
        {
            return Update(filter, Updater.Set(field, value));
        }




        /// <summary>
        /// update found entities by filter with updated fields
        /// </summary>
        /// <param name="filter">collection filter</param>
        /// <param name="updates">updated field(s)</param>
        /// <returns>true if successful, otherwise false</returns>
        public virtual bool Update(FilterDefinition<T> filter, params UpdateDefinition<T>[] updates)
        {
            if (updates.Length == 0)
            {
                throw new Exception("nothing to updates");
            }
            return Retry(() =>
            {
                var update = Updater.Combine(updates).CurrentDate(i => i.ModifiedOn);
                var updateOption = new UpdateOptions
                {
                    IsUpsert = false //更新文档时，如果文档不存在是否自动插入
                };
                return Collection.UpdateMany(filter,
                        update.CurrentDate(i => i.ModifiedOn), updateOption)
                    .IsAcknowledged;
            });
        }


        /// <summary>
        /// update found entities by filter with updated fields
        /// </summary>
        /// <param name="filter">collection filter</param>
        /// <param name="updates">updated field(s)</param>
        /// <returns>true if successful, otherwise false</returns>
        public virtual bool Update(Expression<Func<T, bool>> filter, params UpdateDefinition<T>[] updates)
        {
            return Retry(() =>
            {
                var update = Updater.Combine(updates).CurrentDate(i => i.ModifiedOn);
                return Collection.UpdateMany(filter, update).IsAcknowledged;
            });
        }


        #endregion Update

        #endregion CRUD
        #region Aggregate
        public virtual TResult Max<TResult>(Expression<Func<T, TResult>> selector)
        {
            return Collection.AsQueryable().GroupBy(selector).Max(p => p.Key);
        }

        public virtual TResult Max<TResult>(Expression<Func<T, bool>> filter, Expression<Func<T, TResult>> selector)
        {
            return Collection.AsQueryable().Where(filter).GroupBy(selector).Max(p => p.Key);
        }

        public virtual TResult Min<TResult>(Expression<Func<T, TResult>> selector)
        {
            return Collection.AsQueryable().GroupBy(selector).Min(p => p.Key);
        }

        public virtual TResult Min<TResult>(Expression<Func<T, bool>> filter, Expression<Func<T, TResult>> selector)
        {
            return Collection.AsQueryable().Where(filter).GroupBy(selector).Min(p => p.Key);
        }



        /// <summary>
        /// validate if filter result exists
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <returns>true if exists, otherwise false</returns>
        public virtual bool Any(Expression<Func<T, bool>> filter)
        {
            return Retry(() => First(filter) != null);
        }

        #region Count
        /// <summary>
        /// get number of filtered documents
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <returns>number of documents</returns>
        public virtual long Count(Expression<Func<T, bool>> filter)
        {
            return Retry(() => Collection.CountDocuments(filter));
        }

        /// <summary>
        /// get number of documents in collection
        /// </summary>
        /// <returns>number of documents</returns>
        public virtual long Count()
        {
            return Retry(() => Collection.CountDocuments(Filter.Empty));
        }



        #endregion Count

        #region Sum

        public double Sum(Expression<Func<T, bool>> filter, Expression<Func<T, double>> selector)
        {
            return Collection.AsQueryable().Where(filter).Sum(selector);
        }

        public double Sum(Expression<Func<T, double>> selector)
        {
            return Collection.AsQueryable().Sum(selector);
        }

        public int Sum(Expression<Func<T, bool>> filter, Expression<Func<T, int>> selector)
        {
            return Collection.AsQueryable().Where(filter).Sum(selector);
        }

        public int Sum(Expression<Func<T, int>> selector)
        {
            return Collection.AsQueryable().Sum(selector);
        }

        public long Sum(Expression<Func<T, bool>> filter, Expression<Func<T, long>> selector)
        {
            return Collection.AsQueryable().Where(filter).Sum(selector);
        }

        public long Sum(Expression<Func<T, long>> selector)
        {
            return Collection.AsQueryable().Sum(selector);
        }

        #endregion
        #endregion Utils

        #region RetryPolicy

        /// <summary>
        /// retry operation for three times if IOException occurs
        /// </summary>
        /// <typeparam name="TResult">return type</typeparam>
        /// <param name="action">action</param>
        /// <returns>action result</returns>
        /// <example>
        /// return Retry(() => 
        /// { 
        ///     do_something;
        ///     return something;
        /// });
        /// </example>
        protected virtual TResult Retry<TResult>(Func<TResult> action)
        {
            return Policy
                .Handle<MongoConnectionException>(i => i.InnerException?.GetType() == typeof(IOException) ||
                                                       i.InnerException?.GetType() == typeof(SocketException))
                .Retry(3)
                .Execute(action);
        }
        protected virtual void Retry(Action action)
        {
            Policy
               .Handle<MongoConnectionException>(i => i.InnerException?.GetType() == typeof(IOException) ||
               i.InnerException?.GetType() == typeof(SocketException))
               .Retry(3)
               .Execute(action);
        }
        #endregion


    }
}