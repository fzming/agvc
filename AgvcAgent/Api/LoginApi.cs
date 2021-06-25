using System;
using System.Threading.Tasks;
using AgvcService;
using CoreService.JwtToken;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgvcAgent.Api
{
    [ApiController]
    [Route("user")]
    public class LoginApi : ControllerBase
    {


        [HttpGet,Route("test")]
        public string Test()
        {
            return "DateTime.Now：" + DateTime.Now;
        }

        private ILoginService UserService { get; }
        private ITokenBuilder TokenBuilder { get; }

        public LoginApi(ILoginService userService, ITokenBuilder tokenBuilder)
        {
            UserService = userService;
            TokenBuilder = tokenBuilder;
        }

        /// <summary>
        /// 登录，生成加密token
        /// </summary>
        /// <returns></returns>
        [HttpGet,Route("token")]
        public async Task<string> LoginAsync()
        {
            //登录验证获取用户信息后生成JWT-TOKEN
            var logined = await UserService.LoginAsync();
            var user = new JwtTokenUser(1, "fan", "410577910@qq.com", "admin");
            string signToken = TokenBuilder.CreateJwtToken(user);
            return signToken;
        }
    }
}