using System;
using System.Net;
using Utility.Extensions;

namespace AgvcService.System.Http
{
    /// <summary>
    ///     讯代理实现
    /// </summary>
    public class XdHttpProxy : IHttpProxy
    {
        public XdHttpProxy()
        {
            Proxy = new WebProxy("forward.xdaili.cn", 80);
        }

        public WebProxy Proxy { get; set; }

        public void SetProxy(ref HttpWebRequest request)
        {
            var timestamp = DateTime.Now.ToJsTimestamp().ToString().Left(10); //时间戳
            var orderno = "ZF20179261302kkyXMa"; //订单号
            var secret = "217fce5517e448409025ca396b9fa15d"; //API密钥
            var authheader = GetProxyAuthHeader(orderno, secret, timestamp);
            request.Headers.Add("Proxy-Authorization", authheader);
            request.Proxy = Proxy;
        }


        /// <summary>
        ///     获取API 动态转发
        ///     http://www.xdaili.cn/branches
        /// </summary>
        /// <param name="orderno"></param>
        /// <param name="secret"></param>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        private static string GetProxyAuthHeader(string orderno, string secret, string timestamp)
        {
            //拼装签名字符串
            var planText = $"orderno={orderno},secret={secret},timestamp={timestamp}";
            //计算签名
            var sign = planText.Md532().ToUpper();
            //拼装请求头Proxy-Authorization的值
            var authHeader = $"sign={sign}&orderno={orderno}&timestamp={timestamp}";
            return authHeader;
        }
    }
}