using System.Text.RegularExpressions;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace CoreRepository.Converters
{
    /// <summary>
    /// 序列化时过滤emoji表情
    /// </summary>
    public class IllegalityStringSerializer : StringSerializer
    {
        private static Regex EmojRegex => new(@"(\ud83c[\udf00-\udfff])|(\ud83d[\udc00-\ude4f])|(\ud83d[\ude80-\udeff])", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
        /// <summary>Serializes a value.</summary>
        /// <param name="context">The serialization context.</param>
        /// <param name="args">The serialization args.</param>
        /// <param name="value">The object.</param>
        protected override void SerializeValue(BsonSerializationContext context, BsonSerializationArgs args, string value)
        {
            value = EmojRegex.Replace(value, "");//过滤emoji表情字符

            base.SerializeValue(context, args, value);
        }
    }
}