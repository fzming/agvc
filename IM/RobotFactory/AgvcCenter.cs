using System.Linq;
using AgvcWorkFactory.Interfaces;
using RobotDefine;

namespace AgvcWorkFactory
{
    /// <summary>
    ///     Agvc中控
    /// </summary>
    public class AgvcCenter : IAgvcCenter
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public AgvcCenter(IRobotTaskEngine taskEngine, IVirtualRobotManager virtualRobotManager)
        {
            TaskEngine = taskEngine;
            VirtualRobotManager = virtualRobotManager;
        }

        /// <summary>
        ///     任务引擎
        /// </summary>
        private IRobotTaskEngine TaskEngine { get; }

        /// <summary>
        ///     机器人管理器
        /// </summary>
        private IVirtualRobotManager VirtualRobotManager { get; }

        /// <summary>
        ///     启动入口
        /// </summary>
        public void Run()
        {
            var mrList = VirtualRobotManager.ReadMrListFromIm();
            if (mrList.Any())
            {
                mrList.ToList().ForEach(mrid =>
                {
                    VirtualRobotManager.CreateVirtualRobot(mrid, null);
                });
                VirtualRobotManager.TryRefreshMRStatus();
            }

            TaskEngine.Start(); //启动工作引擎
        }

        /// <summary>
        ///     停止
        /// </summary>
        public void Stop()
        {
            TaskEngine.Dispose();
        }
    }
}