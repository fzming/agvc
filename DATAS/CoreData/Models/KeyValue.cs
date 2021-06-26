namespace CoreData.Models
{
    public class KeyValue
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
    }

    public class EnumKeyValue<T, TEnum>
    {
        public string Key { get; set; }
        public T Value { get; set; }
        public TEnum Type { get; set; }
    }
}