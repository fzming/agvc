namespace IMCommunication.Robot.Protocol.Interrupt
{
    public class Response : Base
    {
        public bool Interrupted { get; set; }

        public string Log { get; set; }
    }
}

