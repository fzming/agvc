using Messages.Transfers.Core;

namespace Messages.Transfers
{
    public class Tx501o : OutputMessageBase
    {
        /// <summary>
        ///     EQP ID
        /// </summary>
        [Deserialization(10, 5)]
        public string eqp_id { get; set; }

        /// <summary>
        ///     Stocker ID
        /// </summary>
        [Deserialization(10, 6)]
        public string stk_id { get; set; }

        /// <summary>
        ///     Array count 1
        /// </summary>
        [Deserialization(3, 7)]
        public string arycnt1 { get; set; }

        /// <summary>
        ///     Return code 2
        /// </summary>
        [Deserialization(6, 8)]
        public string retcode2 { get; set; }

        /// <summary>
        ///     SQL error code 2
        /// </summary>
        [Deserialization(6, 9)]
        public string sqlcode2 { get; set; }

        /// <summary>
        ///     Cassette type
        /// </summary>
        [Deserialization(2, 10)]
        public string car_type { get; set; }

        /// <summary>
        ///     Carrier ID
        /// </summary>
        [Deserialization(13, 11)]
        public string car_id { get; set; }

        /// <summary>
        ///     Tray ID
        /// </summary>
        [Deserialization(13, 12)]
        public string tray_id { get; set; }

        /// <summary>
        ///     Lot type
        /// </summary>
        [Deserialization(1, 13)]
        public string lot_type { get; set; }

        /// <summary>
        ///     Cassette Lot ID
        /// </summary>
        [Deserialization(20, 14)]
        public string sublot_id { get; set; }

        /// <summary>
        ///     Transfer source EQP ID
        /// </summary>
        [Deserialization(10, 15)]
        public string eqp_from { get; set; }

        /// <summary>
        ///     Transfer source port ID
        /// </summary>
        [Deserialization(10, 16)]
        public string port_from { get; set; }

        /// <summary>
        ///     Transfer target EQP ID
        /// </summary>
        [Deserialization(10, 17)]
        public string eqp_to { get; set; }

        /// <summary>
        ///     Transfer target Port ID
        /// </summary>
        [Deserialization(10, 18)]
        public string port_to { get; set; }
    }
}