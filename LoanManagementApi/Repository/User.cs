using JWTAuthentication;
using LoanManagementApi.Model;
using Microsoft.EntityFrameworkCore;

namespace LoanManagementApi.Repository
{
    public class User:IUser
    {
        private readonly EFContext _context;
        private readonly IHelper _helper;
        public User(EFContext context, IHelper helper)
        {
            _context = context;
            _helper = helper;
        }
        public IEnumerable<UserRegistration> GetUserRegistation()
        {
            return _context.UserRegistration.ToList();
        }
        public UserRegistration UserRegistation(UserRegistration userRegistration)
        {
            userRegistration.RolesId = _context.Roles.AsNoTracking().Where(x => x.RoleName == userRegistration.RoleName).Select(x => x.Id).FirstOrDefault();
            userRegistration.Password = _helper.EncryptPassword(userRegistration.Password);
            _context.UserRegistration.Add(userRegistration);
            int res = _context.SaveChanges();
            _context.Entry(userRegistration).Reference(u => u.Roles).Load();
            return userRegistration;
        }
    }
}
