using System;

namespace Utility.Attributes
{
    [AttributeUsage(
        AttributeTargets.Parameter | AttributeTargets.Field |
        AttributeTargets.Property)]
    public class TagAttribute : Attribute, IDescription
    {
        public TagAttribute(string description)
        {
            Description = description;
        }

        public string Description { get; }
    }
}