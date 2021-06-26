using System;
using AgvcService.System.Models;
using Newtonsoft.Json;

namespace AgvcService.Users.Models
{
    public class UserDto : AbstractUserProfile
    {

        /// <summary>
        /// 登录ID 
        /// </summary>
        /// <remarks>
        /// 系统唯一，账号，手机号,邮箱，用户名
        /// </remarks>
        public string LoginId { get; set; }

        /// <summary>
        /// 来源设备
        /// </summary>
        public string Device { get; set; }
        /// <summary>
        /// 来源设备
        /// </summary>
        public string DeviceId { get; set; }
        /// <summary>
        /// 管理角色Id
        /// </summary>
        public string RoleId { get; set; }
        /// <summary>
        /// 管理角色名称
        /// </summary>
        public string RoleName { get; set; }
        public int RoleLevel { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [JsonProperty("_c")]
        public DateTime CreatedOn { get; set; }
        /// <summary>
        /// 禁止登陆
        /// 单纯创建用户，不设置用户名和密码
        /// </summary>
        public bool ForbiddenLogin { get; set; }


    }
}