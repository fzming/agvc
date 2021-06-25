using CoreData;
using CoreData.Models;
using Utility;
using Utility.Extensions;

namespace AgvcService.Users.Models
{
    /// <summary>
    /// 开具发票模型
    /// </summary>
    public class MakeReceiptModel : IValidation
    {
        /// <summary>
        /// 发票ID
        /// </summary>
        public string ReceiptId { get; set; }
        /// <summary>
        /// 快递单号
        /// </summary>
        public string ExpressNumber { get; set; }
        /// <summary>
        /// 快递公司
        /// </summary>
        public string ExpressCompany { get; set; }

        /// <summary>
        /// 验证开票模型
        /// </summary>
        /// <returns></returns>
        public Result<bool> Validate()
        {
            if (ReceiptId.IsNullOrEmpty())
            {
                return Result<bool>.Fail("开票记录ID不能为空");
            }

            if (ExpressCompany.IsNullOrEmpty())
            {
                return Result<bool>.Fail("请输入物流公司名称");
            }

            if (ExpressNumber.IsNullOrEmpty())
            {
                return Result<bool>.Fail("请输入快递单号");
            }

            return Result<bool>.Successed;
        }
    }
}