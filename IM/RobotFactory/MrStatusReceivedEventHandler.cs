using System;

namespace RobotFactory
{
    public delegate void MrStatusReceivedEventHandler(object sender, MrStatusEventArg e);
    public delegate void MrStatusErrorEventHandler(object sender, MrStatusErrorArg e);
    public delegate void MrRequestStatusRefreshEventHandler(object sender, MrIdArg e);

    public class MrIdArg:EventArgs
    {
        public string MRID { get; set; }
    }

    public class MrStatusErrorArg : MrIdArg
    {
        public string Error { get; set; }
    }
}