using System.IO;

namespace Utility.Helpers
{
    /// <summary>
    ///     Byte和File互转帮助类
    /// </summary>
    public class ByteHelper
    {
        /// <summary>
        ///     读文件到byte[]
        /// </summary>
        /// <param name="fileName">硬盘文件路径</param>
        /// <returns></returns>
        public static byte[] ReadFileToByte(string fileName)
        {
            FileStream pFileStream = null;
            var pReadByte = new byte[0];
            try
            {
                pFileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                var r = new BinaryReader(pFileStream);
                r.BaseStream.Seek(0, SeekOrigin.Begin); //将文件指针设置到文件开
                pReadByte = r.ReadBytes((int) r.BaseStream.Length);
                return pReadByte;
            }
            catch
            {
                return pReadByte;
            }
            finally
            {
                pFileStream?.Close();
            }
        }

        /// <summary>
        ///     写byte[]到fileName
        /// </summary>
        /// <param name="pReadByte">byte[]</param>
        /// <param name="fileName">保存至硬盘路径</param>
        /// <returns></returns>
        public static bool WriteByteToFile(byte[] pReadByte, string fileName)
        {
            FileStream pFileStream = null;
            try
            {
                pFileStream = new FileStream(fileName, FileMode.OpenOrCreate);
                pFileStream.Write(pReadByte, 0, pReadByte.Length);
            }
            catch
            {
                return false;
            }
            finally
            {
                pFileStream?.Close();
            }

            return true;
        }

        public static byte[] StreamToBytes(Stream stream)

        {
            var bytes = new byte[stream.Length];

            stream.Read(bytes, 0, bytes.Length);

            // 设置当前流的位置为流的开始 

            stream.Seek(0, SeekOrigin.Begin);

            return bytes;
        }

        /// 将 byte[] 转成 Stream
        public static Stream BytesToStream(byte[] bytes)

        {
            Stream stream = new MemoryStream(bytes);

            return stream;
        }
    }
}