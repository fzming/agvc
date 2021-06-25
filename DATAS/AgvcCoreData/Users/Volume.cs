using System.Collections.Generic;
using System.Linq;

namespace AgvcCoreData.Users
{
    public class Volume<T>
    {
        public string Name { get; set; }
        public T Value { get; set; }
    }

    public class GenericVolume<TName,TValue>
    {
        public TName Name { get; set; }
        public TValue Value { get; set; }
    }
    public class TagGenericVolume<TTag,TName, TValue>: GenericVolume<TName, TValue>
    {
        public TTag Tag { get; set; }
    }

    public static class GenericVolumeExtension
    {
        public static List<Volume<TValue>> ToValues<TName, TValue>(this IEnumerable<GenericVolume<TName, TValue>> volumes)
        {
            return volumes.Select(p => new Volume<TValue> { Name = p.Name.ToString(), Value = p.Value }).ToList();
        }
    }
}