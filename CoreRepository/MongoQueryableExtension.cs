using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CoreData;
using CoreData.Core;
using CoreData.Core.Aggregate;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Newtonsoft.Json.Linq;
using Utility;

namespace CoreRepository
{
    // ReSharper disable once InconsistentNaming
    public static class MongoQueryableExtension
    {
        public static object[] ParseObjects(this object[] values)
        {
            return values.Select(p => p.ToString().ParseObject()).ToArray();
        }
        public static object ParseObject(this string value)
        {
            try
            {

                var json = $"{{value:{value}}}";

                var jObject = JObject.Parse(json);
                if (jObject.TryGetValue("value", out var jsonValue))
                {
                    switch (jsonValue.Type)
                    {
                        case JTokenType.Integer:
                            return jsonValue.ToObject<int>();

                        case JTokenType.Array:
                            return jsonValue.ToObject<Array>();
                        case JTokenType.Float:
                            return jsonValue.ToObject<float>();
                        case JTokenType.Boolean:
                            return jsonValue.ToObject<bool>();
                        case JTokenType.Null:
                        case JTokenType.Undefined:
                            return null;
                        case JTokenType.Date:
                            return jsonValue.ToObject<DateTime>();
                        //string
                        case JTokenType.String:
                        case JTokenType.Guid:
                        case JTokenType.Uri:
                            return jsonValue.ToString();
                        case JTokenType.TimeSpan:
                            return jsonValue.ToObject<TimeSpan>();
                    }


                    return jsonValue;
                }
            }
            catch
            {
                // ignored
            }

            return value;

        }
        /// <summary>
        /// https://mikaelkoskinen.net/post/mongodb-aggregation-framework-examples-in-c
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static dynamic ToDynamic(this BsonDocument doc)
        {
            var json = BsonExtensionMethods.ToJson(doc);
            dynamic obj = JToken.Parse(json);
            return obj;
        }
        public static BsonDocument RenderToBsonDocument<T>(this FilterDefinition<T> filter)
        {
            var serializerRegistry = BsonSerializer.SerializerRegistry;
            var documentSerializer = serializerRegistry.GetSerializer<T>();
            return filter.Render(documentSerializer, serializerRegistry);
        }

        /// <summary>
        /// 分页查询(同步)
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="query">查询条件</param>
        /// <param name="pageIndex">页面索引</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="orderByKeySelector">排序字段</param>
        /// <param name="desc">排序DESC OR ASC</param>
        /// <returns></returns>
        public static PageResult<TSource> ToPageList<TSource, TKey>(this IMongoQueryable<TSource> query,
            int pageIndex, int pageSize,
            Expression<Func<TSource, TKey>> orderByKeySelector, bool desc = false) where TSource : AggregateRoot
        {

            if (orderByKeySelector != null)
            {
                query = desc ? query.OrderByDescending(orderByKeySelector) : query.OrderBy(orderByKeySelector);
            }

            return query.ToPageList(pageIndex, pageSize);

        }
        public static PageResult<TSource> ToPageList<TSource>(this IMongoQueryable<TSource> query,
            int pageIndex, int pageSize) where TSource : AggregateRoot
        {
            //分页返回
            List<TSource> datas;
            if (pageSize > 0 && pageIndex > 0)
            {
                var total = pageIndex == 1 ? AsyncHelper.RunSync(() => query.CountAsync()) : 0;

                datas = query.Skip((pageIndex - 1) * pageSize)
                        .Take(pageSize).ToList();

                return new PageResult<TSource>(
                    datas,
                    pageSize,
                    total);
            }

            //非分页返回
            datas = query.ToList();
            return new PageResult<TSource>(
                datas,
                pageSize,
                datas.Count);
        }

