using System;
using System.Linq;
using System.Threading;
using AgvUtility;
using Messages.Parser;
using RobotDefine;
using RobotFactory;
using RobotFactory.Tasks;

namespace AgvcAgent
{
    public static class AgvcCenter
    {
        public static RobotTaskEngine TaskEngine { get; set; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public static void Run()
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
            TaskEngine = new RobotTaskEngine();
            TaskEngine.Start(); //启动工作引擎
        }

        public static void Stop()
        {
            TaskEngine.Dispose();
        }
    }
}