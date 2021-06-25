using CoreData.Core.Aggregate;
using MongoDB.Bson.Serialization.Attributes;

namespace CoreData.Core
{
    /// <summary>
    /// 主键抽象实体基类
    /// </summary>
    /// <typeparam name="TPrimaryKey">主键类型</typeparam>
    public abstract class PrimaryAggregateEntity<TPrimaryKey> : AggregateRoot
    {
        /// <summary>
        /// 主键
        /// </summary>
        [BsonIgnore] public abstract TPrimaryKey Id { get; set; }
        public override bool Equals(object obj)
        {
            var compareTo = obj as PrimaryAggregateEntity<TPrimaryKey>;

            if (ReferenceEquals(this, compareTo)) return true;
            if (ReferenceEquals(null, compareTo)) return false;

            return Id.Equals(compareTo.Id);
        }

        public static bool operator ==(PrimaryAggregateEntity<TPrimaryKey> a, PrimaryAggregateEntity<TPrimaryKey> b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
                return true;

            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(PrimaryAggregateEntity<TPrimaryKey> a, PrimaryAggregateEntity<TPrimaryKey> b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return GetType().GetHashCode() * 907 + Id.GetHashCode();
        }

        public override string ToString()
        {
            return GetType().Name + " [Id=" + Id + "]";
        }
    }
}