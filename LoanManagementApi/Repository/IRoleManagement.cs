using LoanManagementApi.Model;

namespace LoanManagementApi.Repository
{
    public interface IRoleManagement
    {
        public int AddRole(string roleName);
        List<Roles> GetAllRoles();
    }
}
