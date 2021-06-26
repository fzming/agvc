using CoreData;

namespace AgvcService.System.Upload
{
    /// <summary>
    /// 单文件上传模型
    /// </summary>
    public class SingleUploadModel : UploadModel
    {
      
        /// <summary>
        /// 上传的文件
        /// </summary>
        public HttpContentFile File { get; set; }
    }
}