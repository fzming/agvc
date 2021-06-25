using System;
using MongoDB.Bson.Serialization.Attributes;

namespace CoreData.Core.Aggregate
{
    /// <summary>
    /// 抽象的聚合根
    /// </summary>
    public abstract class AggregateRoot : IAggregateRoot
    {

        /// <summary>
        /// 创建时间
        /// </summary>
        /// <value>记录创建时间，只初始化一次</value>
        [BsonIgnore]
        public abstract DateTime CreatedOn { get; set; }


        /// <summary>
        /// 修改时间
        /// </summary>
        /// <value>记录修改时间，每次更新将会自动更新</value>
        [BsonIgnore]
        public abstract DateTime ModifiedOn { get; set; }


    }
}
