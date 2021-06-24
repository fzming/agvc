using CoreData;

namespace DtoModel.Kernel
{
    /// <summary>
    /// 通用验证接口
    /// </summary>
    public interface IValidation
    {
        /// <summary>
        /// 验证元数据
        /// </summary>
        /// <returns></returns>
         Result<bool> Validate();
    }
}