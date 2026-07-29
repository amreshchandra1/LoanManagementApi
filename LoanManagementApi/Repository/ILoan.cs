using LoanManagementApi.Model;
using Microsoft.AspNetCore.Identity;
namespace LoanManagementApi.Repository
{
    public interface ILoan
    {
        public LoanApplication CreateLoanApplication(LoanApplication loanApplication);
        public int ApproveReject(Guid id,LoanStatus ls);
        public UserRegistration UserRegistation(UserRegistration userRegistration);
        public decimal CalculateEmi(double principal, double annualInterestRate, int tenureInMonths);
        public List<LoanApplication> ViewLoanHistoryByUserName(string username);
        public List<LoanStatusTracking> LoanStatusTracking();
        public int UpdateLoanStatus(Guid id, LoanStatus ls);
        public List<LoanStatusTracking> GetLoanStatusTrackings(Guid guid);
    }
    
}
