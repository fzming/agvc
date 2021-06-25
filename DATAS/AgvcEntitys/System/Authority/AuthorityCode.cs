using AgvcCoreData.System;
using CoreData.Core;

namespace AgvcEntitys.System.Authority
{
    /// <summary>
    /// 操作指令
    /// </summary>
    public class AuthorityCode:MongoEntity
    {
        /// <summary>
        /// 授权码值 系统唯一值
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 授权名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 指令说明
        /// </summary>
        public string Desc { get; set; }
        /// <summary>
        /// 是否已失效
        /// </summary>
        public bool Disabled { get; set; }
        /// <summary>
        /// [可选]所属菜单ID
        /// </summary>
        public string MenuId { get; set; }
        /// <summary>
        /// 禁用性质
        /// </summary>
        public CodeDisableType DisableType { get; set; }
    }
}