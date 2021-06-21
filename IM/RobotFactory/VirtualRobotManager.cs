using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using AgvUtility;

using RobotDefine;

namespace RobotFactory
{
    public static class VirtualRobotManager
    {

        private static RobotStatusWatcher statusWatcher;

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        static VirtualRobotManager()
        {
            statusWatcher = new RobotStatusWatcher();
            statusWatcher.MrStatusReceived += StatusWatcher_MrStatusReceived;
        }
        private static List<VirtualRobot> VirtualRobots { get; set; } = new List<VirtualRobot>();

        public static VirtualRobot FindRobot(string MRID)
        {
            return VirtualRobots.FirstOrDefault(p => p?.MRStatus.MRID == MRID);
        }

        public static VirtualRobot FindIdleRobot()
        {
            var idleRobot = VirtualRobots.FirstOrDefault(p =>
                p.TaskCount == 0 && p.IsIdle());
            return idleRobot;
        }
        public static Task<VirtualRobot> FindIdleRobotAsync()
        {
            return Task.Run(() =>
          {
              var idleRobot = VirtualRobots.FirstOrDefault(p =>
                  p.TaskCount == 0 && p.IsIdle());
              if (idleRobot == null)
              {
                  TryRefreshMRStatus();
              }

              return idleRobot;
          });

        }
        /// <summary>
        /// 尝试刷新所有机器状态
        /// </summary>
        public static void TryRefreshMRStatus()
        {
            foreach (var robot in VirtualRobots)
            {
                TryRefreshMRStatus(robot.MRStatus.MRID);
            }
        }
        /// <summary>
        /// 立即獲取MR狀態
        /// </summary>
        /// <param name="MRID"></param>
        /// <returns></returns>
        public static MRStatus GetMRStatusSync(string MRID)
        {
            // var response = WS.Dispatch<Protocol.Query.MRStatus.Response>(new Protocol.Query.MRStatus
            // {
            //     MRID = MRID
            // });
            var response = AsyncHelper.RunSync(() => WS.DispatchAsync<Protocol.Query.MRStatus.Response>(new Protocol.Query.MRStatus
            {
                MRID = MRID
            }));
            var mrStatus = response.MRStatus;
            var robot = FindRobot(mrStatus.MRID);
            if (robot != null) robot.MRStatus = mrStatus;
            return mrStatus;
        }

        public static List<string> ReadMrListFromIm()
        {
            var response = AsyncHelper.RunSync(() => WS.DispatchAsync<Protocol.Query.MRList.Response>(new Protocol.Query.MRList()));
            return response.MRIDs;
        }
        /// <summary>
        /// 尝试机器状态数据
        /// </summary>
        public static void TryRefreshMRStatus(string MRID)
        {
            statusWatcher.Watch(MRID);
        }

        private static void StatusWatcher_MrStatusReceived(object sender, MrStatusEventArg e)
        {
            // Console.WriteLine("[StatusWatcher_MrStatusReceived]");
            // Console.WriteLine(e.MrStatus.ToJson());
            var robot = FindRobot(e.MrStatus.MRID);
            robot?.OnMRStatusChange(e.MrStatus);
        }

        public static void Dispose()
        {
            statusWatcher?.Stop();
        }

        public static void AddVirtualRobot(VirtualRobot virtualRobot)
        {
            if (FindRobot(virtualRobot.MRStatus.MRID) == null)
            {
                virtualRobot.OnMrRequestStatusRefresh += (sender, e) =>
                {
                    TryRefreshMRStatus(e.MRID);
                };
                VirtualRobots.Add(virtualRobot);
            }
        }


        public static IEnumerable<VirtualRobot> GetAllVirtualRobots()
        {
            return VirtualRobots;
        }
    }
}