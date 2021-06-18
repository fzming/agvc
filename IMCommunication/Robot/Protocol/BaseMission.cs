using IMCommunication.Robot.Protocol.Mission;

namespace IMCommunication.Robot.Protocol
{
    public abstract class BaseMission : Base
    {
        protected BaseMission()
        {
        }

        public Response GetResponse(bool accept, string log) => 
            new Response { 
                Accept = accept,
                Log = log,
                SN = base.SN
            };

        public string MissionID { get; set; }

        public string MRID { get; set; }
    }
}

