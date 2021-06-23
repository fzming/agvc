using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;

namespace CoreRepository.MapConventions
{
    internal class IgnoreManyPropertiesConvention : ConventionBase, IMemberMapConvention
    {
        public void Apply(BsonMemberMap mMap)
        {
            if (mMap.MemberType.Name == "Many`1")
            {
                mMap.SetShouldSerializeMethod(_ => false);
            }
        }
    }
}