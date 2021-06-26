namespace DeviceDefine
{
    public class Device : IDevice
    {
        #region Implementation of IDevice

        /// <summary>
        ///     设备ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///     设备名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     设备类型
        /// </summary>
        public DeviceType DeviceType { get; set; }

        /// <summary>
        ///     负载端口
        /// </summary>
        public string[] Ports { get; set; }

        #endregion
    }
}