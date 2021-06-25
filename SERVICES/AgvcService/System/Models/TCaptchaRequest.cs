using System.Diagnostics.CodeAnalysis;

namespace AgvcService.System.Models
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    public class CaptchaRequest
    {
        public object bizState { get; set; }
        public string appid { get; set; }
        public int ret { get; set; }
        public string ticket { get; set; }
        public string randstr { get; set; }
    }

}