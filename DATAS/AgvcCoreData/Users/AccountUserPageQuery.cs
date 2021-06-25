using CoreData;

namespace AgvcCoreData.Users
{
    public class AccountUserPageQuery : PageQuery
    {
        public string Mobile { get; set; }
        /// <summary>
        /// 分公司ID
        /// </summary>
        public string BranchCompanyId { get; set; }
        /// <summary>
        /// 部门ID
        /// </summary>
        public string DepartmentId { get; set; }
    }
}