using Messages.Transfers.Core;

namespace Messages.Transfers
{
    /// <summary>
    /// Location Change Report
    /// </summary>
    public class Txa01i : MessageBase
    {
        /// <summary>
        /// car_id
        /// </summary>
        [Deserialization(13, 4)]
        public string car_id { get; set; }
        /// <summary>
        /// tray_id
        /// </summary>
        [Deserialization(1, 5)]
        public string tray_id { get; set; }
        /// <summary>
        /// bond_flag
        /// </summary>
        [Deserialization(1, 7)]
        public string bond_flag { get; set; }
        /// <summary>
        /// block_id
        /// </summary>
        [Deserialization(10, 8)]
        public string block_id { get; set; }
    }
}