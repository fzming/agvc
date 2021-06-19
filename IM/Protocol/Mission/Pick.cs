using Protocol.Core;

namespace Protocol.Mission
{
    /// <summary>
    /// 當機器人身上有一個以上空儲位時，命令機器人前往取貨					
    /// </summary>
    public class Pick : BaseMission
    {
        public string BoxID { get; set; }

        public string Goal { get; set; }

        public int Port { get; set; }

        public int WaferCount { get; set; }
    }
}

