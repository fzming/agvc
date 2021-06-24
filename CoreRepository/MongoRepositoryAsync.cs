using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Sockets;
using System.Threading.Tasks;
using CoreData;
using CoreRepository.Core;
using CoreRepository.Core.Aggregate;
using CoreRepository.Kernel;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Nito.AsyncEx;
using Polly;
using Utility;

namespace CoreRepository
{
    /// <summary>
    /// 支持异步版本函数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial class MongoRepository<T> where T : MongoEntity
    {
        public AsyncLock Mutex { get; }

        protected MongoRepository(IMongoUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
            Mutex = new AsyncLock();
        }

        /// <summary>
        /// 根据表达式更新
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="lambda"></param>
        /// <returns></returns>
        public async Task<long> UpdateAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, T>> lambda)
        {
            var updateDefinitionList = MongoExpression<T>.GetUpdateDefinition(lambda);

            var updates = new UpdateDefinitionBuilder<T>().Combine(updateDefinitionList);

            var result = await Collection.UpdateManyAsync<T>(predicate, updates);

            return result.ModifiedCount;
        }

        /// <summary>
        /// 字段值是否有重复
        /// </summary>
        /// <typeparam name="TField"></typeparam>
        /// <param name="field">字段</param>
        /// <param name="value">值</param>
        /// <param name="orgId">机构ID（可选）</param>
        /// <param name="modifyId">修改的数据库实体ID（可选）</param>
        /// <returns></returns>
        public async Task<bool> IsFieldRepeatAsync<TField>(Expression<Func<T, TField>> field, TField value,
            string orgId = "", string modifyId = "")
        {
            var filters = new List<FilterDefinition<T>>
            {

                Filter.Eq(field, value)
            };
            if (typeof(MongoEntity).IsAssignableFrom(typeof(T)) && orgId.IsObjectId())
            {
                filters.Add(Filter.Eq("_o", ObjectId.Parse(orgId)));
            }
            if (modifyId.IsObjectId())
            {
                filters.Add(Filter.Ne("_id", ObjectId.Parse(modifyId)));
            }

            var filter = Filter.And(filters);
            var count = await Collection.CountDocumentsAsync(filter);
            return count > 0;
        }

        public async Task<bool> IsFieldRepeatAsync<TField>(string field, TField value, string orgId = "", string modifyId = "")
        {
            var filters = new List<FilterDefinition<T>>
            {

                Filter.Eq(field, value)
            };
            if (typeof(MongoEntity).IsAssignableFrom(typeof(T)) && orgId.IsObjectId())
            {
                filters.Add(Filter.Eq("_o", ObjectId.Parse(orgId)));
            }
            if (modifyId.IsObjectId())
            {
                filters.Add(Filter.Ne("_id", ObjectId.Parse(modifyId)));
            }

            var filter = Filter.And(filters);
            var count = await Collection.CountDocumentsAsync(filter);
            return count > 0;
        }

        #region CRUD

        #region Async Delete



        /// <summary>
        /// delete entity
        /// </summary>
        /// <param name="entity">entity</param>
        public virtual Task<bool> DeleteAsync(T entity)
        {
            return DeleteAsync(entity.Id);
        }
        public virtual Task<bool> DeleteAsync(Expression<Func<T, bool>> filter)
        {
            return DeleteManyAsync(filter);
        }
        /// <summary>
        /// delete by selector
        /// </summary>
        /// <param name="id">selector</param>
        public virtual Task<bool> DeleteAsync(string id)
        {
            return DeleteOneAsync(p => p.Id == id);
        }


        /// <summary>
        /// delete items with filter
        /// </summary>
        /// <param name="filter">expression filter</param>
        public virtual Task<bool> DeleteManyAsync(Expression<Func<T, bool>> filter)
        {
            return RetryAsync(async () =>
           {
               var rs = await Collection.DeleteManyAsync(filter).ConfigureAwait(false);
               return rs.DeletedCount > 0;
           });
        }
        public virtual Task<bool> DeleteOneAsync(Expression<Func<T, bool>> filter)
        {
            return RetryAsync(async () =>
           {
               var rs = await Collection.DeleteOneAsync(filter).ConfigureAwait(false);
               return rs.DeletedCount > 0;
           });
        }


