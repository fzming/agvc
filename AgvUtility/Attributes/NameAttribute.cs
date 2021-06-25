using System;

namespace Utility.Attributes
{
    [AttributeUsage(
        AttributeTargets.Parameter | AttributeTargets.Field |
        AttributeTargets.Property)]
    public class NameAttribute : Attribute, IDescription
    {
        public NameAttribute(string description)
        {
            Description = description;
        }

        public string Description { get; }
    } 
    
}
