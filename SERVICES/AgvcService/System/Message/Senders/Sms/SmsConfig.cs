using System.Diagnostics.CodeAnalysis;

namespace AgvcService.System.Message.Senders.Sms
{
    /// <summary>
    ///     短信配置类
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    internal class SmsConfig
    {
        public string uid { get; set; }
        public string pwd { get; set; }
        public string sign { get; set; }
        public string api { get; set; }
        public string mappingDebugMobile { get; set; }
    }
}