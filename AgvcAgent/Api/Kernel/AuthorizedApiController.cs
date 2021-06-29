using System.Collections.Generic;
using System.Security.Claims;
using AgvcService.Users.Models;
using CoreData;
using CoreService.JwtToken;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharpCompress;

namespace AgvcAgent.Api.Kernel
{
    /// <summary>
    ///     符合身份验证基类
    ///     注意：[AllowAnonymous] 将跳过身份授权
    /// </summary>
    [Authorize]
    [ApiController]
    public class AuthorizedApiController : ControllerBase
    {
        private Lazy<JwtTokenUser> _lazyTokenUser;

        public AuthorizedApiController()
        {

            _lazyTokenUser = new Lazy<JwtTokenUser>(() => {
                 
                if (HttpContext.User.Identity is ClaimsIdentity identity)
                {
                    return GetTokenUser(identity);

                }
                var clams = User.Claims;
                return null;
            });
        }

        [NonAction]
        private JwtTokenUser GetTokenUser(ClaimsIdentity identity)
        {

            return new JwtTokenUser
            {
                UserID =  User.FindFirstValue(ClaimTypes.NameIdentifier),
                Email = User.FindFirstValue(ClaimTypes.Email),
                Name = User.FindFirstValue(ClaimTypes.Name),
                Role = User.FindFirstValue(ClaimTypes.Role),
                OrgId = User.FindFirstValue(ClaimTypes.GroupSid)
            };
        }
        /// <summary>
        ///     用户ID
        /// </summary>
        public string ClientId => _lazyTokenUser.Value.UserID;

        /// <summary>
        ///     机构ID
        /// </summary>
        public string OrgId => _lazyTokenUser.Value.OrgId;
        /// <summary>
        ///  角色ID
        /// </summary>
        public string RoleId => _lazyTokenUser.Value.Role;

        /// <summary>
        ///     当前授权用户
        /// </summary>
        protected Identity CurrentUser => new()
        {
            Name = _lazyTokenUser.Value.Name,
            Id = ClientId
        };


       
    }
}