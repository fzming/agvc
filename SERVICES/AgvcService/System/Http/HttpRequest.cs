using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using Utility.Extensions;

namespace AgvcService.System.Http
{
    public class HttpClientRequest : IHttpRequest
    {
        /// <summary>
        ///     UserAgent
        /// </summary>
        private const string USER_AGENT =
            "Mozilla/5.0 (Windows NT 6.2; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/54.0.2840.87 Safari/537.36";

        /// <summary>
        ///     获取响应异常时的重试策略
        /// </summary>
        public readonly RetryPolicy<HttpResponseMessage> HttpRequestPolicy;

        public HttpClientRequest()
        {
            Headers = new Dictionary<string, string>();
            HttpRequestPolicy = Policy.HandleResult<HttpResponseMessage>(
                    r => r.StatusCode == HttpStatusCode.InternalServerError)
                .WaitAndRetry(3,
                    retryAttempt => TimeSpan.FromSeconds(retryAttempt));
        }

        /// <summary>
        ///     Proxy
        /// </summary>
        private IHttpProxy Proxy { get; set; }

        /// <summary>
        ///     MomentUserAgent
        /// </summary>
        private string MomentUserAgent { get; set; }

        public Dictionary<string, string> Headers { get; set; }

        public void PostJson(string url, string reffer, object obj, Action<string> callback)
        {
            var json = JsonConvert.SerializeObject(obj);
            var result = "";

            var req = (HttpWebRequest) WebRequest.Create(url);

            req.Method = "POST";
            req.Accept = "application/json";
            req.ContentType = "application/json";
            req.Referer = reffer;
            var data = Encoding.UTF8.GetBytes(json);

            req.ContentLength = data.Length;

            using (var reqStream = req.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length);

                reqStream.Close();
            }

            var resp = (HttpWebResponse) req.GetResponse();

            var stream = resp.GetResponseStream();

            //获取响应内容
            if (stream != null)
            {
                using var reader = new StreamReader(stream, Encoding.UTF8);
                result = reader.ReadToEnd();
            }

            callback.Invoke(result);
        }

        public Task<string> PostJsonAsync(string url, object obj, Action<HttpWebRequest> reqAction)
        {
            var json = JsonConvert.SerializeObject(obj);


            var req = (HttpWebRequest) WebRequest.Create(url);

            req.Method = "POST";
            req.Accept = "application/json";
            req.ContentType = "application/json";
            reqAction?.Invoke(req);
            var data = Encoding.UTF8.GetBytes(json);

            req.ContentLength = data.Length;

            using (var reqStream = req.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length);

                reqStream.Close();
            }

            var resp = (HttpWebResponse) req.GetResponse();

