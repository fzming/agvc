using AgvcWorkFactory.Interfaces;
using Messages.Transfers.Core;

namespace AgvcWorkFactory.Tasks
{
    public interface IRobotTask : ITask
    {
        /// <summary>
        /// 路径类型
        /// </summary>
        TaskPathType PathType { get; set; }
        /// <summary>
        /// 任务形式(只读)
        /// </summary>
        RobotTaskType TaskType { get; }
        /// <summary>
        ///     设置MES->AGVC TX501i消息
        /// </summary>
        /// <param name="message"></param>
        void AddTrxMessage(IMessage message);

        /// <summary>
        ///     执行任务
        /// </summary>
        /// <param name="virtualRobot"></param>
        void Run(VirtualRobot virtualRobot);
    }
}