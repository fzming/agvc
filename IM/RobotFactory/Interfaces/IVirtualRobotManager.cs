using System;
using System.Collections.Generic;
using RobotDefine;
using Utility;

namespace AgvcWorkFactory.Interfaces
{
    public interface IVirtualRobotManager : ISingletonDependency
    {
        IVirtualRobot FindRobot(string MRID);
        IEnumerable<IVirtualRobot> FindIdleRobots();

        /// <summary>
        ///     尝试刷新所有机器状态
        /// </summary>
        void TryRefreshMRStatus();

        /// <summary>
        ///     尝试机器状态数据
        /// </summary>
        void TryRefreshMRStatus(string MRID);

        /// <summary>
        ///     立即獲取MR狀態
        /// </summary>
        /// <param name="MRID"></param>
        /// <returns></returns>
        MRStatus GetMRStatusSync(string MRID);

        IEnumerable<string> ReadMrListFromIm();
        void CreateVirtualRobot(string mrId,Action<IVirtualRobot> createAction);
        IEnumerable<IVirtualRobot> GetAllVirtualRobots();
    }
}