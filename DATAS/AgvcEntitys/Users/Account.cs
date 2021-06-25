using System.Collections.Generic;
using AgvcCoreData.Users;

namespace AgvcEntitys.Users
{
    /// <summary>
    /// 用户账户,包含货代用户，堆场用户
    /// </summary>
    public class Account : UserAccountBase
    {
        /// <summary>
        /// 公司ID
        /// </summary>
        public string CompanyId { get; set; }


        #region APP设备信息

        /// <summary>
        /// APP设备信息
        /// </summary>
        public List<AppOpenIdentify> AppUserIdentifys { get; set; }
        /// <summary>
        /// APP用户信息
        /// </summary>
        public AppUserInfo AppUserInfo { get; set; }

        #endregion


        /// <summary>
        /// 禁止登陆
        /// 单纯创建用户，不设置用户名和密码
        /// </summary>
        public bool ForbiddenLogin { get; set; }
        /// <summary>
        /// 幽灵账号，将不会在账户管理中显示
        /// </summary>
        public bool Ghost { get; set; }
        /// <summary>
        /// 限制：船公司用户角色，箱属列表
        /// </summary>
        public string[] BoxOwnerIds { get; set; }

        /// <summary>
        /// 源设备ID
        /// </summary>
        public string DeviceId { get; set; }
        #region 分公司和部门

        /// <summary>
        /// 分公司ID
        /// </summary>
        public string BranchCompanyId { get; set; }
        /// <summary>
        /// 部门ID
        /// </summary>
        public string DepartmentId { get; set; }

        #endregion
    }
}