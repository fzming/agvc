using System.Threading.Tasks;
using AgvcService.System.Models;
using CoreService.Interfaces;

namespace AgvcService.System
{
    /// <summary>
    ///     短信服务
    /// </summary>
    public interface ISmsService : IService
    {
        /// <summary>
        ///     发送短信
        /// </summary>
        /// <param name="mobile">手机号（多个手机号使用逗号进行分隔）</param>
        /// <param name="content">内容</param>
        /// <returns></returns>
        Task<bool> SendAsync(string mobile, string content);

        /// <summary>
        ///     发送短信验证码(支持分布式)
        /// </summary>
        /// <param name="smsCodeDto">发送验证码模型</param>
        /// <returns></returns>
        Task<bool> SendAuthenticationAsync(SmsCodeDto smsCodeDto);

        /// <summary>
        ///     验证短信验证码
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="smsCode"></param>
        /// <returns></returns>
        Task<bool> ValidateAuthenticationAsync(string mobile, SmsCodeType smsCode);
    }
}