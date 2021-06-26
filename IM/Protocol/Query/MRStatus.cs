namespace Protocol.Query
{
    public class MRStatus : Base
    {
        /// <summary>
        ///     機器人編號
        /// </summary>
        public string MRID { get; set; }

        public Response GetResponse(RobotDefine.MRStatus status)
        {
            return new()
            {
                SN = SN,
                MRStatus = status
            };
        }

        public class Response : Base
        {
            public RobotDefine.MRStatus MRStatus { get; set; }
        }
    }
}