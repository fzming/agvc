using CoreData;
using CoreData.Models;
using Utility;
using Utility.Extensions;

namespace AgvcCoreData.Users
{
    /// <summary>
    /// 收件地址
    /// </summary>
    public class DeliveryAddress:IValidation
    {
        /// <summary>
        /// 单位名称，公司名称（可选）
        /// </summary>
        public string Company { get; set; }
        /// <summary>
        /// 用户姓名
        /// </summary>
        public string User { get; set; }
        /// <summary>
        /// 联系手机号
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 详细地址
        /// </summary>
        public string Address { get; set; }


        public Result<bool> Validate()
        {
            if (User.Length<2)
            {
                return Result<bool>.Fail("联系人姓名有误");
            }

            if (!Mobile.IsMobile())
            {
                return Result<bool>.Fail("联系人手机号不正确");
            }

            if (Address.Length<5){
                return Result<bool>.Fail("联系地址太短");
            }
            return Result<bool>.Successed;
        }
    }


}