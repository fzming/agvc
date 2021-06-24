
using CoreData;
using DtoModel;
using Microsoft.AspNetCore.Mvc;

namespace AgvcAgent.Api.Kernel
{
    /// <summary>
    /// 符合OAuth2.0授权的身份验证基类
    /// 注意：[AllowAnonymous] 将跳过身份授权
    /// </summary>
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class AuthorizedApiController : ControllerBase
    {

        #region 构造函数

        public AuthorizedUser AuthorizedUser { get; }
        public AuthorizedApiController()
        {
            AuthorizedUser = new AuthorizedUser(User);
        }

        #endregion
        /// <summary>
        /// 用户ID
        /// </summary>
        public string ClientId => AuthorizedUser.ClientId;
        /// <summary>
        /// 机构ID
        /// </summary>
        public string OrgId => AuthorizedUser.OrgId;

        /// <summary>
        /// 当前授权用户
        /// </summary>
        protected Identity CurrentUser => new()
        {
            Name = AuthorizedUser.ClientName,
            Id = ClientId,
        };
    }
}