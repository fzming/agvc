namespace Protocol.Mission
{
    /// <summary>
    /// 當機器人身上有 B001 時，命令機器人前往放貨				
    /// </summary>
    public class Drop : BaseMission
    {
        /// <summary>
        /// 貨箱號碼
        /// </summary>
        public string BoxID { get; set; }
        /// <summary>
        /// 目標點
        /// </summary>
        public string Goal { get; set; }
        /// <summary>
        /// 目標儲位
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// 晶圓片數
        /// </summary>
        public int WaferCount { get; set; }
    }
}

