using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Utility.Extensions
{
    /// <summary>
    ///     流，字节对象扩展
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public static class BitmapExtension
    {
        /// <summary>
        ///     byte[]转图片
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static Bitmap ToBitmap(this byte[] bytes)
        {
            MemoryStream stream = null;
            try
            {
                stream = new MemoryStream(bytes);
                return new Bitmap(stream);
            }

            finally
            {
                stream?.Close();
            }
        }

        /// <summary>
        ///     图片转byte[]
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static byte[] ToByteArray(this Bitmap image)
        {
            return (byte[]) TypeDescriptor.GetConverter(image).ConvertTo(image, typeof(byte[]));
        }

        /// <summary>
        ///     将 Stream 转成 byte[]
        /// </summary>
        public static byte[] ToBytes(this Stream stream)
        {
            var bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);

            // 设置当前流的位置为流的开始  
            stream.Seek(0, SeekOrigin.Begin);
            return bytes;
        }

        /// <summary>
        ///     将 byte[] 转成 Stream
        /// </summary>
        public static Stream ToStream(this byte[] bytes)
        {
            Stream stream = new MemoryStream(bytes);
            return stream;
        }


        /// <summary>
        ///     将 Stream 写入文件
        /// </summary>
        /// <param name="stream">流对象</param>
        /// <param name="filePath">相对路径或物理路径</param>
        public static string WriteToFile(this Stream stream, string filePath)
        {
            if (!Path.IsPathRooted(filePath)) filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePath);
            // 把 Stream 转换成 byte[]  
            var bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            // 设置当前流的位置为流的开始  
            stream.Seek(0, SeekOrigin.Begin);

            // 把 byte[] 写入文件  
            using var fs = new FileStream(filePath, FileMode.Create);
            using var bw = new BinaryWriter(fs);
            bw.Write(bytes);

            return filePath;
        }

        /// <summary>
        ///     将图片写入本地文件
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="imgPath"></param>
        /// <param name="maxWidth"></param>
        /// <param name="maxHeight"></param>
        /// <returns></returns>
        public static string WriteToImage(this Stream stream, string imgPath, int maxWidth = 0,
            int maxHeight = 0)
        {
            var originalImage = Image.FromStream(stream);
            var original = new
            {
                originalImage.Width,
                originalImage.Height
            };
            //原图宽高均小于模版，不作处理，直接保存
            if (original.Width <= maxWidth && original.Height <= maxHeight || maxWidth == 0 && maxHeight == 0)
                return stream.WriteToFile(imgPath);

            #region 自动计算MaxHeight,MaxWidth

            if (maxWidth == 0)
                maxWidth = originalImage.Width * maxHeight / originalImage.Height;
            else if (maxHeight == 0) maxHeight = originalImage.Height * maxWidth / originalImage.Width;

            if (maxWidth > original.Width)
            {
                maxWidth = original.Width;
                maxHeight = originalImage.Height * maxWidth / originalImage.Width;
            }

            if (maxHeight > original.Height)
            {
                maxHeight = original.Height;
                maxWidth = originalImage.Width * maxHeight / originalImage.Height;
            }

            #endregion

            //开始切图
            //新建一个bmp图片
            using (var bitmap = new Bitmap(maxWidth, maxHeight))
            {
                using var graphics = Graphics.FromImage(bitmap);
                //设置质量
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;

                //置背景色
                graphics.Clear(Color.White);
                //画图
                graphics.DrawImage(originalImage, new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                    new Rectangle(0, 0, original.Width, original.Height), GraphicsUnit.Pixel);

                if (!Path.IsPathRooted(imgPath)) imgPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, imgPath);

                //保存缩略图
                bitmap.Save(imgPath, GetImageFormat(imgPath));
            }
            //释放资源

            originalImage.Dispose();
            return imgPath;
        }

        /// <summary>
        ///     根据扩展名获取图片格式
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static ImageFormat GetImageFormat(string file)
        {
            var extension = Path.GetExtension(file)?.ToLower();
            switch (extension)
            {
                case ".bmp":
                    return ImageFormat.Bmp;
                case ".gif":
                    return ImageFormat.Gif;
                case ".png":
                    return ImageFormat.Png;
                case ".jpg":
                case ".jpeg":
                    return ImageFormat.Jpeg;
                case ".tiff":
                case ".tiff2":
                    return ImageFormat.Tiff;
            }

            return ImageFormat.Jpeg;
        }

        /// <summary>
        ///     从文件读取 Stream
        /// </summary>
        public static Stream ReadStream(this string fileName)
        {
            // 打开文件  
            var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            // 读取文件的 byte[]  
            var bytes = new byte[fileStream.Length];
            fileStream.Read(bytes, 0, bytes.Length);
            fileStream.Close();
            // 把 byte[] 转换成 Stream  
            Stream stream = new MemoryStream(bytes);
            return stream;
        }

        /// <summary>
        ///     保存到指定位置文件
        /// </summary>
        /// <param name="bitmap">源BITMAP对象</param>
        /// <param name="filepath">可选：相对路径或物理路径</param>
        /// <returns>保存的文件路径</returns>
        public static string ToFile(this Bitmap bitmap, string filepath = "")
        {
            if (bitmap == null) return string.Empty;
            if (!Path.IsPathRooted(filepath))
                filepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    filepath.IsNullOrEmpty() ? $"temp\\{Guid.NewGuid():D}.jpg" : filepath);


            //  Directory.CreateDirectory(filepath); //Mark 

            bitmap.Save(filepath, ImageFormat.Jpeg);
            bitmap.Dispose();
            return filepath;
        }

        public static byte[] ToBytes(this Bitmap bitmap)
        {
            using var ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Png);
            var buffer = new byte[ms.Length];
            ms.Position = 0;
            ms.Read(buffer, 0, (int) ms.Length);
            ms.Close();
            return buffer;
        }

        /// <summary>
        ///     保存为PNG字符串
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static string ToBase64PngString(this Bitmap bitmap)
        {
            //保存为PNG到内存流  
            return "data:image/png;base64," + Convert.ToBase64String(bitmap.ToBytes());
        }

        public static Image ParseImageFromBase64String(this string base64String)
        {
            var imageBytes = GetImageBytes(base64String);
            using var ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            ms.Write(imageBytes, 0, imageBytes.Length);
            return Image.FromStream(ms, true);
        }

        public static byte[] GetImageBytes(this string base64String)
        {
            var imageBytes = Convert.FromBase64String(base64String.Replace("data:image/png;base64,", ""));
            return imageBytes;
        }
    }
}