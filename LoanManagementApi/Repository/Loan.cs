using JWTAuthentication;
using LoanManagementApi;
using LoanManagementApi.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LoanManagementApi.Repository
{
    public class Loan : ILoan
    {
        private readonly EFContext _context;
        private readonly IGenerateToken _generateToken;
        private readonly IHelper _helper;
        public Loan(EFContext context,IHelper helper) 
        {
            _context = context;
            _helper= helper;
        }
        public IEnumerable< UserRegistration> GetUserRegistation()
        {
            return _context.UserRegistration.ToList();
        }
        public UserRegistration UserRegistation(UserRegistration userRegistration)
        {
            userRegistration.RolesId = _context.Roles.AsNoTracking().Where(x => x.RoleName == userRegistration.RoleName).Select(x=>x.Id).FirstOrDefault();
            userRegistration.Password=_helper.EncryptPassword(userRegistration.Password);
            _context.UserRegistration.Add(userRegistration);
            int res = _context.SaveChanges();
            _context.Entry(userRegistration).Reference(u => u.Roles).Load();
            return userRegistration;
        }
        public bool ValidateUserRegistation(UserRegistration userRegistration)
        {
            var isExist = _context.UserRegistration.Any(x => x.Email == userRegistration.Email || x.UserName == userRegistration.UserName);
            
            return isExist;
        }
        public LoanApplication CreateLoanApplication(LoanApplication loanApplication)
        {
            loanApplication.CalculatedEmi = CalculateEmi(Convert.ToDouble(loanApplication.PrincipalAmount), loanApplication.AnnualInterestRate, loanApplication.TermInMonths);
            _context.LoanApplications.Add(loanApplication);
            var res = _context.SaveChanges();
            return loanApplication;
        }
        public int ApproveReject(Guid id,LoanStatus ls)
        {
            int result = 0;
            var loanApplication = _context.LoanApplications.Find(id);
            if (loanApplication != null)
            {
                loanApplication.Status = ls.ToString();
                _context.Update(loanApplication);
                result=_context.SaveChanges();
            }
            return result;
        }
        public int UpdateLoanStatus(Guid id, LoanStatus ls)
        {
            int result = 0;
            var loanApplication = _context.LoanApplications.Find(id);
            if (loanApplication != null)
            {
                loanApplication.Status = ls.ToString();
                _context.Update(loanApplication);
                result = _context.SaveChanges();
            }
            return result;
        }
        public List<LoanStatusTracking> GetLoanStatusTrackings(Guid guid)
        {
            var p=
            _context.LoanStatusTracking.AsNoTracking()
            .Where(x => x.LoanApplicationId == guid)
              .Select(x => new LoanStatusTracking
              {
               LoanApplicationId = x.LoanApplicationId,
               Status = x.Status,
               SubmittedDate = x.SubmittedDate
              })
             .ToList();
            return p;
        }
       
        public decimal CalculateEmi(double principal, double annualInterestRate, int tenureInMonths)
        {
            // 1. Calculate monthly interest rate (r) from annual rate
            double monthlyRate = annualInterestRate / (12 * 100);

            // 2. Handle 0% interest rate edge case to avoid division by zero
            if (monthlyRate == 0)
            {
                return (decimal)Math.Round(principal / tenureInMonths, 2);
            }

            // 3. Compute (1 + r)^n
            double compoundFactor = Math.Pow( Convert.ToDouble( 1 + monthlyRate), tenureInMonths);

            // 4. Apply the standard EMI formula
            double emi = principal * monthlyRate * compoundFactor / (compoundFactor - 1);

            // 5. Return the result rounded to standard currency decimal places
            return (decimal)Math.Round(emi, 2);
        }
        public List<LoanApplication> ViewLoanHistoryByUserName(string username)
        {
            return _context.LoanApplications.ToList();
        }
        public List<LoanStatusTracking> LoanStatusTracking()
        {
            return _context.LoanStatusTracking.ToList();
        }
    }
}
