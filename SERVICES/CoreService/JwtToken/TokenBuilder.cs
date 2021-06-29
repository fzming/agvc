using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Utility;

namespace CoreService.JwtToken
{
    public interface ITokenBuilder : ISingletonDependency
    {
        /// <summary>
        ///     创建加密JwtToken
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        string CreateJwtToken(JwtTokenUser user);

        /// <summary>
        ///     创建包含用户信息的CalimList
        /// </summary>
        /// <param name="authUser"></param>
        /// <returns></returns>
        List<Claim> CreateClaimList(JwtTokenUser authUser);
    }

    /// <summary>
    ///     token创建
    /// </summary>
    public class TokenBuilder : ITokenBuilder
    {
        private readonly JwtTokenOptions _tokenOptions;

        public TokenBuilder(IOptions<JwtTokenOptions> tokenOptions)
        {
            _tokenOptions = tokenOptions.Value;
        }

        /// <summary>
        ///     创建加密JwtToken
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public string CreateJwtToken(JwtTokenUser user)
        {
            var claimList = CreateClaimList(user);
            var jwtSecurityToken = new JwtSecurityToken(
                _tokenOptions.Issuer
                , _tokenOptions.Audience
                , claimList
                //, notBefore: utcNow
                , expires: DateTime.Now.AddDays(3)
                , signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenOptions.RawSigningKey)),
                    SecurityAlgorithms.HmacSha256)
            );
            var jwtToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            //加密jwtToken
            jwtToken = AESCryptoHelper.Encrypt(jwtToken);
            return jwtToken;
        }

        /// <summary>
        ///     创建包含用户信息的CalimList
        /// </summary>
        /// <param name="authUser"></param>
        /// <returns></returns>
        public List<Claim> CreateClaimList(JwtTokenUser authUser)
        {
            //身份单元项项集合
            var claimList = new List<Claim>
            {
                new(ClaimTypes.Email, authUser.Email??string.Empty), //身份单元项
                new(ClaimTypes.Name, authUser.Name),
                new(ClaimTypes.NameIdentifier, authUser.UserID.ToString()),
                new(ClaimTypes.Role, authUser.Role ?? string.Empty),
                new(ClaimTypes.GroupSid,authUser.OrgId??string.Empty)
            };
            return claimList;
        }

       
    }
}