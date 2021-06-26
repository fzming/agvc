namespace Messages.Transfers.Core
{
    /// <summary>
    ///     MES->AGVC
    /// </summary>
    public abstract class InputMessageBase : MessageBase
    {
        [Deserialization(1, 3)] public string reqqmode { get; set; }
    }
}