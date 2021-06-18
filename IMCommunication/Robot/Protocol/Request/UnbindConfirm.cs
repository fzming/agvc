using IMCommunication.Robot.Protocol.Core;

namespace IMCommunication.Robot.Protocol.Request
{
    public class UnbindConfirm : BaseRequest
    {
        public string MissionID { get; set; }
    }
}

