using System.Collections.Generic;
using AgvcCoreData.Users;
using AgvcService.System.Models;

namespace AgvcService.Users.Models
{
    /// <summary>
    /// 机构用户档案
    /// </summary>
    public class AccountUserProfile:AbstractUserProfile
    {
        public string CompanyId { get; set; }
        
        #region 账户余额
        /// <summary>
        /// 客户端ID
        /// </summary>
        public double BalanceTotal { get; set; }
        #endregion

        /// <summary>
        /// 限制：船公司用户角色，箱属列表
        /// </summary>
        public string[] BoxOwnerIds { get; set; }

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
        /// 功能列表
        /// </summary>
        public Dictionary<string,string> Features { get; set; }
    }
}