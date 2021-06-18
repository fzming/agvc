namespace IMCommunication.Robot.Protocol
{
    public abstract class Base : JSON.AutoJsonable
    {
        protected Base()
        {
        }

        public string SN { get; set; }
    }
}

