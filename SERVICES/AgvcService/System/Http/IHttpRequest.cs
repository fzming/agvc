using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Utility;

namespace AgvcService.System.Http
{
    /// <summary>
    ///     HTTP请求
    /// </summary>
    public interface IHttpRequest : ITransientDependency
    {
        /// <summary>
        ///     自定义标头
        /// </summary>
        Dictionary<string, string> Headers { get; set; }

        /// <summary>
        ///     移除代理对象
        /// </summary>
        void RemoveHttpProxy();

        /// <summary>
        ///     添加代理对象
        /// </summary>
        /// <param name="proxy"></param>
        void AddHttpProxy(IHttpProxy proxy);

        void PostJson(string url, string reffer, object obj, Action<string> callback);
        Task<string> PostJsonAsync(string url, object obj, Action<HttpWebRequest> reqAction);
        void GetJson(string url, string reffer, object obj, Action<string> callback);
        Task<string> GetJsonAsync(string url, object param, Action<HttpWebRequest> reqAction);
        T Parse<T>(string result);
        HttpWebRequest CreateWebRequest(string url, HttpMethod httpMethod, CookieContainer cookie);
        Task<HttpWebResponse> GetResponseAsync(string url, HttpMethod httpMethod, CookieContainer cookie);

        /// <summary>
        ///     查看网页源代码
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        Task<string> ViewSourceAsync(string url);

        Task<string> GetStringAsync(string url, CookieContainer cookie = null);
        Task<string> GetStringAsync(string url, object param, CookieContainer cookie = null);
        Task<T> GetAsync<T>(string url);
        Task<T> PostAsync<T>(string url, object postObject = null);
        Task<T> PostAsync<T>(string url, X509Certificate cert);

        /// <summary>
        ///     将自定义创建的Request对象通过Post进行发送
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <param name="postObject"></param>
        /// <returns></returns>
        Task<T> PostAsync<T>(HttpWebRequest request, object postObject = null);

        Task<T> GetAnonymousAsync<T>(string url, T anonymousType, object param = null, CookieContainer cookie = null,
            Func<string, string> contentCallback = null);

        Task<T> PostAnonymousAsync<T>(string url, T anonymousType, object param = null);

        /// <summary>
        ///     设置单次有效的User_Agent
        /// </summary>
        /// <param name="userAgent"></param>
        void SetMomentUserAgent(string userAgent);
    }
}