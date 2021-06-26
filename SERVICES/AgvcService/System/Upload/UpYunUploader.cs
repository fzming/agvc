using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Utility.Extensions;

namespace AgvcService.System.Upload
{

    /// <summary>
    /// 又拍云上传实现
    /// </summary>
    internal class UpYunUploader : IUpYunUploader
    {
        private IConfiguration Configuration { get; }
        public Dictionary<string, object> ResponseHeaders { get; set; }
        private UpYunConfig _setting;
        private UpYunConfig Setting
        {
            get { return _setting ??= Configuration.GetSection("UpYun").Get<UpYunConfig>(); }
        }

        public UpYunUploader(IConfiguration configuration)
        {
            Configuration = configuration;

            FileSet = new FileSetting { AutoMkdir = true };
        }


        string ParseUploadPath(string filePath)
        {
            if (FileSet.AutoMkdir == false) //不自动创建目录，将采用待上传文件一致的文件目录结构
            {
                var directory = Path.GetDirectoryName(filePath);
                var folder = Regex.Replace(directory ?? throw new InvalidOperationException(), @"\w+:", "", RegexOptions.IgnoreCase);
                folder = folder.Replace("\\", "/").Trim('/');
                var fileName = Path.GetFileName(filePath);
                var remoteFolder = folder + "/".Replace("\\", "/");
                return $"/{remoteFolder.Trim('/')}/{fileName}";
            }

            var ext = Path.GetExtension(filePath);
            return "/" + DateTime.Now.ToString("yyMMdd") + "/" + $"{filePath.GetFileMd5()}{ext}";

        }
        public async Task<string> UploadFileAsync(string filePath, bool deleteWhenUploaded = true)
        {
            var path = ParseUploadPath(filePath);
            var bytes = filePath.GetFileBytes();
            var rs = await WriteFileAsync(path, bytes);
            if (rs)
            {
                if (deleteWhenUploaded)
                {
                    try
                    {
                        File.Delete(filePath);
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
                return $"{Domain}{path}";
            }

            return string.Empty;
        }

        public async Task<string> UpdateBytesAsync(string fileName, byte[] bytes)
        {
            var path = "/" + DateTime.Now.ToString("yyMMdd") + "/" + $"{fileName}";
            var rs = await WriteFileAsync(path, bytes);
            if (rs)
            {
                return $"{Domain}{path}";
            }

            return string.Empty;
        }

        public string GetLocalServerFileUrl(string path)
        {
            try
            {
                path = path.Replace(@"\", @"/");
                var slash = path.StartsWith("/") ? "" : "/";
                var config = Configuration.GetSection("server").Get<ServerConfig>();
                return $"{config.Domain.TrimEnd('/', '\\')}{slash}{path}";
            }
            catch (Exception)
            {
                return string.Empty;
            }

        }

        private string Domain => $"http://{(Setting.UserDomain.IsNullOrEmpty() ? Setting.ApiDomain : Setting.UserDomain)}";

        #region 目录操作

        /// <summary>
        /// 获取某个子目录的占用信息
        /// </summary>
        /// <param name="url">子目录的网络路径，必须以"/"开头，例如：/20200921</param>
        /// <returns>空间占用量，失败返回 0</returns>

        public async Task<long> GetFolderUsageAsync(string url)
        {
            var headers = new Dictionary<string, object>();
            long size;
            using var resp = await CreateWorkerAsync("GET", Setting.DL + Setting.BucketName + url + "?usage", null, headers);
            try
            {
                var content = await resp.Content.ReadAsStringAsync();
                size = long.Parse(content);
            }
            catch (Exception)
            {
                size = 0;
            }

            return size;
        }
        /// <summary>
        /// 获取总体空间的占用信息
        /// </summary>
        /// <returns>空间占用量，失败返回 0</returns>
        public async Task<long> GetBucketUsageAsync()
        {
            return await GetFolderUsageAsync("/");
        }

        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="path">子目录的网络路径，必须以"/"开头，例如：/20200921</param>
        /// <returns></returns>
        public async Task<bool> MkDirAsync(string path)
        {
            var headers = new Dictionary<string, object> { { "folder", "create" } };

            using var resp = await CreateWorkerAsync("POST", ConvertBucketPath(path), null, headers);
            return resp.StatusCode == HttpStatusCode.OK;
        }


        /// <summary>
        /// 删除目录
        /// </summary>
        /// <!--DELETE /<bucket>/path/to/folder-->
        /// <param name="path">目录路径 必须以"/"开头，例如：/20200921</param>
        /// <returns></returns>
        public async Task<bool> RmDirAsync(string path)
        {
            var headers = new Dictionary<string, object>();
            return await DeleteAsync(path, headers);
        }

        /// <summary>
        /// 读取目录列表
        /// </summary>
        /// <param name="path">目录路径 必须以"/"开头，例如：/20200921</param>
        /// <returns></returns>

        public async Task<List<FolderItem>> ReadDirAsync(string path)
        {
            var headers = new Dictionary<string, object>();
            using var resp = await CreateWorkerAsync("GET", ConvertBucketPath(path), null, headers);
            var content = await resp.Content.ReadAsStringAsync();
            content = content.Replace("\t", "\\").Replace("\n", "\\");
            var ss = content.Split('\\');
            var i = 0;
            var list = new List<FolderItem>();
            while (i < ss.Length)
            {
                var fi = new FolderItem(ss[i], ss[i + 1], int.Parse(ss[i + 2]), int.Parse(ss[i + 3]));
                list.Add(fi);
                i += 4;
            }
            return list;
        }

        #endregion

        #region 文件操作
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="path">文件路径（包含文件名）</param>
        /// <returns></returns>
        public async Task<bool> DeleteFileAsync(string path)
        {
            Dictionary<string, object> headers = new Dictionary<string, object>();
            return await DeleteAsync(path, headers);
        }
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="path">又拍云远程文件路径（包含文件名）</param>
        /// <param name="data">文件内容 或 文件IO数据流</param>
        /// <returns></returns>
        private async Task<bool> WriteFileAsync(string path, byte[] data)
        {
            var headers = new Dictionary<string, object>();

            using var resp = await CreateWorkerAsync("POST", ConvertBucketPath(path), data, headers);
            return resp.StatusCode == HttpStatusCode.OK;
        }
        public async Task<byte[]> ReadFileAsync(string path)
        {
            var headers = new Dictionary<string, object>();

            using var resp = await CreateWorkerAsync("GET", ConvertBucketPath(path), null, headers);
            return await resp.Content.ReadAsByteArrayAsync();
        }
        #endregion
        #region Internal

        FileSetting FileSet { get; }

        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
        private class FileSetting
        {

            /// <summary>
            /// 是否自动创建目录
            /// </summary>
            public bool AutoMkdir { get; set; }
            /// <summary>
            /// 文件 MD5 校验码
            /// </summary>
            /// <remarks>
            /// 设置待上传文件的 Content-MD5 值（如又拍云服务端收到的文件MD5值与用户设置的不一致，将回报 406 Not Acceptable 错误）
            /// </remarks>
            public string ContentMd5 { get; set; }
            /// <summary>
            /// 设置待上传文件的 访问密钥
            /// </summary>
            /// <remarks>
            ///   * 设置待上传文件的 访问密钥（注意：仅支持图片空！，设置密钥后，无法根据原文件URL直接访问，需带 URL 后面加上 （缩略图间隔标志符+密钥） 进行访问）
            ///   * 如缩略图间隔标志符为 ! ，密钥为 bac，上传文件路径为 /folder/test.jpg ，那么该图片的对外访问地址为： http://空间域名/folder/test.jpg!bac
            /// </remarks>
            public string FileSecret { get; set; }
        }
        private async Task<HttpResponseMessage> CreateWorkerAsync(string method, string url, byte[] postData, Dictionary<string, object> headers)
        {
            using var handler = new HttpClientHandler { UseProxy = false };
            using var httpClient = new HttpClient(handler);
            using var byteContent = new ByteArrayContent(postData ?? new byte[] { });
            httpClient.BaseAddress = new Uri("http://" + Setting.ApiDomain);

            if (FileSet.AutoMkdir)
            {
                byteContent.Headers.Add("mkdir", "true");
            }

            if (postData != null)
            {
                if (FileSet.ContentMd5 != null)
                {
                    byteContent.Headers.Add("Content-MD5", FileSet.ContentMd5);

                }
                if (FileSet.FileSecret != null)
                {
                    byteContent.Headers.Add("Content-Secret", FileSet.FileSecret);

                }
            }

            if (Setting.UpAuth)
            {
                UpyunAuth(byteContent, method, url);
            }
            else
            {

                var value = Convert.ToBase64String(
                    new ASCIIEncoding().GetBytes(Setting.UserName + ":" + Setting.Password));

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", value);
            }
            //增加自定义Header
            foreach (var kv in headers.Where(kv => !headers.ContainsKey(kv.Key)))
            {
                byteContent.Headers.Add(kv.Key, kv.Value.ToString());
            }

            HttpResponseMessage responseMsg;
            if ("Get".EqualsIgnoreCase(method))
            {
                responseMsg = await httpClient.GetAsync(url);
            }
            else if ("Post".EqualsIgnoreCase(method))
            {
                responseMsg = await httpClient.PostAsync(url, byteContent);
            }
            else if ("PUT".EqualsIgnoreCase(method))
            {
                responseMsg = await httpClient.PutAsync(url, byteContent);
            }
            else if ("Delete".EqualsIgnoreCase(method))
            {
                responseMsg = await httpClient.DeleteAsync(url);
            }
            else
            {
                throw new Exception("未知method：" + method);
            }

            ResponseHeaders = new Dictionary<string, object>();
            foreach (var header in responseMsg.Headers)
            {
                if (header.Key.Length > 7 && header.Key.Substring(0, 7) == "x-upyun")
                {
                    ResponseHeaders.Add(header.Key, header.Value);
                }
            }

            return responseMsg;
        }
        /// <summary>
        /// 执行删除操作
        /// </summary>
        /// <param name="path">必须以斜杠开始</param>
        /// <param name="headers"></param>
        /// <returns></returns>
        private async Task<bool> DeleteAsync(string path, Dictionary<string, object> headers)
        {
            var resp = await CreateWorkerAsync("DELETE", ConvertBucketPath(path), null, headers);
            return resp.StatusCode == HttpStatusCode.OK;
        }

        private string ConvertBucketPath(string path)
        {
            if (path.Contains("http://")) //网络地址转换
            {
                path = path.GetRegexValue(@"http[s]*://[^\/]+?/(.*)", 1);
            }

            path = path.Replace(@"\", @"/").RegexReplace(@"[\/]+", "/");
            if (!path.StartsWith(Setting.DL))
            {
                path = Setting.DL + path;
            }

            return Setting.DL + Setting.BucketName + path;

        }


        #region  UpYun Auth 授权

        private static string Md5(string str)
        {
            using var m = MD5.Create();
            var s = m.ComputeHash(Encoding.UTF8.GetBytes(str));
            var r = BitConverter.ToString(s);
            r = r.Replace("-", "");
            return r.ToLower();
        }

        private void UpyunAuth(ByteArrayContent requestContent, string method, string uri)
        {
            var dt = DateTime.UtcNow;
            var date = dt.ToString("ddd, dd MMM yyyy HH':'mm':'ss 'GMT'", new CultureInfo("en-US"));

            requestContent.Headers.Add("Date", date);
            var body = requestContent.ReadAsStringAsync().Result;
            string auth;
            if (!string.IsNullOrEmpty(body))
                auth = Md5(method + '&' + uri + '&' + date + '&' + requestContent.ReadAsByteArrayAsync().Result.Length +
                           '&' + Md5(Setting.Password));
            else
                auth = Md5(method + '&' + uri + '&' + date + '&' + 0 + '&' + Md5(Setting.Password));
            requestContent.Headers.Add("Authorization", "UpYun " + Setting.UserName + ':' + auth);
        }

        #endregion
        #endregion
    }
}