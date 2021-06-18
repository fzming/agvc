using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MesCommunication.Protocol
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
