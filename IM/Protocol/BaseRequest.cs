namespace Protocol
{
    public abstract class BaseRequest : Base
    {
        public string MissionID { get; set; }
        /// <summary>
        ///     机器人编号
        /// </summary>
        public string MRID { get; set; }
        public class Response : Base
        {
            /// <summary>
            ///     AGVC 是否同意接受任務，True 表示同意
            /// </summary>
            public bool Agree { get; set; }

            /// <summary>
            ///     日志
            /// </summary>
            public string Log { get; set; }
        }
        public Response GetResponse(bool agree, string log)
        {
            return new()
            {
                Agree = agree,
                Log = log,
                SN = SN
            };
        }
    }
}