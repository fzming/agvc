using System.Collections.Generic;

namespace IMCommunication.Robot.Protocol.Mission
{
    public class Move : BaseMission
    {
        public List<string> Goals { get; set; }
    }
}

