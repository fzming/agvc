using System.Threading.Tasks;
using AgvcService.System.Models;
using CoreService;

namespace AgvcService.System
{
    /// <summary>
    /// 账号保护服务实现
    /// </summary>
    [Export(typeof(IPassportProtectService))]
    internal class PassportProtectService:AbstractService,IPassportProtectService
    {
        private  CaptchaCredential _captchaCredential;

        #region IOC

        private IRedisHashCache RedisHashCache { get; }
        private IConfigManager ConfigManager { get; }

        private CaptchaCredential CaptchaCredential
        {
            get
            {
                return _captchaCredential ??=ConfigManager.GetConfig<CaptchaCredential>("TCaptcha");
            }
        }

        [ImportingConstructor]
        public PassportProtectService(IRedisHashCache redisHashCache,IConfigManager configManager)
        {
            RedisHashCache = redisHashCache;
            ConfigManager = configManager;
           
        }

        #endregion
        
        public Task<int> GetLoginFailedCountAsync(string clientId, string scope)
        {
          return  RedisHashCache.HashGetAsync<int>(scope, clientId);
        }

        public async Task<double> IncreaseLoginFailedAsync(string clientId, string scope)
        {
            var failures = await RedisHashCache.HashGetAsync<int>(scope, clientId);
            if (failures==0)
            {
                failures ++;
                await RedisHashCache.HashSetAsync(scope, clientId, failures);
                return failures;
            }
            else
            {
                return await RedisHashCache.HashIncrementAsync(scope, clientId);
            }
          
        }

        public Task<bool> ClearLoginFailedAsync(string clientId, string scope)
        {
            return RedisHashCache.HashDeleteAsync(scope, clientId);
        }

        /// <summary>
        /// 腾讯云核查验证码票据结果
        /// </summary>
        /// <param name="captchaRequest"></param>
        /// <returns></returns>
        public Task<bool> ValidateCaptchaResult(CaptchaRequest captchaRequest)
        {
            return Task.FromResult(false);
            // var cr = new Credential
            // {
            //     SecretId = _captchaCredential.SecretId,
            //     SecretKey = _captchaCredential.SecretKey
            // };
            // var client = new CaptchaClient(cr,"");
            //
            // var req = new DescribeCaptchaResultRequest
            // {
            //     CaptchaAppId = (ulong?) _captchaCredential.CaptchaAppId,
            //     AppSecretKey = _captchaCredential.AppSecretKey,
            //     CaptchaType = 9,
            //     Ticket = captchaRequest.ticket,
            //     Randstr = captchaRequest.randstr,
            //     UserIp = NetworkHelper.RealIpAddress
            // };
            //
            //
            //
            // var resp = await client.DescribeCaptchaResult(req);
            // /*
            //  *  response
            //  *  1:验证成功，
            //  *  0:验证失败，
            //  *  100:AppSecretKey参数校验错误
            //  *
            //     evil_level	[0,100]，恶意等级   
            //     
            //     err_msg	验证错误信息                                        
            //  *
            //  */
            // return resp.CaptchaCode == 1;
        }
    }
}