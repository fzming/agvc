
namespace CoreData.Models
{
    /// <summary>
    /// 更新模型接口
    /// </summary>
    public interface IUpdateModel
    {
        /// <summary>
        /// 主键ID值
        /// </summary>
        // [Required(ErrorMessage = "{0} 不能为空")]
        // [DisplayName("ID"), ObjectId]
        string Id { get; set; }
    }
}