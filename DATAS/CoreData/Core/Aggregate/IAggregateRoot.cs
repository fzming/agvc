using System;

namespace CoreData.Core.Aggregate
{
    /// <summary>
    /// 聚合根接口
    /// </summary>
    public interface IAggregateRoot
    {
        /// <summary>
        /// create date
        /// </summary>
        DateTime CreatedOn { get; }


        /// <summary>
        /// modify date
        /// </summary>
        // ReSharper disable once UnusedMemberInSuper.Global
        DateTime ModifiedOn { get; }
    }
}
