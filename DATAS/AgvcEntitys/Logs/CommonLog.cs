using CoreData;
using CoreData.Core;

namespace AgvcEntitys.Logs
{
    /// <summary>
    ///     机构通用日志
    /// </summary>
    public class CommonLog : OEntity
    {
        /// <summary>
        ///     自定义KEY
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        ///     自定义内容
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        ///     日志添加人员
        /// </summary>
        public Identity User { get; set; }
    }
}