using LoanManagementApi.Model;
using Microsoft.EntityFrameworkCore;

namespace LoanManagementApi.Repository
{
    public class RoleManagement : IRoleManagement
    {
        private readonly ILogger<RoleManagement> _logger;
        private readonly EFContext _context;
        public RoleManagement( EFContext context, ILogger<RoleManagement> logger)
        {
            _context = context;
            _logger = logger;
        }
        public int AddRole(string roleName)
        {
            int result = 0;
            //if(_context.Roles.Any(x=>x.RoleName==roleName))
            //{
            //    _logger.LogError($"{roleName} is already exist");
            //    return result;
            //}
            _context.Add(new Roles { RoleName = roleName });
            result =  _context.SaveChanges();
            return result;
        }
        public List<Roles> GetAllRoles()
        {
            return _context.Roles.AsNoTracking().ToList();
        }
    }
}
