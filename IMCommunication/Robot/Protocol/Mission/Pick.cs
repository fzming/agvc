﻿namespace IMCommunication.Robot.Protocol.Mission
{
    public class Pick : BaseMission
    {
        public string BoxID { get; set; }

        public string Goal { get; set; }

        public int Port { get; set; }

        public int WaferCount { get; set; }
    }
}
