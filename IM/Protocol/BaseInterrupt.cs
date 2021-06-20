using Protocol.Interrupt;

namespace Protocol
{
    public abstract class BaseInterrupt : Base
    {
        protected BaseInterrupt()
        {
        }

        public Response GetResponse(bool interrupted, string log) => 
            new Response { 
                Interrupted = interrupted,
                Log = log,
                SN = base.SN
            };
        /// <summary>
        /// 機器人編號
        /// </summary>
        public string MRID { get; set; }
    }
}

