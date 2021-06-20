namespace Protocol.Query
{
    public class MRStatus : Base
    {
        public Response GetResponse(RobotDefine.MRStatus status) => 
            new Response { 
                SN = base.SN,
                MRStatus = status
            };
        /// <summary>
        /// 機器人編號
        /// </summary>
        public string MRID { get; set; }

        public class Response : Base
        {
            public RobotDefine.MRStatus MRStatus { get; set; }
        }
    }
}

