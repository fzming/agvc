namespace Protocol.Query
{
    public class Echo : Base
    {
        public Response GetResponse()
        {
            return new() {SN = SN};
        }

        public class Response : Base
        {
        }
    }
}