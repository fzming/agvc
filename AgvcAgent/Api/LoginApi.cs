using System;
using System.Threading.Tasks;
using AgvcAgent.Api.Filters.GlobalFilters;
using AgvcService;
using AgvcService.System;
using AgvcService.System.Models.Authorization;
using AgvcService.Users.Models;
using CoreService.JwtToken;
using Microsoft.AspNetCore.Mvc;

namespace AgvcAgent.Api
{
    /*
     *不要通过从 Controller 类派生来创建 Web API 控制器。 Controller 派生自 ControllerBase，并添加对视图的支持，因此它用于处理 Web 页面，而不是 Web API 请求。 此规则有一个例外：如果打算为视图和 Web API 使用相同的控制器，则从 Controller 派生控制器。
     *
     */
    [ApiController]
    [Route("user")]
    public class LoginApi : ControllerBase
    {
        public LoginApi(IOpenAuthorizationService openAuthorizationService, ITokenBuilder tokenBuilder)
        {
            OpenAuthorizationService = openAuthorizationService;
            TokenBuilder = tokenBuilder;
        }
        private IOpenAuthorizationService OpenAuthorizationService { get; }
        private ITokenBuilder TokenBuilder { get; }


        [HttpGet]
        [Route("test")]
        public string Test()
        {
            return "DateTime.Now：" + DateTime.Now;
        }

        /// <summary>
        ///     登录，生成加密token
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("token")]
        public async Task<dynamic> LoginAsync([FromForm] PasswordLoginModel loginModel)
        {
            //登录验证获取用户信息后生成JWT-TOKEN
            var result = await OpenAuthorizationService.LoginAccountWithPasswordAsync(loginModel);
            if (result.Success)
            {
                var data = result.Data;//account user => jwtTokenUser
                var user = new JwtTokenUser(data.Id, data.Nick, data.Email, data.RoleId);
                var signToken = TokenBuilder.CreateJwtToken(user);
                return new 
                {
                    token_type="Bearer",
                    access_token= signToken
                };
            }

            return null;
        }
    }
}