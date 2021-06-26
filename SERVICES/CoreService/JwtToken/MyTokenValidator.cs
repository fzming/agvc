using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace CoreService.JwtToken
{
    /// <summary>
    ///     自定义token校验
    /// </summary>
    public class MyTokenValidator : ISecurityTokenValidator
    {
        public int MaximumTokenSizeInBytes { get; set; }

        public bool CanReadToken(string securityToken)
        {
            return true;
        }

        public bool CanValidateToken => true;

        //验证token
        public ClaimsPrincipal ValidateToken(string securityToken, TokenValidationParameters validationParameters,
            out SecurityToken validatedToken)
        {
            var jwtToken = AESCryptoHelper.Decrypt(securityToken);
            var principal =
                new JwtSecurityTokenHandler().ValidateToken(jwtToken, validationParameters, out validatedToken);
            return principal;
        }
    }
}