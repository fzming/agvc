using Protocol.Mission;

namespace RobotFactory
{
    /// <summary>
    /// 取货任务
    /// 當機器人身上有一個以上空儲位時，命令機器人前往取貨				
    /// </summary>
    public class PickTask
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

        public void Run(Pick pick)
        {

        }
    }
}