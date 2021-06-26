using AgvcCoreData.System;
using AgvcCoreData.Users;

namespace AgvcService.System.Models
{
    public abstract class AbstractUserProfile
    {
        /// <summary>
        ///     账户ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///     用户昵称
        /// </summary>
        public string Nick { get; set; }

        /// <summary>
        ///     头像地址
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        ///     邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        ///     手机号
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        ///     联系电话
        /// </summary>
        public string ContactPhone { get; set; }

        /// <summary>
        ///     性别
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        ///     用户介绍
        /// </summary>
        public string Introduction { get; set; }

        /// <summary>
        ///     角色
        /// </summary>
        public RoleDto[] Roles { get; set; }

        /// <summary>
        ///     机构ID
        /// </summary>
        public string OrgId { get; set; }

        /// <summary>
        ///     机构名称
        /// </summary>
        public string OrgName { get; set; }

        /// <summary>
        ///     分公司
        /// </summary>
        public string BranchCompany { get; set; }

        /// <summary>
        ///     分公司ID
        /// </summary>
        public string BranchCompanyId { get; set; }

        /// <summary>
        ///     部门
        /// </summary>
        public string Department { get; set; }

        /// <summary>
        ///     部门ID
        /// </summary>
        public string DepartmentId { get; set; }

        /// <summary>
        ///     机构包括模块 至少一种
        /// </summary>
        public ModuleType[] Modules { get; set; }

        /// <summary>
        ///     首次登陆强制修改密码
        /// </summary>
        public bool NeedChangePassword { get; set; }

        /// <summary>
        ///     实名身份认证
        /// </summary>
        public IdCert Cert { get; set; }

        /// <summary>
        ///     已征得车主授权
        /// </summary>
        public bool Agreement { get; set; }
    }
}