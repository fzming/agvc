using System;

namespace Utility.Attributes
{
    /// <summary>
    /// 非空的特性标志
    /// </summary>
    public class NotNullAttribute : Attribute,IDescription
    {
        public NotNullAttribute()
        {
            
        }

        public NotNullAttribute(string description)
        {
            Description = description;
        }

        public string Description { get;}
    }
}
