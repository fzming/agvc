namespace AgvcService.System.Models
{
    /// <summary>
    ///     创建角色模型
    /// </summary>
    public class CreateRoleModel
    {
        /// <summary>
        ///     角色名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     角色描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     角色级别 （最高100）
        ///     普通角色设定不超过99
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        ///     授权访问的菜单ID列表或指令列表
        /// </summary>
        public string[] Authorizes { get; set; }
    }
}