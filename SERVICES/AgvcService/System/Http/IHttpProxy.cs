using System.Net;
using Utility;

namespace AgvcService.System.Http
{
    public interface IHttpProxy : ITransientDependency
    {
        /// <summary>
        ///     代理地址对象
        /// </summary>
        WebProxy Proxy { get; }

        /// <summary>
        ///     为HTTP请求增加代理
        /// </summary>
        /// <param name="request">http请求</param>
        void SetProxy(ref HttpWebRequest request);
    }
}