namespace Protocol
{
    public abstract class BaseMission : Base
    {
        /// <summary>
        ///     任務編號
        /// </summary>
        public string MissionID { get; set; }

        /// <summary>
        ///     机器人编号
        /// </summary>
        public string MRID { get; set; }

        public Response GetResponse(bool accept, string log)
        {
            return new()
            {
                Accept = accept,
                Log = log,
                SN = SN
            };
        }

        public class Response : Base
        {
            public bool Accept { get; set; }

            public string Log { get; set; }
        }
    }
}