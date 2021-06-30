using CoreData.Core;

namespace AgvcEntitys.Agv
{
    public class MrEntity : MongoEntity
    {
        public string MrId { get; set; }
        public string MrName { get; set; }
        /// <summary>
        /// 是否带手臂，不带手臂的是晶棒区机器人
        /// </summary>
        public bool Arm { get; set; }
    }
}