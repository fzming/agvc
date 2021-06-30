using CoreData.Core;

namespace AgvcEntitys.Agv
{
    /// <summary>
    /// Stock Or EQP
    /// </summary>
    public class Device : MongoEntity
    {

        /// <summary>
        ///     设备名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     设备类型
        /// </summary>
        public DeviceType DeviceType { get; set; }

        /// <summary>
        ///     负载端口配置
        /// </summary>
        public int[] Ports { get; set; }

    }
    public enum DeviceType
    {
        Stock,
        EQP
    }
}