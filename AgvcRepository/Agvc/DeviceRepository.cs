using AgvcEntitys.Agv;
using CoreRepository;

namespace AgvcRepository
{
    /// <summary>
    /// Stock OR EQP 仓储实现
    /// </summary>
    public class DeviceRepository :MongoRepository<Device>, IDeviceRepository
    {
        protected DeviceRepository(IMongoUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }

    public interface IDeviceRepository: IRepository<Device>
    {

    }
}