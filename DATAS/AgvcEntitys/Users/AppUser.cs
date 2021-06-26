using CoreData.Core;

namespace AgvcEntitys.Users
{
    /// <summary>
    ///     移动端用户
    /// </summary>
    public class AppUser : OEntity
    {
        #region APP基础信息

        /// <summary>
        ///     应用ID
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        ///     OpenId
        /// </summary>
        public string OpenId { get; set; }

        /// <summary>
        ///     UnionId
        /// </summary>
        public string UnionId { get; set; }

        #endregion


        #region 绑定内容

        /// <summary>
        ///     已绑定的手机号
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        ///     已绑定的ConsignorId
        /// </summary>
        public string ConsignorId { get; set; }

        #endregion
    }
}