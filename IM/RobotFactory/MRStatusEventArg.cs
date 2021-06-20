using System;
using RobotDefine;

namespace RobotFactory
{
    public class MrStatusEventArg : EventArgs
    {
        public MRStatus MrStatus { get; set; }
    }
}