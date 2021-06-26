using System.Collections.Generic;

namespace RobotDefine
{
    /// <summary>
    ///     MR状态
    /// </summary>
    public class MRStatus : IMRStatus
    {
        /// <summary>
        ///     當前抵達點名稱
        /// </summary>
        public string ArrivedGoal { get; set; }

        /// <summary>
        ///     電池電量百分比
        /// </summary>
        public double Battery { get; set; }

        /// <summary>
        ///     任務編號
        /// </summary>
        public string CurrentMissionID { get; set; }

        /// <summary>
        ///     錯誤訊息
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        ///     方向角(deg)
        /// </summary>
        public double Head { get; set; }

        /// <summary>
        ///     IO 狀態
        /// </summary>
        public IOperatorStatus IOperatorStatus { get; set; }

        /// <summary>
        ///     任務狀態
        /// </summary>
        public MissionStatus MissionStatus { get; set; }

        /// <summary>
        ///     機器人編號
        /// </summary>
        public string MRID { get; set; }

        /// <summary>
        ///     車身儲位
        /// </summary>
        public List<string> Starage { get; set; }

        /// <summary>
        ///     X 座標(mm)
        /// </summary>
        public double X { get; set; }

        /// <summary>
        ///     Y 座標(mm)
        /// </summary>
        public double Y { get; set; }
    }
}