        public static Task<PageResult<TSource>> ToPageListAsync<TSource, TKey, TPager>(this IMongoQueryable<TSource> query, TPager paging,
            Expression<Func<TSource, TKey>> orderByKeySelector, bool desc = false)
            where TSource : class
            where TPager : PageQuery, new()
        {
            paging ??= new TPager();
            return query.ToPageListAsync(paging.PageIndex, paging.PageSize, orderByKeySelector, desc);
        }
        public static Task<PageResult<TSource>> ToPageListAsync<TSource, TPager>(this IMongoQueryable<TSource> query, TPager paging)
            where TSource : class
            where TPager : PageQuery, new()
        {
            paging ??= new TPager();
            return query.ToPageListAsync(paging.PageIndex, paging.PageSize);
        }
        /// <summary>
        /// 分页查询(异步)
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="query">查询条件</param>
        /// <param name="pageIndex">页面索引</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="orderByKeySelector">排序字段</param>
        /// <param name="desc">排序DESC OR ASC</param>
        /// <returns></returns>
        public static Task<PageResult<TSource>> ToPageListAsync<TSource, TKey>(this IMongoQueryable<TSource> query, int pageIndex, int pageSize,
            Expression<Func<TSource, TKey>> orderByKeySelector, bool desc = false) where TSource : class
        {

            if (orderByKeySelector != null)
            {
                query = desc ? query.OrderByDescending(orderByKeySelector) : query.OrderBy(orderByKeySelector);
            }

            return query.ToPageListAsync(pageIndex, pageSize);


        }
        public static async Task<PageResult<TSource>> ToPageListAsync<TSource>(this IMongoQueryable<TSource> query, int pageIndex, int pageSize) where TSource : class
        {
            //分页返回
            if (pageSize > 0 && pageIndex > 0)
            {
                //var totalTask = pageIndex == 1 ? query.CountAsync() : Task.FromResult(0); //仅第一页才统计总记录数
                var totalTask = query.CountAsync();

                var dataQuery = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);
#if DEBUG
                Trace.WriteLine(dataQuery.ToString());
#endif
                var dataTask = dataQuery.ToListAsync();

                await Task.WhenAll(totalTask, dataTask).ConfigureAwait(false);//多个线程同时运行
                return new PageResult<TSource>(
                    dataTask.Result,
                    pageSize,
                    totalTask.Result);
            }

            //非分页返回
            var datas = await query.ToListAsync().ConfigureAwait(false);
            return new PageResult<TSource>(
                datas,
                pageSize,
                datas.Count);
        }
        /*This can then be called with the following:
         *var results = await collection.AggregateByPage(
            Builders<Person>.Filter.Empty,
            Builders<Person>.Sort.Ascending(x => x.Surname),
            page: 2,
            pageSize: 5);
         *
         */
        public static async Task<PageResult<TSource>> ToPageListAsync<TSource>(
            this IMongoCollection<TSource> collection,
            FilterDefinition<TSource> filter,
            SortDefinition<TSource> sortDefinition,
            int pageIndex,
            int pageSize) where TSource : MongoEntity
        {
            if (pageIndex == 0 || pageSize == 0)
            {
                var cursor = await collection.FindAsync(filter,new FindOptions<TSource>
                {
                    Sort = sortDefinition
                });
                var datas = await cursor.ToListAsync();
                return new PageResult<TSource>
                {
                    Datas = datas,
                    Total = datas.Count,
                    PageCount = 1
                };
            }

            var countFacet = AggregateFacet.Create("count",
                PipelineDefinition<TSource, AggregateCountResult>.Create(new[]
                {
                    PipelineStageDefinitionBuilder.Count<TSource>()
                }));

            var dataFacet = AggregateFacet.Create("data",
                PipelineDefinition<TSource, TSource>.Create(new[]
                {
                    PipelineStageDefinitionBuilder.Sort(sortDefinition),
                    PipelineStageDefinitionBuilder.Skip<TSource>((pageIndex - 1) * pageSize),
                    PipelineStageDefinitionBuilder.Limit<TSource>(pageSize),
                }));


            var aggregation = await collection.Aggregate(new AggregateOptions
            {
                AllowDiskUse = true
            })
                .Match(filter)
                .Facet(countFacet, dataFacet)
                .ToListAsync();

            var count = aggregation.First()
                .Facets.First(x => x.Name == "count")
                .Output<AggregateCountResult>()?
                .FirstOrDefault()?
                .Count ?? 0;

            // var totalPages = (int)Math.Ceiling((double)count / pageSize);

            var data = aggregation.First()
                .Facets.First(x => x.Name == "data")
                .Output<TSource>().ToList();

            return new PageResult<TSource>(
                data,
                pageSize,
                (int)count);

        }
    }
}