using Utility;

namespace RobotFactory.Interfaces
{
    public interface IRobotStatusWatcher : ITransientDependency
    {
        event MrStatusReceivedEventHandler MrStatusReceived;
        event MrStatusErrorEventHandler MrStatusError;
        void Watch(string MRID);
    }
}