using System;
using RobotDefine;

namespace AgvcWorkFactory
{
    public class MrStatusEventArg : EventArgs
    {
        public MRStatus MrStatus { get; set; }
    }
}