using AutoMapper;

namespace Utility.Extensions.AutoMapperConverters
{
    public class BooleanTypeConverter : ITypeConverter<int, bool>
    {
        public bool Convert(int source, bool destination, ResolutionContext context)
        {
            return source > 0;
        }
    }
}