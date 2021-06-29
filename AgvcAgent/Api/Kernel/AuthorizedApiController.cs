using System.Security.Claims;
using AgvcService.Users.Models;
using CoreData;
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
        private Lazy<string> lzUserClams;

        public AuthorizedApiController()
        {
            lzUserClams = new Lazy<string>(() => {
                var clams = User.Claims;
                return string.Empty;
            });
        }

        /// <summary>
        ///     用户ID
        /// </summary>
        public string ClientId
        {
            get
            {
                return lzUserClams.Value;
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

       
    }
}