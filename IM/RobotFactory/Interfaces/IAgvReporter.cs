using Protocol;
using Protocol.Report;
using Utility;

namespace AgvcWorkFactory.Interfaces
{
    public interface IAgvReporter : ISingletonDependency
    {
        bool TryAddWatch(AgvReport agvReport);
        void RemoveWatch(string key);
        int GetAgvInitializeInterval();
        /// <summary>
        ///     IMG->AGVC 汇报了状态
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        BaseReport.Response OnReport(BaseReport report);
        

        BaseReport GetReport(string key);
        
    }
}