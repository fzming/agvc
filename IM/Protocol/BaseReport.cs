﻿using Protocol.Report;

namespace Protocol
{
    public abstract class BaseReport : Base
    {
        protected BaseReport()
        {
        }

        public Response GetResponse(bool received) => 
            new Response { 
                Received = received,
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
    }
}
