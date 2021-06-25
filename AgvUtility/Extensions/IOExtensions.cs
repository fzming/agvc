using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Extensions
{
    public static class IoExtensions
    {
        /// <summary>
        /// 读取指定文件的字节数据
        /// </summary>
        /// <param name="localFilePath">本地待读取的文件路径</param>
        /// <returns></returns>
        public static byte[] GetFileBytes(this string localFilePath)
        {
            if (!File.Exists(localFilePath))
            {
                throw new FileNotFoundException("文件未找到", localFilePath);
            }

            using var fs = new FileStream(localFilePath, FileMode.Open, FileAccess.Read);
            using var r = new BinaryReader(fs);
            var bytes = r.ReadBytes((int)fs.Length);
            r.Close();
            return bytes;
        }
        /// <summary>
        /// 读取文件内容
        /// </summary>
        /// <param name="localFilePath"></param>
        /// <returns></returns>
        public static string GetFileString(this string localFilePath)
        {

            var str = new StringBuilder();
            using var fs = File.OpenRead(localFilePath);
            var left = fs.Length;
            const int maxLength = 100; //每次读取的最大长度  
            var start = 0;//起始位置  
            while (left > 0)
            {
                var buffer = new byte[maxLength];//缓存读取结果  
                fs.Position = start;//读取开始的位置  
                var num = fs.Read(buffer, 0, left < maxLength ? Convert.ToInt32(left) : maxLength);//已读取长度  
                if (num == 0)
                {
                    break;
                }
                start += num;
                left -= num;
                str = str.Append(Encoding.UTF8.GetString(buffer));//byte - string
            }

            return str.ToString();

        }
        /// <summary>
        /// 异步读取文本内容
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static async Task<string> ReadAsync(this string filename)
        {
            char[] buffer;

            using (var sr = new StreamReader(filename))
            {
                buffer = new char[(int)sr.BaseStream.Length];
                await sr.ReadAsync(buffer, 0, (int)sr.BaseStream.Length);
            }

            return new string(buffer);
        }

        public static string GetFileMd5(this string file)
        {
            var oMd5Hasher =
                new MD5CryptoServiceProvider();

            var oFileStream = new FileStream(file, FileMode.Open,
                FileAccess.Read, FileShare.ReadWrite);
            var bytes = oMd5Hasher.ComputeHash(oFileStream);
            oFileStream.Close();
            //由以连字符分隔的十六进制对构成的String，其中每一对表示value 中对应的元素；例如“F-2C-4A”
            var strHashData = BitConverter.ToString(bytes);
            //替换-
            strHashData = strHashData.Replace("-", "");
           
            return strHashData.ToLower();

        }
        /// <summary>
        ///  判断文件流是否是图片流
        /// </summary>
        /// <param name="imageStream"></param>
        /// <returns></returns>
        // ReSharper disable once UnusedMember.Global
        public static bool IsImage(this Stream imageStream)
        {
            if (imageStream.Length > 0)
            {
                var header = new byte[4]; // Change size if needed.
                var imageHeaders = new[]{
                    "\xFF\xD8", // JPEG
                    "BM",       // BMP
                    "GIF",      // GIF
                    Encoding.ASCII.GetString(new byte[]{137, 80, 78, 71})}; // PNG

                imageStream.Read(header, 0, header.Length);

                var isImageHeader = imageHeaders.Count(str => Encoding.ASCII.GetString(header).StartsWith(str)) > 0;
                if (isImageHeader)
                {
                    try
                    {
                        Image.FromStream(imageStream).Dispose();
                        imageStream.Close();
                        return true;
                    }

                    catch
                    {
                        // ignored
                    }
                }
            }

            imageStream.Close();
            return false;
        }
    }
}