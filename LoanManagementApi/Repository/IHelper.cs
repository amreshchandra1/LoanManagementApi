using Microsoft.AspNetCore.Mvc;

namespace LoanManagementApi.Repository
{
    public interface IHelper
    {
        public decimal CalculateEmi(double principal, double annualInterestRate, int tenureInMonths);
        public int? TryGetLoanStatusIntValue(string statusString);
        public string EncryptPassword(string password);
    }
}
