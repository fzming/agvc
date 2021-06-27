using System;
using System.Threading;
using Protocol;

namespace AgvcWorkFactory
{
    public class AgvRequest
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public AgvRequest(string mrId, string missionId, Type type, AutoResetEvent waitHandle)
        {
            MrId = mrId;
            MissionId = missionId;
            Type = type;
            WaitHandle = waitHandle;
            CreateTime = DateTime.Now;
        }

        public string MrId { get; set; }
        public string MissionId { get; set; }
        public Type Type { get; set; }
        public AutoResetEvent WaitHandle { get; set; }

        public DateTime CreateTime { get; set; }

        //汇报后========
        public Func<BaseRequest, bool> AgreeCall { get; set; }
        public BaseRequest Request { get; set; }
        public bool Received { get; set; }
        public double Ms { get; set; }

        public string GetKey()
        {
            return $"{MrId}-{MissionId}-{Type.FullName}";
        }
    }
}