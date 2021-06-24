using System;
using System.ComponentModel.Composition;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.Infrastructure;
using Nito.AsyncEx;
using WeiMan.Common.Interface;
using WeiMan.Common.Interface.Extensions;
using WeiMan.Common.Interface.Ioc;
using WeiMan.Service.Interface.Users;

namespace WeiMan.OpenApi.OAuth
{
    /// <summary>
    /// RefreshToken生成和发放
    /// </summary>
    public class OpenRefreshTokenProvider : AuthenticationTokenProvider
    {
        [Import(typeof(ITokenService))]
        private ITokenService TokenService { get; set; }
        private readonly AsyncLock _mutex;
        public OpenRefreshTokenProvider()
        {
            _mutex = new AsyncLock();
            Injector.SatisfyImportsOnce(this);
        }
        /// <summary>
        /// Refresh token 的生成、发放、保存
        /// 生成 refresh_token
        /// </summary>
        public override async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            var client_id = context.Ticket.Identity.GetUserId();
            using (await _mutex.LockAsync())
            {
                // var refresh_token = await GenerateOAuthClientSecretAsync(client_id);
                var refresh_token = Guid.NewGuid().ToString("n");//生成refresh-ticket
                var token = new Refresh_Token
                {
                    IssuedUtc = DateTime.UtcNow,
                    ExpiresUtc = DateTime.UtcNow.AddDays(30),
                    refresh_token = refresh_token,
                    IdentityName = client_id
                };
                //创建时间
                context.Ticket.Properties.IssuedUtc = token.IssuedUtc;
                //过期时间
                context.Ticket.Properties.ExpiresUtc = token.ExpiresUtc;
                //允许刷新
                context.Ticket.Properties.AllowRefresh = true;

                token.refresh_ticket = context.SerializeTicket();
                //删除旧Ticket

                await TokenService.RemoveRefreshToken(token.IdentityName);
                //存储refresh-token
                await TokenService.SetRefreshToken(token);

                context.SetToken(refresh_token);
            }

        }
        async Task<string> GenerateOAuthClientSecretAsync(string client_id = "")
        {
            //！！！ http://stackoverflow.com/questions/23652166/how-to-generate-oauth-2-client-id-and-secret
            return await Task.Run(() =>
            {
                //byte[] salt = Guid.NewGuid().ToByteArray();
                var salt = new byte[32];
                if (client_id.IsNotNullOrEmpty())
                {
                    salt = client_id.ToByteArray();
                }
                //System.Security.Cryptography.Rfc2898DeriveBytes
                using var provider = new RNGCryptoServiceProvider();
                // generated salt
                provider.GetBytes(salt);

                return Convert.ToBase64String(salt).TrimEnd('=').Replace('+', '-').Replace('/', '_');
            });

            /*
              https://msdn.microsoft.com/zh-cn/library/system.security.cryptography.rngcryptoserviceprovider.aspx
              http://bitoftech.net/2014/12/15/secure-asp-net-web-api-using-api-key-authentication-hmac-authentication/
              using (var cryptoProvider = new RNGCryptoServiceProvider())
              {
                  byte[] secretKeyByteArray = new byte[32]; //256 bit
                  cryptoProvider.GetBytes(secretKeyByteArray);
                  var APIKey = Convert.ToBase64String(secretKeyByteArray);
              }
            */
        }
        /// <summary>
        /// 验证持有 refresh token 的客户端
        /// 由 refresh_token 解析成 access_token
        /// </summary>
        public override async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            //用户可以通过refresh-token（时效较长）再次获取token令牌(时效较短)
            //提示：这里可以强制某用户的refresh-token无效，不再继续颁发token，客户端需要重新登录。

            var value = await TokenService.GetRefreshTokenTicket(context.Token);
            if (!value.IsNullOrEmpty())
            {
                context.DeserializeTicket(value);
                //获取后立即删除当前ticket
                await TokenService.RemoveRefreshTokenEx(context.Token);
            }

        }

    }
}