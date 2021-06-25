using System;

namespace CoreData.Core
{
    public abstract class AutoGuidPrimaryKey
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
    }
}