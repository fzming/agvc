using AgvcCoreData.System;

namespace AgvcService.System.Models
{
    /// <summary>
    /// 创建授权码模型
    /// </summary>
    public class CreateCodeModel
    {
        /// <summary>
        /// 授权名称
        /// </summary>
        public string Name { get; set; }   
        /// <summary>
        /// 指令说明
        /// </summary>
        public string Desc { get; set; }
        /// <summary>
        /// 是否已禁用此授权码
        /// </summary>
        public bool Disabled { get; set; }
        /// <summary>
        /// [可选]所属菜单ID
        /// </summary>
        public string MenuId { get; set; }
        /// <summary>
        /// 禁用类型
        /// </summary>
        public CodeDisableType DisableType { get; set; }
    }
}