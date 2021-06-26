namespace RobotDefine
{
    /// <summary>
    ///     IO 狀態
    /// </summary>
    public enum IOperatorStatus
    {
        /// <summary>
        ///     MR 系統無法辨識的狀態
        /// </summary>
        UnKnow,

        /// <summary>
        ///     MR 系統初始化
        /// </summary>
        Initialize,

        /// <summary>
        ///     MR 闲置中
        /// </summary>
        Idle,

        /// <summary>
        ///     MR 於充電站上(MissionStatus 可能為 Dock 或 Standby)
        /// </summary>
        Docked,

        /// <summary>
        ///     MR 進行任務中的通訊動作
        /// </summary>
        Busy,

        /// <summary>
        ///     MR 移動中
        /// </summary>
        Moving,

        /// <summary>
        ///     MR Robot Arm 正在動作中
        /// </summary>
        Handling,

        /// <summary>
        ///     MR 暫停
        /// </summary>
        Pause,

        /// <summary>
        ///     MR 定位異常
        /// </summary>
        Lost,

        /// <summary>
        ///     移動失敗
        /// </summary>
        MovingFail,

        /// <summary>
        ///     MR 出現 Job Timeout
        /// </summary>
        Timeout,

        /// <summary>
        ///     MR 被觸發 EMO
        /// </summary>
        EMO,

        /// <summary>
        ///     MR bumper 感測觸發
        /// </summary>
        Bumper,

        /// <summary>
        ///     MR 任務或其他異常警報
        /// </summary>
        Alarm
    }
}