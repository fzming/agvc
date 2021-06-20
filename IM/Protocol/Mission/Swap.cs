namespace Protocol.Mission
{
    public class Swap : BaseMission
    {
        public string DropBoxID { get; set; }

        public int DropWaferCount { get; set; }

        public string Goal { get; set; }

        public string PickBoxID { get; set; }

        public int PickWaferCount { get; set; }

        public int Port { get; set; }
    }
}

