namespace IMCommunication.Robot.Protocol.Query
{
    public class MRStatus : Base
    {
        public Response GetResponse(Define.MRStatus status) => 
            new Response { 
                SN = base.SN,
                MRStatus = status
            };

        public string MRID { get; set; }

        public class Response : Base
        {
            public Define.MRStatus MRStatus { get; set; }
        }
    }
}

