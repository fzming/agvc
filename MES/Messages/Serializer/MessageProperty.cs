using System.Reflection;

namespace Messages.Serializer
{
    internal class MessageProperty
    {
        public int Length { get; set; }
        public string Name { get; set; }
        public PropertyInfo PropertyInfo { get; set; }
        public int Order { get; set; }
    }
}