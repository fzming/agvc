namespace MesCommunication.Protocol
{
    /// <summary>
    /// Location Change Report(AGVC->MES 请求移动位置）
    /// </summary>
    public class Txd27i : InputCommand
    {
        /// <summary>
        /// ticket_no
        /// </summary>
        [Deserialization(14, 4)]
        public string ticket_no { get; set; }
        /// <summary>
        /// eqp_id
        /// </summary>
        [Deserialization(10, 5)]
        public string eqp_id { get; set; }
        /// <summary>
        /// eqp_port_id
        /// </summary>
        [Deserialization(10, 6)]
        public string eqp_port_id { get; set; }
        /// <summary>
        /// car_type
        /// </summary>
        [Deserialization(2, 7)]
        public string car_type { get; set; }
        /// <summary>
        /// car_id
        /// </summary>
        [Deserialization(13, 8)]
        public string car_id { get; set; }
        /// <summary>
        /// tray_id
        /// </summary>
        [Deserialization(13, 9)]
        public string tray_id { get; set; }
        /// <summary>
        /// tran_stat
        /// MI:Manual In
        /// MO:Manual Out
        /// AI:Auto In
        /// AO:Auto Out
        /// XS:Trans Start
        /// XC:Trans Complete
        /// </summary>
        [Deserialization(2, 10)]
        public string tran_stat { get; set; }
        /// <summary>
        /// lot_type
        /// </summary>
        [Deserialization(1, 11)]
        public string lot_type { get; set; }
        /// <summary>
        /// sublot_id
        /// </summary>
        [Deserialization(20, 12)]
        public string sublot_id { get; set; }
        /// <summary>
        /// pattern
        /// </summary>
        [Deserialization(1, 13)]
        public string pattern { get; set; }
    }
}