using System.Linq;
using CoreData;

namespace AgvcService.System.Upload
{
    public  static class UploadValidator
    {
        public static Result<bool> Validate(this HttpContentFile file,UploadOption option)
        {

            if (file.ContentLength <= 0)
            {
                return Result<bool>.Fail("没有上传的文件");
            }
            if (file.ContentLength > option.AllowMaxSize * 1024 * 1024)
            {
                return Result<bool>.Fail($"文件大小不允许超过：{option.AllowMaxSize}M");
            }
            var ext = file.FileName.Split('.').Last().ToLower();//文件扩展名
            if (!option.AllowExtensions.Contains(ext))
            {
                return Result<bool>.Fail("不支持的上传文件格式");
            }
            return Result<bool>.Successed;
        }

    }
}