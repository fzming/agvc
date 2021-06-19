using System.Collections.Generic;
using System.Linq;
using Protocol.Core;

namespace Protocol.Query
{
    public class MRList : Base
    {
        public Response GetResponse(IEnumerable<string> mrids)
        {
            if (mrids != null)
            {
                return new Response { 
                    SN = base.SN,
                    MRIDs = mrids.ToList<string>()
                };
            }
            return new Response { SN = base.SN };
        }

        public class Response : Base
        {
            public List<string> MRIDs { get; set; }
        }
    }
}

