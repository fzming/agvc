using Protocol.Core;
namespace Protocol.Query
{
    public class MRStatus : Base
    {
        public Response GetResponse(MRStatus status) => 
            new Response { 
                SN = base.SN,
                MRStatus = status
            };

        public string MRID { get; set; }

        public class Response : Base
        {
            public MRStatus MRStatus { get; set; }
        }
    }
}

