using Protocol;
using Protocol.Report;
using Utility;

namespace RobotFactory.Interfaces
{
    public interface IAgvReporter : ISingletonDependency
    {
        bool TryAddWatch(AgvReport agvReport);
        void RemoveWatch(string key);

        /// <summary>
        /// IMG->AGVC 汇报了状态
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        Response OnReport(BaseReport report);

        BaseReport GetReport(string key);
    }
}