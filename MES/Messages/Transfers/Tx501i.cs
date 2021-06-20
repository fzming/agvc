using Messages.Transfers.Core;

namespace Messages.Transfers
{
    /// <summary>
    /// Transfer Request（MES->AGVC）
    /// </summary>
    public class Tx501i:InputMessageBase
    {
        /// <summary>
        /// EQP ID
        /// </summary>
        [Deserialization(10,4)]
        public string eqp_id { get; set; }
        /// <summary>
        /// Stocker ID
        /// </summary>
        [Deserialization(10,5)]
        public string stk_id { get; set; }
        /// <summary>
        /// Xfer_flg
        /// </summary>
        [Deserialization(1,6)]
        public string xfer_flg { get; set; }
        /// <summary>
        /// Array count 1
        /// </summary>
        [Deserialization(3,7)]
        public string arycnt1 { get; set; }
        /// <summary>
        /// Cassette type
        /// </summary>
        [Deserialization(2,8)]
        public string car_type { get; set; }
        /// <summary>
        /// Carrier ID
        /// </summary>
        [Deserialization(13,9)]
        public string car_id { get; set; }
        /// <summary>
        /// Tray ID
        /// </summary>
        [Deserialization(13,10)]
        public string tray_id { get; set; }
        /// <summary>
        /// Lot type
        /// </summary>
        [Deserialization(1,11)]
        public string lot_type { get; set; }
        /// <summary>
        /// Cassette Lot ID
        /// </summary>
        [Deserialization(20,12)]
        public string sublot_id { get; set; }
        /// <summary>
        /// Cassette group
        /// </summary>
        [Deserialization(10,13)]
        public string carg_id { get; set; }
        /// <summary>
        /// Empty flag 
        /// </summary>
        [Deserialization(1,14)]
        public string empty_flg { get; set; }
        /// <summary>
        /// Wafer count 1 in current Sub Lot
        /// </summary>
        [Deserialization(4,15)]
        public string cur_sublot_wafcnt { get; set; }
        /// <summary>
        /// Carrier status
        /// </summary>
        [Deserialization(1,16)]
        public string carstats { get; set; }
        /// <summary>
        /// Customized Lot ID
        /// </summary>
        [Deserialization(15,17)]
        public string kemlot_id { get; set; }
        /// <summary>
        /// Transfer source EQP ID
        /// </summary>
        [Deserialization(10,18)]
        public string eqp_from { get; set; }
        /// <summary>
        /// Transfer source port ID
        /// </summary>
        [Deserialization(10,19)]
        public string port_from { get; set; }
        /// <summary>
        /// Transfer target EQP ID
        /// </summary>
        [Deserialization(10,20)]
        public string eqp_to { get; set; }
        /// <summary>
        /// Transfer target Port ID
        /// </summary>
        [Deserialization(10,21)]
        public string port_to { get; set; }
        /// <summary>
        /// MessageBase generator
        /// </summary>
        [Deserialization(20,22)]
        public string user_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Deserialization(14,23)]
        public string ticket_no { get; set; }
    }
}
