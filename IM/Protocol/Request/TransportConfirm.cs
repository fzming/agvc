namespace Protocol.Request
{
    public class TransportConfirm:BaseRequest
    {
        public string Goal { get; set; }

        public string MissionID { get; set; }

        public int Port { get; set; }
    }
}