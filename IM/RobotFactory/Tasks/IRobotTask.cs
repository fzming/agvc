using Messages.Transfers.Core;
using RobotFactory.Interfaces;

namespace RobotFactory.Tasks
{
    public interface IRobotTask : ITask
    {
        /// <summary>
        /// 设置MES->AGVC TX501i消息
        /// </summary>
        /// <param name="message"></param>
        void AddTrxMessage(IMessage message);
        void SetAgvReporter(IAgvReporter agvReporter);
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