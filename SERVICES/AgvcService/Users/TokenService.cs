using System.Collections.Generic;
using System.Threading.Tasks;
using AgvcEntitys.Users;
using AgvcRepository.Users.Interfaces;
using CoreService;

namespace AgvcService.Users
{
    [Export(typeof(ITokenService))]
    internal class TokenService : AbstractService, ITokenService
    {
        private IAccountRefreshTokenRepository AccountRefreshTokenRepository { get; }

        [ImportingConstructor]
        public TokenService(
            IAccountRepository accountRepository,
            IAccountRefreshTokenRepository accountRefreshTokenRepository,
            IConfigManager configManager)
        {
            AccountRefreshTokenRepository = accountRefreshTokenRepository;
        }




        #region refrsh-token
        public Task<AccountRefreshToken> GetRefreshToken(string refreshToken)
        {
            return AccountRefreshTokenRepository.LastAsync(p => p.refresh_token == refreshToken);
        }
        /// <summary>
        /// 用户ID黑名单，处于此列表的用户ID将无法重新获取Ticket
        /// </summary>
        private List<string> DicTicketBlacklist=>new List<string>();
        public async Task<string> GetRefreshTokenTicket(string refreshToken)
        {    //用户可以通过refresh-token（时效较长）再次获取token令牌(时效较短)
            //提示：这里可以强制某用户的refresh-token无效，不再继续颁发token，客户端需要重新登录。
            var r = await GetRefreshToken(refreshToken);

            #region 用户验证

            if (DicTicketBlacklist.Contains(r?.IdentityName))
            {
                return string.Empty;
            }

            #endregion
            return r?.refresh_ticket;
        }

        public async Task SetRefreshToken(Refresh_Token token)
        {
            var entity = token.MapTo(new AccountRefreshToken());
            await AccountRefreshTokenRepository.InsertAsync(entity);
        }

        public Task<bool> RemoveRefreshToken(string identityName)
        {
            return AccountRefreshTokenRepository.DeleteAsync(p => p.IdentityName == identityName);
        }

        public Task RemoveRefreshTokenEx(string contextToken)
        {
            return AccountRefreshTokenRepository.DeleteAsync(p => p.refresh_token == contextToken);

        }

        #endregion
    }
}