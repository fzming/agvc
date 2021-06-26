using Serialize;

namespace Protocol
{
    public abstract class Base : JSON.AutoJsonable
    {
        /// <summary>
        ///     通訊流水號
        /// </summary>
        public string SN { get; set; } = "0000";
    }
}