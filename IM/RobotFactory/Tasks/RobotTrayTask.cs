namespace AgvcWorkFactory.Tasks
{
    /// <summary>
    /// 晶棒搬运任务
    /// </summary>
    [TaskType(RobotTaskType.Tray)]
    public class RobotTrayTask : AbstractRobotTask
    {
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

        #endregion
    }
}