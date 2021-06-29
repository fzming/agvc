using System.Security.Claims;
using AgvcService.Users.Models;
using CoreData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        /// <summary>
        ///     用户ID
        /// </summary>
        public string ClientId
        {
            get
            {
                return User.Claims.
            }
        }

        /// <summary>
        ///     机构ID
        /// </summary>
        public string OrgId => AuthorizedUser.OrgId;

        /// <summary>
        ///     当前授权用户
        /// </summary>
        protected Identity CurrentUser => new()
        {
            Name = AuthorizedUser.ClientName,
            Id = ClientId
        };

        #region 构造函数

        public AuthorizedUser AuthorizedUser { get; }


        #endregion

        #region Overrides of ControllerBase

        /// <summary>
        /// Creates a <see cref="T:Microsoft.AspNetCore.Mvc.SignInResult" />.
        /// </summary>
        /// <param name="principal">The <see cref="T:System.Security.Claims.ClaimsPrincipal" /> containing the user claims.</param>
        /// <returns>The created <see cref="T:Microsoft.AspNetCore.Mvc.SignInResult" /> for the response.</returns>
        public override SignInResult SignIn(ClaimsPrincipal principal)
        {
            return base.SignIn(principal);
        }

        #endregion
    }
}