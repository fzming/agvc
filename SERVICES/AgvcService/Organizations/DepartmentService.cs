using System.Threading.Tasks;
using AgvcEntitys.Organization;
using AgvcRepository.Orgnizations.Interfaces;
using CoreService;

namespace AgvcService.Organizations
{
    public class DepartmentService : AbstractCrudService<Department>, IDepartmentService
    {
        #region IOC

        private IDepartmentRepository DepartmentRepository { get; }

        public DepartmentService(IDepartmentRepository departmentRepository) : base(departmentRepository)
        {
            DepartmentRepository = departmentRepository;
        }

        #endregion

        #region 部门重复判断

        public Task<bool> IsNameExsitAsync(string orgId, string companyId, string name)
        {
            return DepartmentRepository.AnyAsync(p =>
                p.OrgId == orgId && p.BranchCompanyId == companyId && p.Name == name);
        }

        public Task<bool> IsNameExsitAsync(string orgId, string companyId, string name, string updateId)
        {
            return DepartmentRepository.AnyAsync(p =>
                p.OrgId == orgId && p.BranchCompanyId == companyId && p.Name == name && p.Id != updateId);
        }

        #endregion
    }
}