            return GetResponseAsStringAsync(resp);
        }

        public void GetJson(string url, string reffer, object obj, Action<string> callback)
        {
            string result;

            var request = (HttpWebRequest) WebRequest.Create(url);

            request.KeepAlive = false;
            request.Referer = url;
            request.AllowAutoRedirect = false;
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            request.Headers.Set("Pragma", "no-cache");

            request.Headers.Add("Accept-Language", "zh-cn,zh;q=0.5");
            request.Headers.Add("Accept-Encoding", "gzip,deflate");
            request.Headers.Add("Accept-Charset", "GB2312,utf-8;q=0.7,*;q=0.7");

            var resp = (HttpWebResponse) request.GetResponse();

            var stream = resp.GetResponseStream();

            //获取响应内容
            using (var reader = new StreamReader(stream ?? throw new InvalidOperationException(), Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }

            callback.Invoke(result);
        }

        public Task<string> GetJsonAsync(string url, object param, Action<HttpWebRequest> reqAction)
        {
            var request = (HttpWebRequest) WebRequest.Create($"{url}?{param.ToQueryString()}");

            request.KeepAlive = false;
            request.AllowAutoRedirect = false;
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            request.Headers.Set("Pragma", "no-cache");
            reqAction?.Invoke(request);

            request.Headers.Add("Accept-Language", "zh-cn,zh;q=0.5");
            request.Headers.Add("Accept-Encoding", "gzip,deflate");
            request.Headers.Add("Accept-Charset", "GB2312,utf-8;q=0.7,*;q=0.7");

            var resp = (HttpWebResponse) request.GetResponse();

            return GetResponseAsStringAsync(resp);
        }

        /// <summary>
        ///     创建复杂的请求对象，以应用不同的访问环境
        /// </summary>
        /// <param name="url"></param>
        /// <param name="httpMethod"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public HttpWebRequest CreateWebRequest(string url, HttpMethod httpMethod, CookieContainer cookie = null)
        {
            if (!(WebRequest.Create(url) is HttpWebRequest request)) return null;
            request.KeepAlive = false;
            request.Referer = url;
            request.AllowAutoRedirect = false;
            request.Accept = "application/json, text/javascript, */*; q=0.01";
            request.Headers.Set("Pragma", "no-cache");
            request.Headers.Add("Accept-Language", "zh-cn,zh;q=0.5");
            request.Headers.Add("Accept-Encoding", "gzip,deflate");
            //            request.Headers.Add("Accept-Charset", "GB2312,utf-8;q=0.7,*;q=0.7");
            if (Headers.AnyNullable())
                foreach (var header in Headers)
                    request.Headers.Add(header.Key, header.Value);
            request.Method = httpMethod.ToString().ToUpper();

            request.UserAgent = MomentUserAgent.IsNotNullOrEmpty() ? MomentUserAgent : USER_AGENT;
            request.Timeout = 15 * 1000; //15''
            if (cookie != null) request.CookieContainer = cookie;

            Proxy?.SetProxy(ref request);
            MomentUserAgent = string.Empty; //clear
            return request;
        }


        public async Task<HttpWebResponse> GetResponseAsync(string url, HttpMethod httpMethod, CookieContainer cookie)
        {
            var request = CreateWebRequest(url, HttpMethod.Get, cookie);
            var response = await request.GetResponseAsync() as HttpWebResponse;

            return response;
        }

        public async Task<string> ViewSourceAsync(string url)
        {
            using var wc = new WebClient {Credentials = CredentialCache.DefaultCredentials};
            using var sr = new StreamReader(await wc.OpenReadTaskAsync(url), Encoding.UTF8);
            return await sr.ReadToEndAsync();
        }

        /// <summary>
        ///     获取字符串
        /// </summary>
        /// <param name="url"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public async Task<string> GetStringAsync(string url, CookieContainer cookie = null)
        {
            var request = CreateWebRequest(url, HttpMethod.Get, cookie);

            try
            {
                var response = await request.GetResponseAsync() as HttpWebResponse;
                var result = await GetResponseAsStringAsync(response);
                return result;
            }
            catch
            {
                return string.Empty;
            }
        }

        public Task<string> GetStringAsync(string url, object param, CookieContainer cookie = null)
        {
            return GetStringAsync($"{url}?{param.ToQueryString()}", cookie);
        }

        /// <summary>
        ///     获取指定类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(string url)
        {
            var result = await GetStringAsync(url);
            return Parse<T>(result);
        }

        /// <summary>
        ///     格式转换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        /// <returns></returns>
        public T Parse<T>(string result)
        {
            if (typeof(T).IsValueType || typeof(T) == typeof(string)) //string,int 值类型
                return (T) Convert.ChangeType(result, typeof(T));

            try
            {
                return JsonConvert.DeserializeObject<T>(result);
            }
            catch
            {
                return default;
            }
        }


        public async Task<T> PostAsync<T>(HttpWebRequest request, object postObject = null)
        {
            request.Method = "POST";
            if (request.ContentType.IsNullOrEmpty())
                request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";

            //buffer 
            if (postObject != null)
            {
                var buffer = postObject.ToQueryString();
                var data = Encoding.Default.GetBytes(buffer);
                using var stream = request.GetRequestStream();
                await stream.WriteAsync(data, 0, data.Length);
            }

            //response 
            try
            {
                var response = await request.GetResponseAsync() as HttpWebResponse;
                var content = await GetResponseAsStringAsync(response);
                return Parse<T>(content);
            }
            catch (Exception)
            {
                return default;
            }
        }

        /// <summary>
        ///     根据匿名类型获取 GET
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="anonymousType">返回的匿名类型</param>
        /// <param name="param">参数</param>
        /// <param name="cookie"></param>
        /// <param name="contentCallback"></param>
        /// <returns></returns>
        public async Task<T> GetAnonymousAsync<T>(string url, T anonymousType,
            object param = null, CookieContainer cookie = null,
            Func<string, string> contentCallback = null)
        {
            var result = await GetStringAsync(url, param, cookie);

            #region JSON返回参数过滤

            var jsonPRegex = @"\w+\((\{[\s\S]*?\})\)";
            if (Regex.IsMatch(result, jsonPRegex)) result = Regex.Match(result, jsonPRegex).Groups[1].Value;

            #endregion

            if (contentCallback != null) result = contentCallback(result);
            return ParseAnonymous(result, anonymousType);
        }

        /// <summary>
        ///     根据匿名类型获取 POST
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="anonymousType">返回的匿名类型</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public async Task<T> PostAnonymousAsync<T>(string url, T anonymousType, object param = null)
        {
            var result = await PostAsync<string>(url, param);
            return ParseAnonymous(result, anonymousType);
        }

        /// <summary>
        ///     设置单次有效的UserAgent
        /// </summary>
        /// <param name="userAgent"></param>
        public void SetMomentUserAgent(string userAgent)
        {
            MomentUserAgent = userAgent;
        }


        /// <summary>
        ///     执行异步POST请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="postObject"></param>
        /// <returns></returns>
        public Task<T> PostAsync<T>(string url, object postObject = null)
        {
            var request = CreateWebRequest(url, HttpMethod.Post);
            return PostAsync<T>(request, postObject);
        }

        public Task<T> PostAsync<T>(string url, X509Certificate cert)
        {
            var request = CreateWebRequest(url, HttpMethod.Post);
            request.ClientCertificates.Add(cert);
            request.ContentType = "text/html";
            return PostAsync<T>(request);
        }

        public T ParseAnonymous<T>(string result, T anonymousType)
        {
            if (typeof(T).IsValueType || typeof(T) == typeof(string)) //string,int 值类型
                return (T) Convert.ChangeType(result, typeof(T));

            try
            {
                return JsonConvert.DeserializeAnonymousType(result, anonymousType);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return default;
            }
        }

        /// <summary>
        ///     根据响应的编码方式获取不同的内容
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        internal async Task<string> GetResponseAsStringAsync(HttpWebResponse response)
        {
            string responseBody;
            if (response == null) return string.Empty;

            if (response.ContentEncoding.ToLower().Contains("gzip"))
            {
                using var stream = new GZipStream(response.GetResponseStream() ?? throw new InvalidOperationException(),
                    CompressionMode.Decompress);
                using var reader = new StreamReader(stream);
                responseBody = await reader.ReadToEndAsync();
            }
            else if (response.ContentEncoding.ToLower().Contains("deflate"))
            {
                using var stream = new DeflateStream(
                    response.GetResponseStream() ?? throw new InvalidOperationException(), CompressionMode.Decompress);
                using var reader = new StreamReader(stream, Encoding.UTF8);
                responseBody = await reader.ReadToEndAsync();
            }
            else
            {
                using var stream = response.GetResponseStream();
                using var reader = new StreamReader(stream ?? throw new InvalidOperationException(), Encoding.UTF8);
                responseBody = await reader.ReadToEndAsync();
            }


            return responseBody;
        }

        #region 访问https 需要忽略证书验证

        /// <summary>
        ///     为了通过证书验证
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="cert"></param>
        /// <param name="chain"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        private static bool RemoteCertificateValidate(object sender, X509Certificate cert, X509Chain chain,
            SslPolicyErrors error)
        {
            //为了通过证书验证，总是返回true
            return true;
        }

        static HttpClientRequest()
        {
            ServicePointManager.DefaultConnectionLimit = 1000; //最大连接数
            //获取验证证书的回调函数
            ServicePointManager.ServerCertificateValidationCallback += RemoteCertificateValidate;
        }

        #endregion

        #region Proxy

        public void RemoveHttpProxy()
        {
            Proxy = null;
        }

        public void AddHttpProxy(IHttpProxy proxy)
        {
            Proxy = proxy;
        }

        #endregion
    }
}