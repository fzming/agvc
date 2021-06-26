namespace DeviceDefine
{
    /// <summary>
    ///     设备定义
    /// </summary>
    public interface IDevice
    {
        /// <summary>
        ///     设备ID
        /// </summary>
        string Id { get; set; }

        /// <summary>
        ///     设备名称
        /// </summary>
        string Name { get; set; }

        /// <summary>
        ///     设备类型
        /// </summary>
        DeviceType DeviceType { get; set; }
    }
}