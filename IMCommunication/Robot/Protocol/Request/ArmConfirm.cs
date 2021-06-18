namespace IMCommunication.Robot.Protocol.Request
{
    public class ArmConfirm : BaseRequest
    {
        public string Goal { get; set; }

        public string MissionID { get; set; }

        public int Port { get; set; }
    }
}

