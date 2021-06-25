using System;

namespace Utility.Attributes
{
    

    [AttributeUsage(AttributeTargets.Property)]
    public class ModelAttribute : Attribute
    {
        public ModelAttribute(string modelName,TextType type= TextType.Text,bool isPrimaryKey=false)
        {
            Name = modelName;
            TextType = type;
            IsPrimaryKey = isPrimaryKey;
        }

        public string Name { get; set; }
        public TextType TextType {
            get;
            set;
        }
        public bool IsPrimaryKey { get; set; }
    }
}