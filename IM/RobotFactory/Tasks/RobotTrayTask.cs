using AgvcWorkFactory.Interfaces;
using Messages.Transfers.Core;

namespace AgvcWorkFactory.Tasks
{
    /// <summary>
    ///     晶棒搬运任务
    /// </summary>
    public class RobotTrayTask : AbstractRobotTask
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public RobotTrayTask(IAgvReporter agvReporter, IWS ws) : base(agvReporter, ws)
        {
        }

        public override void ExecuteFromToRules()
        {
            base.ExecuteFromToRules();
        }

        #region Overrides of AbstractRobotTask

        /// <summary>
        ///     實際任務单次執行步驟(From)
        /// </summary>
        protected override void OnRunFromTask(TaskGoal goal, int index)
        {
        }

        /// <summary>
        ///     實際任務单次執行步驟(To)
        /// </summary>
        protected override void OnRunToTask(TaskGoal goal, int index)
        {
        }

        /// <summary>
        ///     任务类型
        /// </summary>
        public override RobotTaskType TaskType => RobotTaskType.Tray;

        protected override void OnTrxMessageAdded(IMessage message)
        {
            base.OnTrxMessageAdded(message);
        }

        #endregion
    }
}