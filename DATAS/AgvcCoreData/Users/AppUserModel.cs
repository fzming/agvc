using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CoreData.Core.Attributes;
using CoreData.Models;

namespace AgvcCoreData.Users
{
    public class AppUserModel : Model
    {
        /// <summary>
        /// 应用ID
        /// </summary>
        public string AppId { get; set; }
        /// <summary>
        /// OpenId
        /// </summary>
        public string OpenId { get; set; }
        /// <summary>
        /// UnionId
        /// </summary>
        public string UnionId { get; set; }

        /// <summary>
        /// 已绑定的手机号
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 创建时指定往来单位ID（可选）
        /// </summary>
        public string ConsignorId { get; set; }

    }

    public class UpdateAppUserModel : AppUserModel, IUpdateModel
    {
        /// <summary>
        /// 主键ID值
        /// </summary>
        [Required(ErrorMessage = "{0} 不能为空")]
        [DisplayName("用户ID"), ObjectId]
        public string Id { get; set; }
    }
}