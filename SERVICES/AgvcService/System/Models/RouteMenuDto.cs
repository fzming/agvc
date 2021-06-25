using AgvcCoreData.System;

namespace AgvcService.System.Models
{
    /// <summary>
    /// 无限级路由菜单模型
    /// </summary>
    public class RouteMenuDto: RouteMenuModel
    {

        /// <summary>
        /// 子路由
        /// </summary>
        public RouteMenuDto[] Children { get; set; }
       

    }
    /// <summary>
    /// 路由菜单模型
    /// </summary>
    public class RouteMenuModel
    {
        /// <summary>
        /// 菜单ID
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 父级菜单ID
        /// </summary>
        public string PaMenuId { get; set; }
        /// <summary>
        /// 父级菜单路径
        /// </summary>
        public string PaMenuPath { get; set; }
        /// <summary>
        /// 菜单名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 菜单地址
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// 组件视图路径
        /// </summary>
        public string Component { get; set; }
        /// <summary>
        /// Meta元数据
        /// </summary>
        public MenuMeta Meta { get; set; }
        /// <summary>
        /// 跳转参数
        /// </summary>
        public string Redirect { get; set; }
        /// <summary>
        /// 根目录展示
        /// </summary>
        public bool AlwaysShow { get; set; }

        /// <summary>
        /// 当设置 true 的时候该路由不会再侧边栏出现 如401，login等页面，或者如一些编辑页面/edit/1
        /// (默认 false)
        /// </summary>
        public bool Hidden { get; set; }

        /// <summary>
        /// 隶属模块
        /// </summary>
        public ModuleType[] Modules { get; set; }
    }

    public class ChangeMenuModule
    {
         public string MenuId { get; set; }
         /// <summary>
         /// 隶属模块
         /// </summary>
         public ModuleType[] Modules { get; set; }
    }
}