namespace MesCommunication.Protocol
{
    /// <summary>
    /// Location Change Report(MES->AGVC 允许移动位置请求）
    /// </summary>
    public class Txd27o:OutputCommand
    {
        /// <summary>
        /// arycnt1
        /// </summary>
        [Deserialization(3,5)]
        public string arycnt1 { get; set; }
        /// <summary>
        /// action
        /// </summary>
        [Deserialization(1,6)]
        public string action { get; set; }
        /// <summary>
        /// stk_id
        /// </summary>
        [Deserialization(10,7)]
        public string stk_id { get; set; }
        /// <summary>
        /// car_type
        /// </summary>
        [Deserialization(2,8)]
        public string car_type { get; set; }
        /// <summary>
        /// car_id
        /// </summary>
        [Deserialization(13,9)]
        public string car_id { get; set; }
        /// <summary>
        /// tray_id
        /// </summary>
        [Deserialization(13,10)]
        public string tray_id { get; set; }
    }
}