using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AgvcService.System.Http;
using AgvcService.System.Message.Address;
using AgvcService.System.Message.Contents;
using CoreData;
using Microsoft.Extensions.Configuration;
using Utility.Extensions;

namespace AgvcService.System.Message.Senders.Sms
{
    /// <summary>
    ///     短信发送
    /// </summary>
    public class SmsSender : IMessageSender
    {
        public SmsSender(IHttpRequest request, IConfiguration configuration)
        {
            Request = request;
            Config = configuration.GetSection("Sms").Get<SmsConfig>();
        }

        private IHttpRequest Request { get; }

        private SmsConfig Config { get; }

        public MessageTransport Ttransport => MessageTransport.Sms;

        /// <summary>
        ///     发送异步消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="content"></param>
        /// <param name="receiver"></param>
        /// <returns></returns>
        public async Task<Result<bool>> SendAsync<T>(T content, IMessageReceiver receiver) where T : IMessageContent
        {
            var message = content as SmsContent;
            var addr = receiver as SmsReceiver;

            #region 构造参数

            var timestamp = GetTimeStamp(false);
            var para = new
            {
                account = Config.uid, //账户
                password = GetMd5(Config.pwd + addr?.Mobile + timestamp).ToLower(), //密码
                mobile = addr?.Mobile,
                content = Getuft8(AddSign(message?.Content)),
                timestamps = timestamp
            };

            #endregion

            var url = Config.api;
            var anonymousType = new {Rets = new SmsRet[] { }};
            var obj = await Request.PostAnonymousAsync(url, anonymousType, para);
            if (obj == null || !obj.Rets.Any()) return Result<bool>.Fail("短信发送失败");
            var respCodeEnum = obj.Rets[0].Rspcode.ToEnum<RespCodeEnum>();
            return respCodeEnum == RespCodeEnum.成功
                ? Result<bool>.Successed
                : Result<bool>.Fail(respCodeEnum.ToString());
        }


        #region Privates

        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        [SuppressMessage("ReSharper", "IdentifierTypo")]
        internal class SmsRet
        {
            /// <summary>
            ///     响应码   见附录- respcode
            /// </summary>
            /// <remarks>
            ///     附录- respcode
            ///     -1 程序异常
            ///     0	成功
            ///     1	用户鉴权错误
            ///     2	IP鉴权错误
            ///     3	手机号码在黑名单
            ///     4	手机号码格式错误
            ///     5	短信内容有误
            ///     7	手机号数量超限
            ///     8	账户已停用
            ///     9	未知错误
            ///     10	时间戳已过期
            ///     11	同号码同模板发送频率过快
            ///     12	同号码同模板发送次数超限
            ///     13	包含敏感词
            ///     14	扩展号不合法
            ///     15	扩展信息长度过长
            ///     99	账户余额不足
            /// </remarks>
            public int Rspcode { get; set; }

            /// <summary>
            ///     信息标识，用于对应状态报告,响应码不成功时无此内容
            /// </summary>
            public string Msg_Id { get; set; }

            /// <summary>
            ///     手机号
            /// </summary>
            public string Mobile { get; set; }

            /// <summary>
            ///     扩展信息为空时无这个字段
            /// </summary>
            public string ExtInfo { get; set; }

            /// <summary>
            ///     计费数
            /// </summary>
            public int Fee { get; set; }
        }

        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        internal enum RespCodeEnum
        {
            成功 = 0,
            用户鉴权错误 = 1,
            Ip鉴权错误 = 2,
            手机号码在黑名单 = 3,
            手机号码格式错误 = 4,
            短信内容有误 = 5,
            手机号数量超限 = 7,
            账户已停用 = 8,
            未知错误 = 9,
            时间戳已过期 = 10,
            同号码同模板发送频率过快 = 11,
            同号码同模板发送次数超限 = 12,
            包含敏感词 = 13,
            扩展号不合法 = 14,
            扩展信息长度过长 = 15,
            手机号重复 = 16,
            批量发送文件为空 = 17,
            Json解析错误 = 18,
            用户已退订 = 19,
            短信内容超过1000字符 = 20,
            账户余额不足 = 99
        }

        /// <summary>
        ///     添加短信签名
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private string AddSign(string content)
        {
            if (Config.sign.IsNullOrEmpty()) return content;
            if (content.Contains(Config.sign)) return content;
            return $"{content}{Config.sign}";
        }

        /// <summary>
        ///     获取当前时间戳
        /// </summary>
        /// <param name="bflag">为真时获取10位时间戳,为假时获取13位时间戳.</param>
        /// <returns></returns>
        private string GetTimeStamp(bool bflag = true)
        {
            var ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var ret = bflag
                ? Convert.ToInt64(ts.TotalSeconds).ToString()
                : Convert.ToInt64(ts.TotalMilliseconds).ToString();

            return ret;
        }

        private string Getuft8(string unicodeString)
        {
            var utf8 = new UTF8Encoding();
            var encodedBytes = utf8.GetBytes(unicodeString);
            var decodedString = utf8.GetString(encodedBytes);
            return decodedString;
        }

        /// <summary>
        ///     MD5　32位加密
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string GetMd5(string input)
        {
            var md5 = MD5.Create(); //实例化一个md5对像
            var s = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            return s.Aggregate("", (current, t) => current + t.ToString("X2"));
        }

        #endregion
    }
}