using Messages.Transfers.Core;

namespace RobotFactory.Tasks
{
    public interface IRobotTask : ITask
    {
        /// <summary>
        /// 设置MES->AGVC TX501i消息
        /// </summary>
        /// <param name="message"></param>
        void AddTrxMessage(IMessage message);
        /// <summary>
        /// 执行任务
        /// </summary>
        /// <param name="virtualRobot"></param>
        void Run(VirtualRobot virtualRobot);
        /// <summary>
        /// 路径类型
        /// </summary>
        TaskPathType PathType { get; set; }
        /// <summary>
        /// 任务类型
        /// </summary>
        RobotTaskType TaskType { get; set; }
    }
}