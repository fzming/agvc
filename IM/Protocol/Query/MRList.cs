using System.Collections.Generic;
using System.Linq;

namespace Protocol.Query
{
    public class MRList : Base
    {
        public Response GetResponse(IEnumerable<string> mrids)
        {
            if (mrids != null)
                return new Response
                {
                    SN = SN,
                    MRIDs = mrids.ToList()
                };
            return new Response {SN = SN};
        }

        public class Response : Base
        {
            public List<string> MRIDs { get; set; }
        }
    }
}