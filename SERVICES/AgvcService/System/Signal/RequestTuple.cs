using System.Diagnostics.CodeAnalysis;

namespace AgvcService.System.Signal
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class RequestTuple
    {
        public string client_id { get; set; }
        /// <summary>
        /// 来源
        /// </summary>
        public string source { get; set; }
        public bool kickoff { get; set; }

        #region 设备信息

        public string agent { get; set; }
        public string ip { get; set; }

        #endregion
    }
}