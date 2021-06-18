namespace MesCommunication.Protocol
{
    /// <summary>
    /// MES->AGVC
    /// </summary>
    public abstract class InputCommand:Command
    {
        [Deserialization(1,3)]
        public string reqqmode { get; set; }
    }
}