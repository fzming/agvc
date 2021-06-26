namespace Messages.Transfers.Core
{
    /// <summary>
    ///     AGVC->MES
    /// </summary>
    public abstract class OutputMessageBase : MessageBase
    {
        /// <summary>
        ///     Return code 1
        /// </summary>
        /// <returns>
        ///     Normal:000000
        /// </returns>
        [Deserialization(6, 3)]
        public string retcode1 { get; set; }

        /// <summary>
        ///     sqlerrcode
        ///     Normal:000000
        /// </summary>
        [Deserialization(6, 4)]
        public string sqlerrcode { get; set; }
    }
}