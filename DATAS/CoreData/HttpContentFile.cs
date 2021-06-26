using System;
using System.IO;
using System.Linq;
using Utility.Extensions;
using Utility.Helpers;

namespace CoreData
{
    public class HttpContentFile
    {
        /// <summary>
        ///     初始化 <see cref="T:System.Object" /> 类的新实例。
        /// </summary>
        public HttpContentFile(string fileName, Stream fileStream, string mediaType = "image/jpeg")
        {
            FileName = fileName;
            FileStream = fileStream;
            ContentLength = fileStream.Length;
            MediaType = mediaType;
        }

        /// <summary>
        ///     初始化 <see cref="T:System.Object" /> 类的新实例。
        /// </summary>
        public HttpContentFile()
        {
        }

        public string FileName { get; set; }
        public long? ContentLength { get; set; }
        public string MediaType { get; set; }
        public Stream FileStream { get; set; }

        /// <summary>
        ///     自动文件名称
        /// </summary>
        public string AutoFileName => $"{Guid.NewGuid():D}.{FileName.Split('.').Last().ToLower()}";

        /// <summary>
        ///     保存为文件
        /// </summary>
        public FileSaveResult Save(UploadOption option = null, string folder = "", string fileName = "")
        {
            if (folder.IsNullOrEmpty()) folder = $"files\\{DateTime.Now:yyMMdd}";

            var shouldCutImage = option != null && (option.AllowMaxWidth > 0 || option.AllowMaxHeight > 0) &&
                                 MediaType.StartsWith("image/");

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folder);


            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            var name = fileName.IsNullOrEmpty() ? AutoFileName : fileName;
            var ret = new FileSaveResult
            {
                PhysicalPath = Path.Combine(path, name),
                RelativePath = $"/{folder.Replace(@"\", "/")}/{name}"
            };
            //保存到服务器本地文件系统
            if (shouldCutImage)
                FileStream.WriteToImage(ret.PhysicalPath, option.AllowMaxWidth, option.AllowMaxHeight);
            else
                FileStream.WriteToFile(ret.PhysicalPath);

            return ret;
        }

        public static HttpContentFile ReadFromFile(string filePath)
        {
            var file = PathHelper.MapPath(filePath);
            var stream = file.ReadStream();
            return new HttpContentFile
            {
                FileStream = stream,
                ContentLength = stream.Length,
                FileName = Path.GetFileName(file)
            };
        }
    }

    /// <summary>
    ///     文件保存结果
    /// </summary>
    public class FileSaveResult
    {
        /// <summary>
        ///     物理路径
        /// </summary>
        public string PhysicalPath { get; set; }

        /// <summary>
        ///     相对路径
        /// </summary>
        public string RelativePath { get; set; }
    }
}