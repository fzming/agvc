﻿using AutoMapper;
using MongoDB.Bson;

namespace Utility.Mapper.Converters
{
    public class StringIdConverter : ITypeConverter<ObjectId, string>
    {
        public string Convert(ObjectId source, string destination, ResolutionContext context)
        {
            return source.ToString();
        }
    }
}