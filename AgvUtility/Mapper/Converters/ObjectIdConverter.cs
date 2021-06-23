using AutoMapper;
using MongoDB.Bson;

namespace Utility.Mapper.Converters
{
    public class ObjectIdConverter : ITypeConverter<string, ObjectId>
    {
        public ObjectId Convert(string source, ObjectId destination, ResolutionContext context)
        {

            return (source != null) ? ObjectId.Parse(source) : ObjectId.Empty;
        }
    }
}