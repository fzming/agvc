using System.Threading.Tasks;
using AgvcService.System.Models;
using CoreService.Interfaces;

namespace AgvcService.System
{
    /// <summary>
    ///     账号保护服务
    /// </summary>
    public interface IPassportProtectService : IService
    {
        /// <summary>
        ///     获取登录失败次数
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        Task<int> GetLoginFailedCountAsync(string clientId, string scope);

        /// <summary>
        ///     增加登录失败次数
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        Task<double> IncreaseLoginFailedAsync(string clientId, string scope);

        /// <summary>
        ///     清除登录失败记录
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        Task<bool> ClearLoginFailedAsync(string clientId, string scope);

        /// <summary>
        ///     腾讯云核查验证码票据结果
        /// </summary>
        /// <param name="captchaRequest"></param>
        /// <returns></returns>
        Task<bool> ValidateCaptchaResult(CaptchaRequest captchaRequest);
    }
}