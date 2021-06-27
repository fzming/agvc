using Protocol.Request;

namespace Protocol
{
    public abstract class BaseRequest : Base
    {
        public string MissionID { get; set; }
        /// <summary>
        ///     机器人编号
        /// </summary>
        public string MRID { get; set; }

        public Response GetResponse(bool agree, string log)
        {
            return new()
            {
                Agree = agree,
                Log = log,
                SN = SN
            };
        }
    }
}