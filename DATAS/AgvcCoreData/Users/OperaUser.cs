using System;
using CoreData;

namespace AgvcCoreData.Users
{
    /// <summary>
    ///     操作人员
    /// </summary>
    public class OperaUser : Identity
    {
        /// <summary>
        ///     操作日期
        /// </summary>
        //  [TrackField("操作日期")]
        public DateTime Time { get; set; } = DateTime.Now;

        /// <summary>
        ///     操作备注
        /// </summary>
        public string Extra { get; set; }
    }
}