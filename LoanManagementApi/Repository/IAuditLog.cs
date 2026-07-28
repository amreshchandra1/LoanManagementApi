namespace LoanManagementApi.Repository
{
    public interface IAuditLog
    {
        void LogAction(string username, string operation, string details);
    }
}
