namespace Messages.Transfers.Core
{
    public abstract class MessageBase : IMessage
    {
        /// <summary>
        /// Transaction ID
        /// </summary>
        [Deserialization(5,1)]
        public string trx_id { get; set; }

        /// <summary>
        /// Transmission type
        /// I:Input  O:Output
        /// </summary>
        [Deserialization(1,2)]
        public string type_id { get; set; }

    }
}
