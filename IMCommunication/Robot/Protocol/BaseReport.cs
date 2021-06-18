using IMCommunication.Robot.Protocol.Report;

namespace IMCommunication.Robot.Protocol
{
    public abstract class BaseReport : Base
    {
        protected BaseReport()
        {
        }

        public Response GetResponse(bool received) => 
            new Response { 
                Received = received,
                SN = base.SN
            };

        public string MissionID { get; set; }

        public string MRID { get; set; }
    }
}

