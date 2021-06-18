using IMCommunication.Robot.Protocol.Core;

namespace IMCommunication.Robot.Protocol.Mission
{
    public class Response : Base
    {
        public bool Accept { get; set; }

        public string Log { get; set; }
    }
}

