namespace AgvcService.System.Message.Address
{
    public class AppReceiver:IMessageReceiver
    {
        public string PushAddress { get; set; }
        public bool Validate()
        {
            return true;
        }
    }
}