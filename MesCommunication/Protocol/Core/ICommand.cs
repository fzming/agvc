namespace MesCommunication.Protocol
{
    public interface ICommand
    {
        /// <summary>
        /// Transaction ID
        /// </summary>
        [Deserialization(5,0)]
        string trx_id { get; set; }

        /// <summary>
        /// Transmission type
        /// I:Input  O:Output
        /// </summary>
        [Deserialization(1,1)]
        string type_id { get; set; }
    }
}