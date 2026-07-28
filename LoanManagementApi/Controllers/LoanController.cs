using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LoanManagementApi.Model;
using LoanManagementApi.Repository;
using Microsoft.AspNetCore.Authorization;

namespace LoanManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoanController : ControllerBase
    {
        private readonly ILoan _loanRepository;
        private readonly ILogger<Loan> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly ILogin _login;
        public LoanController(ILogger<Loan> logger, ILoan loanRepository, IHttpContextAccessor httpContextAccessor,ILogin login) 
        {
            _loanRepository = loanRepository;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _login = login;
        }
        [HttpPost("UserRegistation")]
        public ActionResult UserRegistation(UserRegistration usrRegis)
        {
            _loanRepository.UserRegistation(usrRegis);
            return Ok();
        }
      //  [Authorize(Roles ="Admin")]
        [HttpPost("CreateLoanApplication")]
        public ActionResult CreateLoanApplication(LoanApplication loanApplication)
        {
             var auth=_httpContextAccessor.HttpContext.Request.Headers.Authorization;
             loanApplication.UserRegistrationUserName = _login.ReadJWT(auth);
            
            _logger.LogInformation("Creating loan application for customer: {CustomerId}", loanApplication.CustomerId);
              var res= _loanRepository.CreateLoanApplication(loanApplication);
             if(res>0)
             {
                _logger.LogInformation("Loan application created successfully for customer: {CustomerId}", loanApplication.CustomerId);
             }
             else
             {
                _logger.LogError("Failed to create loan application for customer: {CustomerId}", loanApplication.CustomerId);
             }
            return Ok();
            // Implementation for creating a loan application
        }
        [HttpPost("ApproveReject")]
        public ActionResult ApproveReject(Guid id, LoanStatus ls)
        {
            _logger.LogInformation("ApproveReject loan application for Id: {id}", id);
            int result= _loanRepository.ApproveReject(id,ls);
            if (result > 0)
            {
                return Ok($"Record Updated {id}");
            }
            else
            {
                return BadRequest("No record updated");
            }
        }
        [HttpPost("CalculateEmi")]
        public  ActionResult CalculateEmi(double principal, double annualInterestRate, int tenureInMonths)
        {
            var emi=_loanRepository.CalculateEmi(principal,annualInterestRate, tenureInMonths);
            return Ok(emi);
        }
        [HttpGet("ViewLoanHistoryByUserName/{username}")]
        public ActionResult<IEnumerable<LoanApplication>> ViewLoanHistoryByUserName(string username)
        {
            return _loanRepository.ViewLoanHistoryByUserName(username).ToList();
        }
    }
}
