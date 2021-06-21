﻿using Protocol.Request;

namespace Protocol
{
    public abstract class BaseRequest : Base
    {
        protected BaseRequest()
        {
        }

        public Response GetResponse(bool agree, string log) => 
            new Response { 
                Agree = agree,
                Log = log,
                SN = base.SN
            };
        /// <summary>
        /// 机器人编号
        /// </summary>
        public string MRID { get; set; }
    }
}
