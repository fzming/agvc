using AutoMapper;

namespace Utility.Mapper.Converters
{
    public class IntTypeConverter : ITypeConverter<string, int>
    {
        public int Convert(string source, int destination, ResolutionContext context)
        {
            int.TryParse(source, out var o);
            return o;
        }
    }
}