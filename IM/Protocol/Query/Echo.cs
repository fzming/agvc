namespace Protocol.Query
{
    public class Echo : Base
    {
        public Response GetResponse() => 
            new Response { SN = base.SN };

        public class Response : Base
        {
        }
    }
}

