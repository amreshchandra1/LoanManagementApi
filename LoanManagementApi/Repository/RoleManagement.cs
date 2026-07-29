using LoanManagementApi.Model;

namespace LoanManagementApi.Repository
{
    public class RoleManagement : IRoleManagement
    {
        private readonly EFContext _context;
        public RoleManagement(EFContext context)
        {
            _context = context;
        }
        public void AddRole(string roleName)
        {
            _context.Add(new Roles { RoleName = roleName });
            _context.SaveChanges();
        }
    }
}
