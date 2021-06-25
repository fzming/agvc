using AgvcCoreData.System;
using CoreData.Core;

namespace AgvcEntitys.System
{
    /// <summary>
    /// 系统菜单
    /// </summary>
    public class Menu : MongoEntity
    {

        /// <summary>
        /// 菜单名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 路由地址
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// 组件视图
        /// </summary>
        public string Component { get; set; }

        /// <summary>
        /// 父菜单ID（菜单级别超过3级）
        /// </summary>
        public string PaMenuId { get; set; }
        /// <summary>
        /// 父菜单ID路径
        /// </summary>
        public string PaMenuPath { get; set; }
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
        /// 排序索引
        /// </summary>
        public int OrderIndex { get; set; }
        /// <summary>
        /// 隶属模块
        /// </summary>
        public ModuleType[] Modules { get; set; }

    }
}