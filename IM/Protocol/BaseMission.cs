﻿using Protocol.Mission;

namespace Protocol
{
    public abstract class BaseMission : Base
    {
        protected BaseMission()
        {
        }

        public Response GetResponse(bool accept, string log) => 
            new Response { 
                Accept = accept,
                Log = log,
                SN = base.SN
            };
        /// <summary>
        /// 任務編號
        /// </summary>
        public string MissionID { get; set; }
        /// <summary>
        /// 机器人编号
        /// </summary>
        public string MRID { get; set; }
        public class Response : Base
        {
            public bool Accept { get; set; }

            public string Log { get; set; }
        }
    }
}
