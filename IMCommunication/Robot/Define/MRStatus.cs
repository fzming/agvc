using System.Collections.Generic;

namespace IMCommunication.Robot.Define
{
    public class MRStatus : IMRStatus
    {
        public string ArrivedGoal { get; set; }

        public double Battery { get; set; }

        public string CurrentMissionID { get; set; }

        public string ErrorMessage { get; set; }

        public double Head { get; set; }

        public IOperatorStatus IOperatorStatus { get; set; }

        public MissionStatus MissionStatus { get; set; }

        public string MRID { get; set; }

        public List<string> Starage { get; set; }

        public double X { get; set; }

        public double Y { get; set; }
    }
}

