using Protocol.Core;

namespace Protocol.Request
{
    public class UnbindConfirm : BaseRequest
    {
        public string MissionID { get; set; }
    }
}

