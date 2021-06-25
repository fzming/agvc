using AgvcEntitys.System;
using AgvcRepository.System.Interfaces;
using CoreRepository;

namespace AgvcRepository.System
{

    public class SystemFeatureRepository : MongoRepository<SystemFeature>, ISystemFeatureRepository
    {
        protected SystemFeatureRepository(IMongoUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}