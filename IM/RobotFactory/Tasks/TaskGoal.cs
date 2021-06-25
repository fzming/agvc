using Messages.Transfers.Core;

namespace AgvcWorkFactory.Tasks
{
    public class TaskGoal
    {
        /// <summary>
        /// 貨箱號碼
        /// </summary>
        public string BoxID { get; set; }
        /// <summary>
        /// 目標點
        /// </summary>
        public string Goal { get; set; } //设备 EQP OR STK OR 1F 3F
        /// <summary>
        /// 目標儲位
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// 晶圓片數
        /// </summary>
        public int WaferCount { get; set; }
        /// <summary>
        /// 是否来至MES 的Transfer Request消息，如果是手工派发的任务，此字段将为NULL
        /// </summary>
        public IMessage TrxMessage { get; set; }
    }
}