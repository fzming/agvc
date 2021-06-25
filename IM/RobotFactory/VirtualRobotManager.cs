using System;
using System.Collections.Generic;
using System.Linq;
using AgvcWorkFactory.Interfaces;
using RobotDefine;
using Utility;

namespace AgvcWorkFactory
{
    public class VirtualRobotManager : IVirtualRobotManager
    {
        private readonly IRobotStatusWatcher _statusWatcher;
        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public VirtualRobotManager(IRobotStatusWatcher statusWatcher)
        {
            this._statusWatcher = statusWatcher;
            this._statusWatcher.MrStatusReceived += StatusWatcher_MrStatusReceived1;
        }

        private void StatusWatcher_MrStatusReceived1(object sender, MrStatusEventArg e)
        {
            // Console.WriteLine("[StatusWatcher_MrStatusReceived]");
            // Console.WriteLine(e.MrStatus.ToJson());
            var robot = FindRobot(e.MrStatus.MRID);
            robot?.OnMRStatusChange(e.MrStatus);
        }

        private List<VirtualRobot> VirtualRobots { get; set; } = new List<VirtualRobot>();

        public VirtualRobot FindRobot(string MRID)
        {
            return VirtualRobots.FirstOrDefault(p => p?.MRStatus.MRID == MRID);
        }

        public IEnumerable<VirtualRobot> FindIdleRobots()
        {
            var idleRobots = new List<VirtualRobot>();
            Console.WriteLine($"-----------------FindIdleRobots-------------------");
            VirtualRobots.ForEach(robot =>
            {
                Console.WriteLine($"-[{robot.MRStatus.MRID}] TaskCount:{robot.TaskCount} Idle:{robot.IsIdle()} Working:{robot.Working} State:{robot.State}");
                if (robot.IsRobotReadyWork())
                {
                    idleRobots.Add(robot);
                }
                else
                {
                    TryRefreshMRStatus(robot.MRStatus.MRID);
                }
            });
            Console.WriteLine($"--------------------------------------------------");
            return idleRobots;
        }
        /// <summary>
        /// 尝试刷新所有机器状态
        /// </summary>
        public void TryRefreshMRStatus()
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
        public MRStatus GetMRStatusSync(string MRID)
        {
            // var response = WS.Dispatch<Protocol.Query.MRStatus.Response>(new Protocol.Query.MRStatus
            // {
            //     MRID = MRID
            // });
            var response = AsyncHelper.RunSync(() => WS.DispatchAsync<Protocol.Query.MRStatus.Response>(new Protocol.Query.MRStatus
            {
                MRID = MRID
            }));
            var mrStatus = response?.MRStatus;
            if (mrStatus == null)
            {
                return null;
            }
            var robot = FindRobot(mrStatus.MRID);
            if (robot != null) robot.MRStatus = mrStatus;
            return mrStatus;
        }

        public IEnumerable<string> ReadMrListFromIm()
        {
            var response = AsyncHelper.RunSync(() => WS.DispatchAsync<Protocol.Query.MRList.Response>(new Protocol.Query.MRList()));

            return response?.MRIDs ?? Enumerable.Empty<string>();
        }
        /// <summary>
        /// 尝试机器状态数据(异步)
        /// </summary>
        public void TryRefreshMRStatus(string MRID)
        {
            _statusWatcher.Watch(MRID);
        }

        public void AddVirtualRobot(VirtualRobot virtualRobot)
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

        public IEnumerable<VirtualRobot> GetAllVirtualRobots()
        {
            return VirtualRobots;
        }

    }
}