        /// <summary>
        /// delete all documents
        /// </summary>
        public virtual Task<bool> ClearAsync()
        {
            return DeleteManyAsync(p => true);
        }
        #endregion Delete

        #region Async Find


        public virtual async Task<IEnumerable<T>> FindAsync(FilterDefinition<T> filter)
        {
            using var cursor = await Collection.FindAsync(filter).ConfigureAwait(false);
            return await cursor.ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> filter)
        {
            using var cursor = await Collection.FindAsync(filter).ConfigureAwait(false);
            return await cursor.ToListAsync();
        }
        /// <summary>
        /// find entities with paging
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <param name="pageIndex">page index, based on 1</param>
        /// <param name="size">number of items in page</param>
        /// <returns>collection of entity</returns>
        public virtual Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> filter, int pageIndex, int size)
        {
            return FindAsync(filter, i => i.Id, pageIndex, size);
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
        public virtual Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> filter, Expression<Func<T, object>> order, int pageIndex, int size)
        {
            return FindAsync(filter, order, pageIndex, size, true);

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
        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> filter,
            Expression<Func<T, object>> order, int pageIndex, int size, bool isDescending)
        {

            var option = new FindOptions<T>
            {
                Skip = (Math.Max(pageIndex, 1) - 1) * size,
                Limit = size,
                Sort = isDescending ? Builders<T>.Sort.Descending(order) : Builders<T>.Sort.Ascending(order)
            };
            using var cursor = await Collection.FindAsync(filter, option).ConfigureAwait(false);
            return await cursor.ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> FindAsync(FilterDefinition<T> filter,
            Expression<Func<T, object>> order, int pageIndex, int size, bool isDescending)
        {

            var option = new FindOptions<T>
            {
                Skip = (Math.Max(pageIndex, 1) - 1) * size,
                Limit = size,
                Sort = isDescending ? Builders<T>.Sort.Descending(order) : Builders<T>.Sort.Ascending(order)
            };

            using var cursor = await Collection.FindAsync(filter, option).ConfigureAwait(false);
            return await cursor.ToListAsync();
        }
        public virtual async Task<IEnumerable<T>> FindAsync(string json,
            Expression<Func<T, object>> order, int pageIndex, int size, bool isDescending)
        {

            var option = new FindOptions<T>
            {
                Skip = (Math.Max(pageIndex, 1) - 1) * size,
                Limit = size,
                Sort = isDescending ? Builders<T>.Sort.Descending(order) : Builders<T>.Sort.Ascending(order)
            };

            var filter = BsonSerializer.Deserialize<BsonDocument>(json);
            using var cursor = await Collection.FindAsync(filter, option);
            var datas = await cursor.ToListAsync().ConfigureAwait(false);
            return datas;
        }

        #endregion async Find

        #region Async FindAll

        /// <summary>
        /// fetch all items in collection
        /// </summary>
        /// <returns>collection of entity</returns>
        public virtual async Task<IEnumerable<T>> FindAllAsync()
        {
            return (await Collection.FindAsync(p => true)).ToEnumerable();
        }

        /// <summary>
        /// fetch all items in collection with paging
        /// </summary>
        /// <param name="pageIndex">page index, based on 0</param>
        /// <param name="size">number of items in page</param>
        /// <returns>collection of entity</returns>
        public virtual Task<IEnumerable<T>> FindAllAsync(int pageIndex, int size)
        {
            return FindAllAsync(i => i.Id, pageIndex, size);
        }

        /// <summary>
        /// fetch all items in collection with paging and ordering
        /// default ordering is descending
        /// </summary>
        /// <param name="order">ordering parameters</param>
        /// <param name="pageIndex">page index, based on 0</param>
        /// <param name="size">number of items in page</param>
        /// <returns>collection of entity</returns>
        public virtual Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, object>> order, int pageIndex, int size)
        {
            return FindAllAsync(order, pageIndex, size, true);
        }

        /// <summary>
        /// fetch all items in collection with paging and ordering in direction
        /// </summary>
        /// <param name="order">ordering parameters</param>
        /// <param name="pageIndex">page index, based on 0</param>
        /// <param name="size">number of items in page</param>
        /// <param name="isDescending">ordering direction</param>
        /// <returns>collection of entity</returns>
        public virtual Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, object>> order, int pageIndex, int size, bool isDescending)
        {

            return RetryAsync(async () =>
            {
                var option = new FindOptions<T>
                {
                    Skip = (Math.Max(pageIndex, 1) - 1) * size,
                    Limit = size,
                    Sort = isDescending ? Builders<T>.Sort.Descending(order) : Builders<T>.Sort.Ascending(order)
                };
                using var cursor = await Collection.FindAsync(Filter.Empty, option).ConfigureAwait(false);
                return (await cursor.ToListAsync()).AsEnumerable();
            });

        }

        #endregion Async FindAll


        #region Async First

        /// <summary>
        /// get first item in collection
        /// </summary>
        /// <returns>entity of <typeparamref name="T"/></returns>
        public virtual Task<T> FirstAsync()
        {
            return Collection.AsQueryable().FirstOrDefaultAsync();
        }

        /// <summary>
        /// get first item in query
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <returns>entity of <typeparamref name="T"/></returns>
        public virtual Task<T> FirstAsync(FilterDefinition<T> filter)
        {
            return Collection.Find(filter).FirstOrDefaultAsync();
        }

        /// <summary>
        /// get first item in query
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <returns>entity of <typeparamref name="T"/></returns>
        public virtual Task<T> FirstAsync(Expression<Func<T, bool>> filter)
        {
            return FirstAsync(filter, i => i.Id);
        }

        /// <summary>
        /// get first item in query with order
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <param name="order">ordering parameters</param>
        /// <returns>entity of <typeparamref name="T"/></returns>
        public virtual Task<T> FirstAsync(Expression<Func<T, bool>> filter, Expression<Func<T, object>> order)
        {
            return FirstAsync(filter, order, false);
        }

        /// <summary>
        /// get first item in query with order and direction
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <param name="order">ordering parameters</param>
        /// <param name="isDescending">ordering direction</param>
        /// <returns>entity of <typeparamref name="T"/></returns>
        public virtual async Task<T> FirstAsync(Expression<Func<T, bool>> filter, Expression<Func<T, object>> order, bool isDescending)
        {
            var r = await FindAsync(filter, order, 1, 1, isDescending);
            return r.FirstOrDefault();
        }

        #endregion First

        #region Async Get


        /// <summary>
        /// get by selector
        /// </summary>
        /// <param name="id">selector value</param>
        /// <returns>entity of <typeparamref name="T"/></returns>
        public virtual Task<T> GetAsync(string id)
        {
            return string.IsNullOrEmpty(id)
                ? Task.FromResult(default(T))
                : Collection.Find(p => p.Id == id).SingleOrDefaultAsync();
        }
        public virtual Task<T> GetAsync(Expression<Func<T, bool>> filter)
        {
            return FirstAsync(filter);
        }
        #endregion Get

        #region Async Insert


        /// <summary>
        /// insert entity
        /// </summary>
        /// <param name="entity">entity</param>
        public virtual Task InsertAsync(T entity)
        {
            return RetryAsync(() => Collection.InsertOneAsync(entity));
        }


        /// <summary>
        /// insert entity collection
        /// </summary>
        /// <param name="entities">collection of entities</param>
        public virtual Task InsertAsync(IEnumerable<T> entities)
        {
            return RetryAsync(() => Collection.InsertManyAsync(entities));
        }
        #endregion Insert

        #region Async Last

        /// <summary>
        /// get first item in collection
        /// </summary>
        /// <returns>entity of <typeparamref name="T"/></returns>
        public virtual Task<T> LastAsync()
        {
            return Query().SortByDescending(i => i.Id).FirstOrDefaultAsync();
        }

        /// <summary>
        /// get last item in query
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <returns>entity of <typeparamref name="T"/></returns>
        public virtual Task<T> LastAsync(FilterDefinition<T> filter)
        {
            return Query(filter).SortByDescending(i => i.Id).FirstOrDefaultAsync();
        }

        /// <summary>
        /// get last item in query
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <returns>entity of <typeparamref name="T"/></returns>
        public virtual Task<T> LastAsync(Expression<Func<T, bool>> filter)
        {
            return LastAsync(filter, i => i.Id);
        }

        /// <summary>
        /// get last item in query with order
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <param name="order">ordering parameters</param>
        /// <returns>entity of <typeparamref name="T"/></returns>
        public virtual Task<T> LastAsync(Expression<Func<T, bool>> filter, Expression<Func<T, object>> order)
        {
            return LastAsync(filter, order, false);
        }

        /// <summary>
        /// get last item in query with order and direction
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <param name="order">ordering parameters</param>
        /// <param name="isDescending">ordering direction</param>
        /// <returns>entity of <typeparamref name="T"/></returns>
        public virtual Task<T> LastAsync(Expression<Func<T, bool>> filter, Expression<Func<T, object>> order, bool isDescending)
        {
            return FirstAsync(filter, order, !isDescending);
        }

        #endregion Last


        #region Async Update 

        public virtual Task<bool> UpdateAsync(T entity)
        {
            return RetryAsync(async () =>
            {
                return (await Collection.ReplaceOneAsync(p => p.Id == entity.Id, entity)).IsAcknowledged;
            });

        }
        /// <summary>
        /// 使用replace更新
        /// </summary>
        /// <param name="id"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual Task<bool> ReplaceAsync(string id, T entity)
        {
            entity.Id = id;
            return RetryAsync(async () =>
            {
                return (await Collection.ReplaceOneAsync(p => p.Id == id, entity)).IsAcknowledged;
            });

        }
        /// <summary>
        /// update a property field in an entity
        /// </summary>
        /// <typeparam name="TField">field type</typeparam>
        /// <param name="entity">entity</param>
        /// <param name="field">field</param>
        /// <param name="value">new value</param>
        public virtual Task<bool> UpdateAsync<TField>(T entity, Expression<Func<T, TField>> field, TField value)
        {
            return UpdateAsync(entity, Updater.Set(field, value));
        }

        public virtual Task<bool> UpdateAsync<TField>(string id, Expression<Func<T, TField>> field, TField value)
        {
            return UpdateAsync(id, Updater.Set(field, value));
        }

        /// <summary>
        /// update multiple propertys
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updates"></param>
        /// <returns></returns>
        public virtual async Task<bool> UpdateAsync(string id, IDictionary<Expression<Func<T, object>>, object> updates)
        {
            var dict = updates.ToDictionary(k => k.Key.GetMemberExpression().Member.Name, v => v.Value);
            var task = Collection.UpdateOneAsync(new BsonDocument("_id", ObjectId.Parse(id)),
                new BsonDocument("$set", new BsonDocument(dict)), new UpdateOptions { IsUpsert = false });
            return (await task).ModifiedCount > 0;
        }

        /// <summary>
        /// update an entity with updated fields
        /// </summary>
        /// <param name="id">selector</param>
        /// <param name="updates">updated field(s)</param>
        public virtual Task<bool> UpdateAsync(string id, params UpdateDefinition<T>[] updates)
        {
            return UpdateAsync(Filter.Eq(i => i.Id, id), updates);
        }


        /// <summary>
        /// update an entity with updated fields
        /// </summary>
        /// <param name="entity">entity</param>
        /// <param name="updates">updated field(s)</param>
        public virtual Task<bool> UpdateAsync(T entity, params UpdateDefinition<T>[] updates)
        {
            return UpdateAsync(entity.Id, updates);
        }

        /// <summary>
        /// update a property field in entities
        /// </summary>
        /// <typeparam name="TField">field type</typeparam>
        /// <param name="filter">filter</param>
        /// <param name="field">field</param>
        /// <param name="value">new value</param>
        public virtual Task<bool> UpdateAsync<TField>(FilterDefinition<T> filter, Expression<Func<T, TField>> field, TField value)
        {
            return UpdateAsync(filter, Updater.Set(field, value));

        }


        /// <summary>
        /// update found entities by filter with updated fields
        /// </summary>
        /// <param name="filter">collection filter</param>
        /// <param name="updates">updated field(s)</param>
        public virtual Task<bool> UpdateAsync(FilterDefinition<T> filter, params UpdateDefinition<T>[] updates)
        {
            return RetryAsync(async () =>
            {
                var update = Updater.Combine(updates)
                    .CurrentDate(i => i.ModifiedOn);
                var updateOption = new UpdateOptions
                {
                    IsUpsert = false //更新文档时，如果文档不存在是否自动插入
                };
                var rs = await Collection.UpdateManyAsync(filter, update, updateOption);
                return rs.IsAcknowledged;
            });
        }

        /// <summary>
        /// update found entities by filter with updated fields
        /// </summary>
        /// <param name="filter">collection filter</param>
        /// <param name="updates">updated field(s)</param>
        public virtual Task<bool> UpdateAsync(Expression<Func<T, bool>> filter, params UpdateDefinition<T>[] updates)
        {
            return RetryAsync(async () =>
           {
               var update = Updater.Combine(updates).CurrentDate(i => i.ModifiedOn);
               var rs = await Collection.UpdateManyAsync(filter, update);
               return rs.IsAcknowledged;
           });
        }

        /// <summary>
        /// 批量更新
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public virtual async Task UpdateManyAsync(IEnumerable<T> entities)
        {
            var updates = new List<WriteModel<T>>();
            var filterBuilder = Builders<T>.Filter;

            foreach (var doc in entities)
            {
                var filter = filterBuilder.Where(x => x.Id == doc.Id);
                updates.Add(new ReplaceOneModel<T>(filter, doc));
            }

            await this.Collection.BulkWriteAsync(updates);
        }


        #endregion Async Update

        #endregion CRUD

        #region Utils


        public virtual Task<bool> AnyAsync(Expression<Func<T, bool>> filter)
        {
            return Collection.Find(filter).AnyAsync();
        }
        #region Count

        /// <summary>
        /// get number of filtered documents
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <returns>number of documents</returns>
        public virtual Task<long> CountAsync(Expression<Func<T, bool>> filter)
        {
            return Collection.CountDocumentsAsync(filter);
        }

        /// <summary>
        /// get number of documents in collection
        /// </summary>
        /// <returns>number of documents</returns>
        public virtual Task<long> CountAsync()
        {

            return Collection.CountDocumentsAsync(Filter.Empty);

        }
        #endregion Count
        #region SumAsync

        /// <summary>
        /// get number of filtered documents
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <param name="selector"></param>
        /// <returns>number of documents</returns>
        public virtual Task<double> SumAsync(Expression<Func<T, bool>> filter, Expression<Func<T, double>> selector)
        {
            return Collection.AsQueryable().Where(filter).SumAsync(selector);
        }

        /// <summary>
        /// get number of documents in collection
        /// </summary>
        /// <returns>number of documents</returns>
        public virtual Task<double> SumAsync(Expression<Func<T, double>> selector)
        {

            return Collection.AsQueryable().SumAsync(selector);

        }

        public Task<int> SumAsync(Expression<Func<T, bool>> filter, Expression<Func<T, int>> selector)
        {
            return Collection.AsQueryable().Where(filter).SumAsync(selector);
        }

        public Task<int> SumAsync(Expression<Func<T, int>> selector)
        {
            return Collection.AsQueryable().SumAsync(selector);
        }

        public Task<long> SumAsync(Expression<Func<T, bool>> filter, Expression<Func<T, long>> selector)
        {
            return Collection.AsQueryable().Where(filter).SumAsync(selector);
        }

        public Task<long> SumAsync(Expression<Func<T, long>> selector)
        {
            return Collection.AsQueryable().SumAsync(selector);
        }

        public async Task PageExecuteAsync(int size,
            Func<int, IEnumerable<T>, Task> pageCallTask)
        {
            var total = await Collection.CountDocumentsAsync(Filter.Empty);
            var pageCount = (total + size - 1) / size;
            var pageIndex = 1;
            while (pageIndex <= pageCount)
            {
                var pageDatas = await FindAsync(Filter.Empty, o => o.Id, pageIndex, size, false);
#if DEBUG
                Trace.WriteLine($"PageExecuteAsync({typeof(T).Name} Total:{total}):{pageIndex}/{pageCount}  Datas Executed:{pageDatas.Count()}");
#endif
                await pageCallTask?.Invoke(pageIndex, pageDatas);
                pageIndex++;
            }
        } 
        public async Task PageExecuteAsync(int size, Expression<Func<T, bool>> filter,
            Func<int, IEnumerable<T>, Task> pageCallTask)
        {
            var total = await Collection.CountDocumentsAsync(filter);
            var pageCount = (total + size - 1) / size;
            var pageIndex = 1;
            while (pageIndex <= pageCount)
            {
                var pageDatas = await FindAsync(filter, o => o.Id, pageIndex, size, false);
#if DEBUG
                Trace.WriteLine($"PageExecuteAsync({typeof(T).Name} Total:{total}):{pageIndex}/{pageCount}  Datas Executed:{pageDatas.Count()}");
#endif
                await pageCallTask?.Invoke(pageIndex, pageDatas);
                pageIndex++;
            }
        }

        public Task<PageResult<T>> PageQueryAsync<TPageQuery, TKey>(TPageQuery pager, Expression<Func<T, bool>> filter,
            Expression<Func<T, TKey>> orderByKeySelector, bool desc = false) where TPageQuery : PageQuery, new()
        {
            var query = Collection.AsQueryable();
            if (filter != null) query = query.Where(filter);
            if (pager is OrgPageQuery oPager && oPager.OrgId.IsObjectId())
            {
                query = query.Where(_ => CreateOrgFilter(oPager.OrgId).Inject());
            }
            return query.ToPageListAsync(pager.PageIndex, pager.PageSize, orderByKeySelector, desc);
        }

        private FilterDefinition<T> CreateOrgFilter(string orgId)
        {
            return Builders<T>.Filter.Eq("_o", ObjectId.Parse(orgId));
        }

        #endregion Sum

        #endregion Utils

        protected virtual async Task<TResult> RetryAsync<TResult>(Func<Task<TResult>> action)
        {
            return await Policy
                .Handle<MongoConnectionException>(i => i.InnerException?.GetType() == typeof(IOException) ||
                                                       i.InnerException?.GetType() == typeof(SocketException))
                .RetryAsync(3)
                .ExecuteAsync(action).ConfigureAwait(false);
        }
        protected virtual async Task RetryAsync(Func<Task> action)
        {
            await Policy
                .Handle<MongoConnectionException>(i => i.InnerException?.GetType() == typeof(IOException) ||
                                                       i.InnerException?.GetType() == typeof(SocketException))
                .RetryAsync(3)
                .ExecuteAsync(action).ConfigureAwait(false);
        }

        public virtual Task<TResult> MaxAsync<TResult>(Expression<Func<T, TResult>> selector)
        {
            return Collection.AsQueryable().GroupBy(selector).MaxAsync(p => p.Key);
        }

        public virtual Task<TResult> MaxAsync<TResult>(Expression<Func<T, bool>> filter, Expression<Func<T, TResult>> selector)
        {
            return Collection.AsQueryable().Where(filter).GroupBy(selector).MaxAsync(p => p.Key);
        }

        public virtual Task<TResult> MinAsync<TResult>(Expression<Func<T, TResult>> selector)
        {
            return Collection.AsQueryable().GroupBy(selector).MinAsync(p => p.Key);
        }

        public virtual Task<TResult> MinAsync<TResult>(Expression<Func<T, bool>> filter, Expression<Func<T, TResult>> selector)
        {
            return Collection.AsQueryable().Where(filter).GroupBy(selector).MinAsync(p => p.Key);
        }
    }
}