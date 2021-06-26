using System.Collections.Generic;
using System.Threading.Tasks;
using Utility;

namespace AgvcService.System.Upload
{
    public interface IUpYunUploader : ISingletonDependency
    {
        /// <summary>
        /// 上传文件至又拍云并返回URL地址
        /// </summary>
        /// <param name="filePath">本地文件地址</param>
        /// <param name="deleteWhenUploaded">上传完毕后是否删除本地源文件</param>
        /// <returns>URL地址</returns>
        Task<string> UploadFileAsync(string filePath, bool deleteWhenUploaded = true);
        /// <summary>
        /// 上传字节到又拍云并返回URL地址
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        Task<string> UpdateBytesAsync(string fileName, byte[] bytes);

        /// <summary>
        /// 获取本地服务器文件的URL地址
        /// </summary>
        /// <param name="relativePath">相对路径</param>
        /// <returns></returns>
        string GetLocalServerFileUrl(string relativePath);
        /// <summary>
        /// 获取某个子目录的占用信息
        /// </summary>
        /// <param name="url">子目录的网络路径，必须以"/"开头，例如：/20200921</param>
        /// <returns>空间占用量，失败返回 0</returns>
        Task<long> GetFolderUsageAsync(string url);

        /// <summary>
        /// 获取总体空间的占用信息
        /// </summary>
        /// <returns>空间占用量，失败返回 0</returns>
        Task<long> GetBucketUsageAsync();

        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="path">子目录的网络路径，必须以"/"开头，例如：/20200921</param>
        /// <returns></returns>
        Task<bool> MkDirAsync(string path);

        /// <summary>
        /// 删除目录
        /// </summary>
        /// <!--DELETE /<bucket>/path/to/folder-->
        /// <param name="path">目录路径 必须以"/"开头，例如：/20200921</param>
        /// <returns></returns>
        Task<bool> RmDirAsync(string path);

        /// <summary>
        /// 读取目录列表
        /// </summary>
        /// <param name="path">目录路径 必须以"/"开头，例如：/20200921</param>
        /// <returns></returns>
        Task<List<FolderItem>> ReadDirAsync(string path);

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="path">文件路径（包含文件名）</param>
        /// <returns></returns>
        Task<bool> DeleteFileAsync(string path);

        Task<byte[]> ReadFileAsync(string path);
    }
}