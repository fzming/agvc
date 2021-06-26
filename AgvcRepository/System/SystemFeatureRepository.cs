using AgvcEntitys.System;
using AgvcRepository.System.Interfaces;
using CoreRepository;

namespace AgvcRepository.System
{

    public class SystemFeatureRepository : MongoRepository<SystemFeature>, ISystemFeatureRepository
    {
        public SystemFeatureRepository(IMongoUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}