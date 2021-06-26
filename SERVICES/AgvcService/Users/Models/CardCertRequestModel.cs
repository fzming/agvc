using AgvcCoreData.Users;

namespace AgvcService.Users.Models
{
    public class CardCertRequestModel : IdCert
    {

        /// <summary>
        /// 请求安全码 防止站外恶意调用
        /// </summary>
        public string SecurityCode { get; set; }
    }
}