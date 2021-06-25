using System;
using System.ComponentModel;

namespace CoreData.Core.Attributes
{
    public class DefaultDateTimeAttribute : DefaultValueAttribute
    {
        public DefaultDateTimeAttribute()
            : base(DateTime.Now) { }

        public DefaultDateTimeAttribute(string dateTime)
            : base(DateTime.Parse(dateTime)) { }
    }
}