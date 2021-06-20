using System;
using System.ComponentModel;

namespace Messages.Transfers.Core
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field,
        Inherited = true, AllowMultiple = false)]
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
