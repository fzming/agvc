using CoreData.Core;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.IdGenerators;

namespace CoreRepository.MapConventions
{
    /// <summary>
    /// GUID替代ObjectId
    /// </summary>
    internal sealed class GuidIdGeneratorConvention : IPostProcessingConvention
    {
        #region IPostProcessingConvention Members
        /// <summary>
        /// Post process the class map.
        /// </summary>
        /// <param name="classMap">The class map to be processed.</param>
        public void PostProcess(BsonClassMap classMap)
        {
            if (typeof(MongoEntity).IsAssignableFrom(classMap.ClassType))
            {
                classMap.IdMemberMap?.SetIdGenerator(new GuidGenerator());
            }
        }

        #endregion

        #region IConvention Members
        /// <summary>
        /// Gets the name of the convention.
        /// </summary>
        public string Name => GetType().Name;

        #endregion
    }
}