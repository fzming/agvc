namespace RobotDefine
{
    /// <summary>
    /// 任务状态
    /// </summary>
    public enum MissionStatus
    {
        /// <summary>
        /// MR 系統無法辨識的狀態
        /// </summary>
        Unknow,
        /// <summary>
        /// MR 系統初始化
        /// </summary>
        Initialize,
        /// <summary>
        /// MR 閒置中，可接收任務指令
        /// </summary>
        Standby,
        /// <summary>
        ///  MR 執行任務中
        /// </summary>
        OnMission,
        /// <summary>
        /// 維修模式
        /// </summary>
        MaintainMode,
        /// <summary>
        /// 進行充電模式
        /// </summary>
        Dock,
        /// <summary>
        /// Alarm 或狀態異常
        /// </summary>
        ErrorEvent,
        /// <summary>
        /// MR 與派車系統通訊中
        /// </summary>
        Handshaking
    }
}

