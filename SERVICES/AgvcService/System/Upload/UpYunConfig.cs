namespace AgvcService.System.Upload
{
    /// <summary>
    ///     又拍云空间配置
    /// </summary>
    public class UpYunConfig
    {
        // ReSharper disable once InconsistentNaming
        public string DL = "/";
        public string BucketName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool UpAuth { get; set; } = false;
        public string ApiDomain { get; set; } //v0.api.upyun.com
        public string UserDomain { get; set; } //image.yy5156.com
    }

    /// <summary>
    ///     服务器配置
    /// </summary>
    public class ServerConfig
    {
        public string Domain { get; set; }
        public string MobileRouteSharePageUrl { get; set; }
        public string MobilePoiSharePageUrl { get; set; }
        public string MobileDomain { get; set; }
    }
}