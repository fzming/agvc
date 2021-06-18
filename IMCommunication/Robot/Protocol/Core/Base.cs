using Serialize;

namespace IMCommunication.Robot.Protocol.Core
{
    public abstract class Base : JSON.AutoJsonable
    {
        protected Base()
        {
        }
        /// <summary>
        /// 通訊流水號
        /// </summary>
        public string SN { get; set; }
    }
}

