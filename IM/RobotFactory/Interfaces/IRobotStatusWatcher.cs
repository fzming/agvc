using Utility;

namespace AgvcWorkFactory.Interfaces
{
    public interface IRobotStatusWatcher : ITransientDependency
    {
        event MrStatusReceivedEventHandler MrStatusReceived;
        event MrStatusErrorEventHandler MrStatusError;
        void Watch(string MRID);
    }
}