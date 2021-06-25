using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CoreData.Core.Attributes;
using CoreData.Models;

namespace AgvcService.System.Models
{
    public class SystemFeatureModel:Model
    {
        /// <summary>
        /// 功能名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 关键字
        /// </summary>
        [NoRepeat]
        public string Key { get; set; }
        /// <summary>
        /// 功能分组名称
        /// </summary>
        public string GroupName { get; set; }
        /// <summary>
        /// 功能描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 默认值
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// 是否进行隐藏，不在列表中显示
        /// </summary>
        public bool Hidden { get; set; }
        /// <summary>
        /// 安全选项：标识此配置不会在客户端进行序列化输出
        /// </summary>
        public bool Safety { get; set; }
    }
    public class UpdateSystemFeatureModel : SystemFeatureModel, IUpdateModel
    {
        /// <summary>
        /// 功能ID
        /// </summary>
        [Required(ErrorMessage = "{0} 不能为空")]
        [DisplayName("功能ID"), ObjectId]
        public string Id { get; set; }
    }
}