using System;
using System.Linq;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;

namespace CoreRepository.MapConventions
{
    /// <summary>
    /// 序列化的时候，希望可以指定时区
    /// 使用此方案后，无需在每个实体的字段中声明BsonDateTimeOptions属性段
    /// </summary>
    internal sealed class UseLocalDateTimeConvention : IMemberMapConvention
    {
        public string Name => "LocalDateTime";

        public void Apply(BsonMemberMap memberMap)
        {
            if (new []{ typeof(DateTime) , typeof(DateTime?) }.Contains(memberMap.MemberType))
            {
                var dateTimeSerializer = new DateTimeSerializer(DateTimeKind.Local);
                if (memberMap.MemberType==typeof(DateTime?))
                {
                    var nullableDateTimeSerializer = new NullableSerializer<DateTime>(dateTimeSerializer);
                    memberMap.SetSerializer(nullableDateTimeSerializer);
                }
                else
                {
                    memberMap.SetSerializer(dateTimeSerializer);
                }
            }
           
        }
    }
}