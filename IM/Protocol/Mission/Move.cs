using System.Collections.Generic;

namespace Protocol.Mission
{
    /// <summary>
    /// 命令機器人移動
    /// </summary>
    public class Move : BaseMission
    {
        /// <summary>
        /// 路徑集合
        /// </summary>
        public List<string> Goals { get; set; }
    }
}

