using System;
using System.Collections.Generic;
using System.Linq;
using AgvcWorkFactory.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Protocol.Query;
using Utility.Helpers;
using MRStatus = RobotDefine.MRStatus;

namespace AgvcWorkFactory
{
    /// <summary>
    ///     虚拟机器人管理器
    /// </summary>
    public class VirtualRobotManager : IVirtualRobotManager
    {
        private IWS WS { get; }
        private IServiceProvider ServiceProvider { get; }
        private readonly IRobotStatusWatcher _statusWatcher;
        private readonly object locker = new object();
        public VirtualRobotManager(IRobotStatusWatcher statusWatcher, IWS ws, IServiceProvider serviceProvider)
        {
            WS = ws;
            ServiceProvider = serviceProvider;
            _statusWatcher = statusWatcher;
            _statusWatcher.MrStatusReceived += StatusWatcher_MrStatusReceived1;
        }

        /// <summary>
        ///     当前虚拟机器人列表
        /// </summary>
        private List<IVirtualRobot> VirtualRobots { get; } = new();

        /// <summary>
        ///     根据MRID查找指定机器人
        /// </summary>
        /// <param name="MRID"></param>
        /// <returns></returns>
        public IVirtualRobot FindRobot(string MRID)
        {
            return VirtualRobots.FirstOrDefault(p => p?.MRStatus.MRID == MRID);
        }

        /// <summary>
        ///     查找空闲机器人
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IVirtualRobot> FindIdleRobots()
        {
            lock (locker)
            {
                var idleRobots = new List<IVirtualRobot>();
                Console.WriteLine("-----------------FindIdleRobots-------------------");
                VirtualRobots.ForEach(robot =>
                {
                    Console.WriteLine(
                        $"-[{robot.MRStatus.MRID}] TaskCount:{robot.TaskCount} Idle:{robot.IsIdle()} Working:{robot.Working} State:{robot.State}");
                    if (robot.IsRobotReadyWork()) idleRobots.Add(robot);
                });
                Console.WriteLine("--------------------------------------------------");
                return idleRobots;
            }
        }

        /// <summary>
        ///     尝试刷新所有机器状态
        /// </summary>
        public void TryRefreshMRStatus()
        {
            foreach (var robot in VirtualRobots) TryRefreshMRStatus(robot.MRStatus.MRID);
        }

        /// <summary>
        ///     立即獲取MR狀態
        /// </summary>
        /// <param name="MRID"></param>
        /// <returns></returns>
        public MRStatus GetMRStatusSync(string MRID)
        {
            var response = AsyncHelper.RunSync(() => WS.DispatchAsync<Protocol.Query.MRStatus.Response>(
                new Protocol.Query.MRStatus
                {
                    MRID = MRID
                }));
            var mrStatus = response?.MRStatus;
            if (mrStatus == null) return null;
            var robot = FindRobot(mrStatus.MRID);
            if (robot != null) robot.MRStatus = mrStatus;
            return mrStatus;
        }

        /// <summary>
        /// 当MR完成了所有队列任务时触发
        /// </summary>
        public event MrIdleEventHandler OnMrIdle;

        /// <summary>
        ///     调用IM读取所有在线MR列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> ReadMrListFromIm()
        {
            lock (locker)
            {
                var response = AsyncHelper.RunSync(() => WS.DispatchAsync<MRList.Response>(new MRList()));
                return response?.MRIDs ?? Enumerable.Empty<string>();
            }
        }

        /// <summary>
        /// 添加机器人
        /// 注意:MRID必须唯一
        /// </summary>
        /// <param name="mrId"></param>
        /// <param name="createAction"></param>
        public void CreateVirtualRobot(string mrId, Action<IVirtualRobot> createAction)
        {
            lock (locker)
            {
                IVirtualRobot robot = null;
                if (FindRobot(mrId) == null)
                {
                    robot = ServiceProvider.GetService<IVirtualRobot>();
                    robot.MRStatus = new MRStatus
                    {
                        MRID = mrId
                    };
                    createAction?.Invoke(robot);
                }

                if (robot!=null)
                {
                    robot.OnMrRequestStatusRefresh += (sender, e) => { TryRefreshMRStatus(e.MRID); };
                    if (OnMrIdle != null) robot.OnMrIdle += OnMrIdle;
                    VirtualRobots.Add(robot);
                }
              
            }
        }


        /// <summary>
        ///    尝试机器状态数据(异步)
        /// </summary>
        public void TryRefreshMRStatus(string MRID)
        {
            _statusWatcher.Watch(MRID);
        }

        /// <summary>
        ///     获取当前的机器人列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IVirtualRobot> GetAllVirtualRobots()
        {
            return VirtualRobots;
        }

        private void StatusWatcher_MrStatusReceived1(object sender, MrStatusEventArg e)
        {
            // Console.WriteLine("[StatusWatcher_MrStatusReceived]");
            // Console.WriteLine(e.MrStatus.ToJson());
            var robot = FindRobot(e.MrStatus.MRID);
            robot?.SetMRStatus(e.MrStatus);
        }
    }
}