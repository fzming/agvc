namespace DtoModel.Kernel
{
    /// <summary>
    /// 元数据模型抽象基类
    /// </summary>
    public abstract class Model : IValidation
    {
        /// <summary>
        /// 验证元数据
        /// </summary>
        /// <returns></returns>
        public virtual Result<bool> Validate()
        {
            var r = ValidationHelper.Validate(this);
            if (r.IsValid)
            {
                return Result<bool>.Successed;
            }

            return Result<bool>.Fail(r.ErrorMembers.FirstOrDefault()?.ErrorMessage);
        }


    }
}