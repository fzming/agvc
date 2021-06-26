using Protocol.Interrupt;

namespace Protocol
{
    public abstract class BaseInterrupt : Base
    {
        /// <summary>
        ///     機器人編號
        /// </summary>
        public string MRID { get; set; }

        public Response GetResponse(bool interrupted, string log)
        {
            return new()
            {
                Interrupted = interrupted,
                Log = log,
                SN = SN
            };
        }
    }
}