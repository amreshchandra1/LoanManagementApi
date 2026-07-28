
using LoanManagementApi.Model;

namespace LoanManagementApi.Repository
{
    public class AuditLogRepository : IAuditLog
    {
        private readonly ILogger<AuditLogRepository> _logger;
        private readonly EFContext _context;
        public AuditLogRepository(ILogger<AuditLogRepository> logger, EFContext context)
        {
            _logger = logger;
            _context = context;
        }
        public void LogAction(string username, string operation, string details)
        {
            var log = new AuditLog
            {
                Username = username,
                Operation = operation,
                Details = details,
               
            };

            _context.AuditLogs.Add(log);
            _context.SaveChanges();
        }
    }
}
