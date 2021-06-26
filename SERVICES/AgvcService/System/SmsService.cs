using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgvcService.System.Message;
using AgvcService.System.Message.Address;
using AgvcService.System.Message.Contents;
using AgvcService.System.Models;
using Cache.IRedis.Interfaces;
using CoreService;

namespace AgvcService.System
{
    public class SmsService : AbstractService, ISmsService
    {
        public SmsService(IRedisStringCache redisStringCache, IEnumerable<IMessageSender> messageSenders)
        {
            MessageSenders = messageSenders;
            RedisStringCache = redisStringCache;
        }

        private IEnumerable<IMessageSender> MessageSenders { get; }
        private IRedisStringCache RedisStringCache { get; }

        private IMessageSender SmSender
        {
            get { return MessageSenders.First(p => p.Ttransport == MessageTransport.Sms); }
        }

        /// <summary>
        ///     Sends the async.
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="mobile">Mobile.</param>
        /// <param name="content">Content.</param>
        public async Task<bool> SendAsync(string mobile, string content)
        {
            var rs = await SmSender.SendAsync(new SmsContent
            {
                Content = content
            }, new SmsReceiver
            {
                Mobile = mobile
            });
            return rs.Success;
        }

        /// <summary>
        ///     发送验证码
        /// </summary>
        /// <param name="smsCodeDto"></param>
        /// <returns></returns>
        public async Task<bool> SendAuthenticationAsync(SmsCodeDto smsCodeDto)
        {
            var success = await SendAsync(smsCodeDto.Mobile, smsCodeDto.FormattedContent);
            if (success)
                return await RedisStringCache.StringSetAsync(
                    GetSendCacheKey(smsCodeDto.Key, smsCodeDto.Mobile),
                    smsCodeDto, smsCodeDto.Expires);

            return false;
        }

        /// <summary>
        ///     Validates the authentication async.
        /// </summary>
        /// <returns>The authentication async.</returns>
        /// <param name="mobile">Mobile.</param>
        /// <param name="smsCode">Sms code.</param>
        public async Task<bool> ValidateAuthenticationAsync(string mobile, SmsCodeType smsCode)
        {
            var key = GetSendCacheKey(smsCode.Key, mobile);
            var cache = await RedisStringCache.StringGetAsync<SmsCodeDto>(key);
            if (cache != null) return cache.Code == smsCode.Code;

            return false;
        }

        private string GetSendCacheKey(string key, string mobile)
        {
            return $"SmsAuth-{key}{mobile}";
        }
    }
}