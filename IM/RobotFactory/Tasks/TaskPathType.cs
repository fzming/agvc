namespace AgvcWorkFactory.Tasks
{
    /// <summary>
    /// 步骤实际Path类型，以及实际执行任务实例类型
    /// </summary>
    public enum TaskPathType
    {
        /// <summary>
        /// Stock到EQP设备
        /// </summary>
        [TaskType(RobotTaskType.Transfer)]
        StockToEQP,
        /// <summary>
        /// Stock到Stock
        /// </summary>
        [TaskType(RobotTaskType.Transfer)]
        StockToStock,
        /// <summary>
        /// EQP设备到Stock
        /// </summary>
        [TaskType(RobotTaskType.Transfer)]
        EQPToStock,
        /// <summary>
        /// EQP设备到EQP
        /// </summary>
        [TaskType(RobotTaskType.Transfer)]
        EQPToEQP,
        /// <summary>
        /// 重Tray From 1F to 3F
        /// 晶棒从1楼搬到3楼，单晶就是指只有1个货物
        /// </summary>
        [TaskType(RobotTaskType.Tray)]
        HeaveTray,
        /// <summary>
        /// 空Tray From 3F To 1F
        /// 晶棒从3楼搬到1楼，单晶就是指只有1个货物
        /// </summary>
        [TaskType(RobotTaskType.Tray)]
        EmptyTray

    }
}