using CoreRepository.Core;

namespace AgvcRepository.Entitys
{
    public class MrEntity : MongoEntity
    {
        public string MrId { get; set; }
        public string MrName { get; set; }
    }
}