using System;
using System.ComponentModel;

namespace Messages.Transfers.Core
{
    /// <summary>
    ///     指示MES的消息字段长度和顺序,用于自动序列化和反序列化.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    [ImmutableObject(true)]
    public sealed class DeserializationAttribute : Attribute
    {
        public DeserializationAttribute()
        {
        }

        public DeserializationAttribute(int length, int order)
        {
            Length = length;
            Order = order;
        }

        public int Length { get; set; }
        public int Order { get; set; }
    }
}