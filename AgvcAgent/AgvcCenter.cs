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
    public class AgvcCenter
    {
        #region Singleton

        private static readonly Lazy<AgvcCenter> Instancelock = new Lazy<AgvcCenter>(() => new AgvcCenter());

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public AgvcCenter()
        {
            VirtualRobotManager = VirtualRobotManager.Instance;
            TaskEngine = new RobotTaskEngine(VirtualRobotManager);
        }

        public static AgvcCenter Instance
        {
            get
            {
                return Instancelock.Value;
            }
        }

        #endregion
        public RobotTaskEngine TaskEngine { get;  }

        public VirtualRobotManager VirtualRobotManager { get; }

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
            VirtualRobotManager.Dispose();
        }
    }
}