using LoanManagementApi.Model;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace LoanManagementApi
{
    public class EFContext : DbContext
    {
        public EFContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<LoanApplication> LoanApplications { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Employee> Employee { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<UserRegistration > UserRegistration { get; set; }
    }
}
