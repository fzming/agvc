using System;

namespace Utility.Attributes
{
    [AttributeUsage(
        AttributeTargets.Parameter | AttributeTargets.Field |
        AttributeTargets.Property)]
    public class ColorAttribute : Attribute
    {
        public ColorAttribute(string description)
        {
            Description = description;
        }

        public ColorAttribute()
        {
        }

        public string Description { get; }
    }
}