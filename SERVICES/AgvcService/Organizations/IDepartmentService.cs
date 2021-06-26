using System.Threading.Tasks;
using AgvcEntitys.Organization;
using CoreService.Interfaces;

namespace AgvcService.Organizations
{
    public interface IDepartmentService : ICrudService<Department>
    {
        Task<bool> IsNameExsitAsync(string orgId, string companyId, string name);
        Task<bool> IsNameExsitAsync(string orgId, string companyId, string name, string updateId);
    }
}