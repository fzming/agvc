using System;
using System.Threading;
using Protocol;

namespace AgvcWorkFactory
{
    public class AgvReport
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public AgvReport(string mrId, string missionId, Type type, AutoResetEvent waitHandle)
        {
            MrId = mrId;
            MissionId = missionId;
            Type = type;
            WaitHandle = waitHandle;
            CreateTime = DateTime.Now;
        }

        public string GetKey()
        {
            return $"{MrId}-{MissionId}-{Type.FullName}";
        }
        public string MrId { get; set; }
        public string MissionId { get; set; }
        public Type Type { get; set; }
        public AutoResetEvent WaitHandle { get; set; }
        public DateTime CreateTime { get; set; }
        //汇报后========
        public Func<BaseReport, bool> AgreeCall { get; set; }
        public BaseReport Report { get; set; }
        public bool Received { get; set; }
        public double Ms { get; set; }

    }
}