using System;

namespace Utility.Attributes
{
    [AttributeUsage(
        AttributeTargets.Parameter | AttributeTargets.Field |
        AttributeTargets.Property)]
    public class ValueAttribute : Attribute, IDescription
    {
        public ValueAttribute(string description)
        {
            Description = description;
        }

        public string Description { get; }
    }
}