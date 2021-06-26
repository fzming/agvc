using Protocol.Report;

namespace Protocol
{
    public abstract class BaseReport : Base
    {
        /// <summary>
        ///     任務編號
        /// </summary>
        public string MissionID { get; set; }

        /// <summary>
        ///     机器人编号
        /// </summary>
        public string MRID { get; set; }

        public Response GetResponse(bool received)
        {
            return new()
            {
                Received = received,
                SN = SN
            };
        }
    }
}