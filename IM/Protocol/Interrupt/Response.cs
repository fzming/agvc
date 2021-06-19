using Protocol.Core;

namespace Protocol.Interrupt
{
    public class Response : Base
    {
        /// <summary>
        /// 是否成功中斷，True 表示機器人已經停止
        /// </summary>
        public bool Interrupted { get; set; }
        /// <summary>
        /// 日誌
        /// </summary>
        public string Log { get; set; }
    }
}

