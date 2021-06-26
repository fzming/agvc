namespace AgvcService.System.Models
{
    public class ChangePasswordModel
    {
        /// <summary>
        ///     旧密码
        ///     注意：当旧密码传入客户ID时会忽略旧密码验证
        /// </summary>
        public string OldPwd { get; set; }

        /// <summary>
        ///     新密码
        /// </summary>
        public string NewPwd { get; set; }
    }
}