using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using AutoMapper;
using MongoDB.Bson;
using Utility.Mapper.Converters;

namespace Utility.Mapper
{
    #region Convertes

    #endregion
    /// <summary>
    /// AutoMapper扩展帮助类
    /// </summary>
    public static class AutoMapperExtension
    {
        /// <summary>
        /// 浮点比较精度
        /// </summary>
        // ReSharper disable once IdentifierTypo
        // ReSharper disable once InconsistentNaming
        private const float LERANCE = 0.000001f;
        /// <summary>
        /// Initializes the <see cref="T:Utility.Mapper.AutoMapperExtension"/> class.
        /// </summary>
        static AutoMapperExtension()
        {

            var configuration = new MapperConfiguration(config =>
            {
                config.CreateMap<string, int>().ConvertUsing(new IntTypeConverter()); //string->int
                config.CreateMap<int, bool>().ConvertUsing(new BooleanTypeConverter());//int -> bool
                config.CreateMap<string, ObjectId>().ConvertUsing(new ObjectIdConverter());//string->objectid
                config.CreateMap<ObjectId, string>().ConvertUsing(new StringIdConverter());//objectid->string
                config.CreateMap<string, DateTime>().ConvertUsing(new DateTimeTypeConverter());//string->datetime
                                                                                               //默认类型通用转换
                config.CreateMap<long, DateTime>().ConvertUsing((s, d) => new DateTime(s));
                config.CreateMap<long, DateTime?>().ConvertUsing((s, d) => s == long.MinValue ? null : new DateTime(s));
                config.CreateMap<DateTime, long>().ConvertUsing((s, d) => s.Ticks);
                config.CreateMap<DateTime?, long>().ConvertUsing((s, d) => s?.Ticks ?? long.MinValue);
                config.CreateMap<string, string>().ConvertUsing((s, d) => s ?? string.Empty);
                //bool? 类型采用int32表示（1：true，0：false，-1：Null）
                config.CreateMap<bool?, int>().ConvertUsing((s, d) => s == null ? -1 : Convert.ToInt32(s.Value));
                config.CreateMap<int, bool?>().ConvertUsing((s, d) => s == -1 ? null : Convert.ToBoolean(s));
                config.CreateMap<int?, int>().ConvertUsing((s, d) => s ?? int.MinValue);
                config.CreateMap<int, int?>().ConvertUsing((s, d) => s == int.MinValue ? null : s);
                config.CreateMap<long?, long>().ConvertUsing((s, d) => s ?? long.MinValue);
                config.CreateMap<long, long?>().ConvertUsing((s, d) => s == long.MinValue ? null : s);
                config.CreateMap<float?, float>().ConvertUsing((s, d) => s ?? float.MinValue);
                config.CreateMap<float, float?>().ConvertUsing((s, d) => Math.Abs(s - float.MinValue) < LERANCE ? null : s);
                config.CreateMap<Guid?, string>().ConvertUsing((s, d) => s == null ? string.Empty : s.Value.ToString());
                config.CreateMap<decimal, float>().ConvertUsing((s, d) => float.Parse(s.ToString(CultureInfo.InvariantCulture)));
                config.CreateMap<decimal?, float>().ConvertUsing((s, d) => s == null ? float.MinValue : float.Parse(s.Value.ToString(CultureInfo.InvariantCulture)));
                config.CreateMap<Enum, int>().ConvertUsing((s, d) => s == null ? int.MinValue : (int)Enum.ToObject(s.GetType(), s));
            });
            configuration.AssertConfigurationIsValid();//验证映射是否成功
            configuration.CreateMapper();

        }
        private static readonly ConcurrentDictionary<string, IMapper> Mappers = new();


        private static IMapper GetMapper<TSource, TDestination>() where TDestination : class
            where TSource : class
        {
            var sourceType = typeof(TSource);
            var destType = typeof(TDestination);
            var key = sourceType.FullName + destType.FullName;
            if (Mappers.TryGetValue(key, out var mapper)) return mapper;
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TSource, TDestination>().PreserveReferences();
                //支持 property_name -> PropertyName 
                //cfg.SourceMemberNamingConvention = new PascalCaseNamingConvention();
                //cfg.DestinationMemberNamingConvention = new LowerUnderscoreNamingConvention();
            });
            mapper = GetMapper(config);
            Mappers.TryAdd(key, mapper);
            return mapper;
        }
        private static IMapper GetMapper(MapperConfiguration config) 
        {
            
            var mapper = config.CreateMapper();
            return mapper;
        }
 
        /// <summary>
        /// 集合列表类型映射
        /// </summary>
        /// <typeparam name="TSource">数据源类型</typeparam>
        /// <typeparam name="TDestination">目标对象类型</typeparam>
        /// <param name="source">数据源</param>
        /// <returns></returns>
        public static List<TDestination> MapTo<TSource, TDestination>(this IEnumerable<TSource> source)
            where TDestination : class
            where TSource : class
        {
            if (source == null) return new List<TDestination>();

            return GetMapper<TSource, TDestination>().Map<List<TDestination>>(source);
        }

        /// <summary>
        /// 集合列表类型映射
        /// </summary>
        /// <typeparam name="TSource">数据源类型</typeparam>
        /// <typeparam name="TDestination">目标对象类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="config">自定义配置</param>
        /// <returns></returns>
        public static List<TDestination> MapTo<TSource, TDestination>(this IEnumerable<TSource> source, MapperConfiguration config)
            where TDestination : class
            where TSource : class
        {
            if (source == null) return new List<TDestination>();

            return GetMapper(config).Map<List<TDestination>>(source);
        }
        public static List<TDestination> MapTo<TSource, TDestination>(this IEnumerable<TSource> source, Action<IMappingExpression<TSource, TDestination>> mappingAction)
            where TDestination : class
            where TSource : class
        {
            if (source == null) return new List<TDestination>();
            var config = new MapperConfiguration(cfg => mappingAction(cfg.CreateMap<TSource, TDestination>().PreserveReferences()));
            var mapper = config.CreateMapper();
            return mapper.Map<List<TDestination>>(source);
        }
        public static TDestination MapTo<TSource, TDestination>(this TSource source, TDestination destination, Action<IMappingExpression<TSource, TDestination>> mappingAction)
            where TSource : class
            where TDestination : class
        {
            if (source == null) return destination;

            var config = new MapperConfiguration(cfg =>
                mappingAction(cfg.CreateMap<TSource, TDestination>().PreserveReferences()));
            var mapper = config.CreateMapper();
            return mapper.Map(source, destination);
        }

        /// <summary>
        /// 类型映射
        /// </summary>
        /// <typeparam name="TSource">数据源类型</typeparam>
        /// <typeparam name="TDestination">目标对象类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="destination">目标对象</param>
        /// <returns></returns>
        public static TDestination MapTo<TSource, TDestination>(this TSource source, TDestination destination)
            where TSource : class
            where TDestination : class
        {
            return source == null ? destination : GetMapper<TSource, TDestination>().Map(source, destination);
        }

        /// <summary>
        /// 类型映射,默认字段名字一一对应
        /// </summary>
        /// <typeparam name="TDestination">转化之后的model，可以理解为viewmodel</typeparam>
        /// <typeparam name="TSource">要被转化的实体，Entity</typeparam>
        /// <param name="source">可以使用这个扩展方法的类型，任何引用类型</param>
        /// <returns>转化之后的实体</returns>
        public static TDestination MapTo<TSource, TDestination>(this TSource source)
            where TDestination : class
            where TSource : class
        {
            if (source == null) return default;

            return GetMapper<TSource, TDestination>().Map<TDestination>(source);
        }
    }
}