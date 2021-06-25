using System;

namespace Utility.Attributes
{
    [AttributeUsage(
        AttributeTargets.Parameter | AttributeTargets.Field |
        AttributeTargets.Property)]
    public class FormatAttribute : Attribute,IDescription
    {
        public FormatAttribute(string description)
        {
            Description = description;
        }

        public string Description { get; }
    }
}
