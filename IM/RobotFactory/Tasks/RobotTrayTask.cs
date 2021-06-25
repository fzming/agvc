using Messages.Transfers.Core;

namespace AgvcWorkFactory.Tasks
{
    /// <summary>
    /// 晶棒搬运任务
    /// </summary>
    [TaskType(RobotTaskType.Tray)]
    public class RobotTrayTask : AbstractRobotTask
    {
        public override void ExecuteFromToRules()
        {
            base.ExecuteFromToRules();
        }
        #region Overrides of AbstractRobotTask

        /// <summary>
        /// 實際任務单次執行步驟(From)
        /// </summary>
        protected override void OnRunFromTask(TaskGoal goal, int index)
        {
        }

        /// <summary>
        /// 實際任務单次執行步驟(To)
        /// </summary>
        protected override void OnRunToTask(TaskGoal goal, int index)
        {
        }

        protected override void OnTrxMessageAdded(IMessage message)
        {
            base.OnTrxMessageAdded(message);
        }

        #endregion
    }
}