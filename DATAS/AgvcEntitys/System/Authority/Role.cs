using CoreData.Core;

namespace AgvcEntitys.System.Authority
{
    /// <summary>
    /// 机构用户角色表
    /// </summary>
    public class Role:OEntity
    {
        /// <summary>
        /// 角色名称
        /// </summary>
        public string Name { get; set; }
 
        /// <summary>
        /// 角色描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 角色级别 （最高100）
        /// 普通角色设定不超过99
        /// </summary>
        public int Level { get; set; }

    }
}