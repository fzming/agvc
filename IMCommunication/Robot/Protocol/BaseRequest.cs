using IMCommunication.Robot.Protocol.Request;

namespace IMCommunication.Robot.Protocol
{
    public abstract class BaseRequest : Base
    {
        protected BaseRequest()
        {
        }

        public Response GetResponse(bool agree, string log) => 
            new Response { 
                Agree = agree,
                Log = log,
                SN = base.SN
            };

        public string MRID { get; set; }
    }
}

