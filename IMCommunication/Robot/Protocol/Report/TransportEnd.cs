using IMCommunication.Robot.Protocol.Core;

namespace IMCommunication.Robot.Protocol.Report
{
    public class TransportEnd : BaseReport
    {
        public string Goal { get; set; }

        public int Port { get; set; }
    }
}

