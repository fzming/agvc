namespace Protocol.Report
{
    [Timeout(20149)]
    public class TransportEnd : BaseReport
    {
        public string Goal { get; set; }

        public int Port { get; set; }
    }
}