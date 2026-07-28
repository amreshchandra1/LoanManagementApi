using LoanManagementApi.Model;
using Microsoft.AspNetCore.Identity;
namespace LoanManagementApi.Repository
{
    public interface ILoan
    {
        public int CreateLoanApplication(LoanApplication loanApplication);
        public int ApproveReject(Guid id,LoanStatus ls);
        public void UserRegistation(UserRegistration userRegistration);
        public decimal CalculateEmi(double principal, double annualInterestRate, int tenureInMonths);
        public List<LoanApplication> ViewLoanHistoryByUserName(string username);
    }
    
}
