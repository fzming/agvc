using AgvcCoreData.System;
using AgvcEntitys.Organization;

namespace AgvcService.Organizations.Models
{
    /// <summary>
    /// 创建机构模型
    /// </summary>
    public class CreateOrgModel
    {
        #region 机构基本资料

        /// <summary>
        /// 机构名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 机构类型
        /// </summary>
        public OrganizationType PrimaryType { get; set; }

        /// <summary>
        /// 机构缩写
        /// </summary>
        public string Prefix { get; set; }

        #endregion

        #region 机构管理员

        /// <summary>
        /// 机构类型为货代时:传入手机号
        /// 机构类型为系统时：不限制
        /// </summary>
        public string LoginId { get; set; }
        /// <summary>
        /// 机构管理员姓名
        /// </summary>
        public string Nick { get; set; }
        /// <summary>
        /// 管理员登录密码
        /// </summary>
        public string LoginPwd { get; set; }
        /// <summary>
        /// 管理员角色
        /// </summary>
        public string RoleId { get; set; }

        #endregion
        /// <summary>
        /// 机构包括模块 至少一种
        /// </summary>
        public ModuleType[] Modules { get; set; }
    }
}