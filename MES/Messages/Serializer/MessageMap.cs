using System;
using System.Collections.Generic;

namespace Messages.Serializer
{
    internal class MessageMap
    {
        public Type Type { get; set; }
        public List<MessageProperty> CommandProperties { get; set; }
    }
}