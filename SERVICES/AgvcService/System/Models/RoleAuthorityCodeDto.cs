using AgvcCoreData.System;

namespace AgvcService.System.Models
{
    /// <summary>
    ///     角色授权指令模型
    /// </summary>
    public class RoleAuthorityCodeDto
    {
        /// <summary>
        ///     授权码值 系统唯一值
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        ///     授权名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     是否已失效
        /// </summary>
        public bool Disabled { get; set; }

        /// <summary>
        ///     指令说明
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        ///     禁用类型
        /// </summary>
        public CodeDisableType DisableType { get; set; }

        /// <summary>
        ///     当前用户是否拥有此权限
        /// </summary>
        public bool HasPermission { get; set; }
    }
}