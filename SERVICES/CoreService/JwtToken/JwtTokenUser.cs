namespace CoreService.JwtToken
{
    /// <summary>
    ///     登录用户信息
    /// </summary>
    public class JwtTokenUser
    {
        public JwtTokenUser(int userID, string name, string email, string role)
        {
            UserID = userID;
            Name = name;
            Email = email;
            Role = role;
        }

        public int UserID { get; set; }
        public string Email { get; set; }

        public string Name { get; set; }
        public string Role { get; set; }
    }
}