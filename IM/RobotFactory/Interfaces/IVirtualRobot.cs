using AgvcWorkFactory.Tasks;
using RobotDefine;
using Utility;

namespace AgvcWorkFactory.Interfaces
{
    public interface IVirtualRobot:ITransientDependency
    {
        /// <summary>
        ///     机器人实时状态
        /// </summary>
        MRStatus MRStatus { get; set; }

        /// <summary>
        ///     正处于充电状态
        /// </summary>
        bool Docking { get; set; }

        /// <summary>
        ///     正处于任务执行状态
        /// </summary>
        bool Working { get; set; }

        /// <summary>
        ///     当前状态描述
        /// </summary>
        string State { get; set; }

        int TaskCount { get; }

        /// <summary>
        ///     请求更新实时状态
        /// </summary>
        event MrRequestStatusRefreshEventHandler OnMrRequestStatusRefresh;
        /// <summary>
        /// 当MR完成了所有队列任务时触发
        /// </summary>
        event MrIdleEventHandler OnMrIdle;
        /// <summary>
        ///     请求异步更新MR状态
        /// </summary>
        void RequestUpdateStatusAsync();

        /// <summary>
        ///     请求同步更新MR状态
        /// </summary>
        void RequestUpdateStatusSync();

        /// <summary>
        ///     设置MR状态
        /// </summary>
        /// <param name="working"></param>
        void SetWorkingStatus(bool working);

        /// <summary>
        ///     是否处于空闲状态
        /// </summary>
        /// <returns></returns>
        bool IsIdle();

        /// <summary>
        /// 是否处于初始化状态.MR重启后,一直会处于此状态.
        /// </summary>
        /// <returns></returns>
        bool IsInitialize();

        void AddTask(IRobotTask task);

        /// <summary>
        ///     檢查MR狀態是否可以執行任務
        /// </summary>
        /// <returns></returns>
        bool IsRobotReadyWork();
        void SetMRStatus(MRStatus eMrStatus);
    }

}