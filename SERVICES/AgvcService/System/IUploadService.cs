using System.Threading.Tasks;
using AgvcCoreData.System;
using AgvcService.System.Upload;
using CoreData;
using CoreService.Interfaces;
using UploadOption = CoreData.UploadOption;

namespace AgvcService.System
{
    /// <summary>
    ///     文件上传服务
    /// </summary>
    public interface IUploadService : IService
    {
        /// <summary>
        ///     上传Bytes
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="fileName"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        Task<AttachmentUrl> UploadBytesAsync(byte[] bytes, string fileName, UploadOption option);

        /// <summary>
        ///     上传文件
        /// </summary>
        /// <param name="file"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        Task<AttachmentUrl> UploadFileAsync(HttpContentFile file, UploadOption option);

        /// <summary>
        ///     多文件上传
        /// </summary>
        /// <param name="files"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        Task<AttachmentUrl[]> UploadFilesAsync(HttpContentFile[] files, UploadOption option);

        /// <summary>
        ///     上传文件
        /// </summary>
        /// <param name="singleUploadModel"></param>
        /// <returns></returns>
        Task<AttachmentUrl> UploadFileAsync(SingleUploadModel singleUploadModel);

        /// <summary>
        ///     上传多个文件
        /// </summary>
        /// <param name="multipleUploadModel"></param>
        /// <returns></returns>
        Task<AttachmentUrl[]> UploadManyFilesAsync(MultipleUploadModel multipleUploadModel);
    }
}