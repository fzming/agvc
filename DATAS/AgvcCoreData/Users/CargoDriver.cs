using CoreData;
using MongoDB.Bson.Serialization.Attributes;

namespace AgvcCoreData.Users
{
    /// <summary>
    /// 货运司机
    /// </summary>
    public class CargoDriver:Identity
    {
  
        /// <summary>
        /// 身份证号
        /// </summary>
        [BsonIgnoreIfNull]
        public string IdCard { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        [BsonIgnoreIfNull]
        public string Mobile { get; set; }
        /// <summary>
        /// 车牌号
        /// </summary>
        [BsonIgnoreIfNull]
        public string Plate { get; set; }
        /// <summary>
        /// 司机线路分享链接
        /// </summary>
        public string RouteShareUrl { get; set; }

    }
}