using CoreData;

namespace AgvcService.System.Upload
{
    /// <summary>
    /// 多文件上传模型
    /// </summary>
    public class MultipleUploadModel : UploadModel
    {   
       
        /// <summary>
        /// 上传的文件列表
        /// </summary>
        public HttpContentFile[] Files { get; set; }
    }
}