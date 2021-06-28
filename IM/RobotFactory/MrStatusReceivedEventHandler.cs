using System;

namespace AgvcWorkFactory
{
    public delegate void MrStatusReceivedEventHandler(object sender, MrStatusEventArg e);

    public delegate void MrStatusErrorEventHandler(object sender, MrStatusErrorArg e);

    public delegate void MrRequestStatusRefreshEventHandler(object sender, MrIdArg e);
    public delegate void MrIdleEventHandler(object sender, MrIdArg e);
    public delegate void MrTaskErrorEventHandler(object sender, MrTaskErrorArg e);
    public delegate void MrTaskCompleteEventHandler(object sender, MrTaskCompleteArg e);

    public class MrIdArg : EventArgs
    {
        public string MRID { get; set; }
    }

    public class MrStatusErrorArg : MrIdArg
    {
        public string Error { get; set; }
    }

    public class MrTaskErrorArg : MrIdArg
    {
        /// <summary>
        /// 任务ID
        /// </summary>
        public string TaskId { get; set; }
        public string Error { get; set; }
    }

    public class MrTaskCompleteArg : MrIdArg
    {
        /// <summary>
        /// 任务ID
        /// </summary>
        public string TaskId { get; set; }
    }
}