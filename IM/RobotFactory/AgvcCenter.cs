using System.Linq;
using AgvcWorkFactory.Interfaces;
using RobotDefine;

namespace AgvcWorkFactory
{
    public class AgvcCenter : IAgvcCenter
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public AgvcCenter(IRobotTaskEngine taskEngine, IVirtualRobotManager virtualRobotManager)
        {
            TaskEngine = taskEngine;
            VirtualRobotManager = virtualRobotManager;
        }

        private IRobotTaskEngine TaskEngine { get; }

        private IVirtualRobotManager VirtualRobotManager { get; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public void Run()
        {

            var mrList = VirtualRobotManager.ReadMrListFromIm();
            if (mrList.Any())
            {
                mrList.ToList().ForEach(mrid =>
                {
                    VirtualRobotManager.AddVirtualRobot(new VirtualRobot
                    {
                        MRStatus = new MRStatus
                        {
                            MRID = mrid
                        }
                    });
                });
                VirtualRobotManager.TryRefreshMRStatus();
            }

            TaskEngine.Start(); //启动工作引擎
        }

        public void Stop()
        {
            TaskEngine.Dispose();
        }
    }
}