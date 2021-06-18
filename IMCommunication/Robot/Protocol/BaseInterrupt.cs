using IMCommunication.Robot.Protocol.Interrupt;

namespace IMCommunication.Robot.Protocol
{
    public abstract class BaseInterrupt : Base
    {
        protected BaseInterrupt()
        {
        }

        public Response GetResponse(bool interrupted, string log) => 
            new Response { 
                Interrupted = interrupted,
                Log = log,
                SN = base.SN
            };

        public string MRID { get; set; }
    }
}

