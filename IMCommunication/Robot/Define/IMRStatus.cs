using System.Collections.Generic;

namespace IMCommunication.Robot.Define
{
    public interface IMRStatus
    {
        string ArrivedGoal { get; }

        double Battery { get; }

        string CurrentMissionID { get; }

        string ErrorMessage { get; }

        double Head { get; }

        IOperatorStatus IOperatorStatus { get; }

        MissionStatus MissionStatus { get; }

        string MRID { get; }

        List<string> Starage { get; }

        double X { get; }

        double Y { get; }
    }
}

