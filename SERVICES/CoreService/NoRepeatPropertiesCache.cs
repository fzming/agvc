using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CoreData.Core.Attributes;
using CoreData.Models;

namespace CoreService
{
    public abstract class NoRepeatPropertiesCache
    {
        private static ConcurrentDictionary<Type, IEnumerable<PropertyInfo>> PropertiesCache { get; } =
            new();
        public static List<PropertyInfo> GetNoRepeatPropertyInfos<TModel>(TModel model) where TModel : Model, new()
        {
            var type = model.GetType();
            if (PropertiesCache.TryGetValue(type, out var infos))
                return infos.ToList();
            var properties = type.GetProperties();
            infos = properties.Where(p => p.GetCustomAttribute(typeof(NoRepeatAttribute)) != null).ToList();
            PropertiesCache.TryAdd(type, infos);
            return infos.ToList();

        }
    }
}