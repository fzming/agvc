using AutoMapper;
using MongoDB.Bson;

namespace Utility.Extensions.AutoMapperConverters
{
    public class ObjectIdConverter : ITypeConverter<string, ObjectId>
    {
        public ObjectId Convert(string source, ObjectId destination, ResolutionContext context)
        {
            return source != null ? ObjectId.Parse(source) : ObjectId.Empty;
        }
    }
}