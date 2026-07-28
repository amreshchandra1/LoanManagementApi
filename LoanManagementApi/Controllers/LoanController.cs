using LoanManagementApi.Model;
using LoanManagementApi.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
            _logger.LogInformation("Creating User Registation");
           var res= _loanRepository.UserRegistation(usrRegis);
            if(res>0)
            {
                _logger.LogInformation("User Registation created successfully for user: {UserName}", usrRegis.UserName);
                return Ok(res);
            }
            else
            {
                _logger.LogError("Failed to create User Registation for user: {UserName}", usrRegis.UserName);
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to create User Registation");
            }
        }
       // [Authorize(Roles ="Admin")]
        [HttpPost("CreateLoanApplication")]
        public ActionResult CreateLoanApplication(LoanApplication loanApplication)
        {
            _logger.LogInformation("Creating loan application for customer: {CustomerId}", loanApplication.CustomerId);
            var auth=_httpContextAccessor.HttpContext.Request.Headers.Authorization;
             loanApplication.UserRegistrationUserName = _login.ReadJWT(auth);
              var res= _loanRepository.CreateLoanApplication(loanApplication);
             if(res>0)
             {
                _logger.LogInformation("Loan application created successfully for customer: {CustomerId}", loanApplication.CustomerId);
                return Ok(res);
             }
             else
             {
                _logger.LogError("Failed to create loan application for customer: {CustomerId}", loanApplication.CustomerId);
                return BadRequest("Failed to create loan application");
            }
        }
        [HttpGet("UpdateLoanStatus/{id}/{ls}")]
        public ActionResult UpdateLoanStatus(Guid id, LoanStatus ls)
        {
            int res= _loanRepository.UpdateLoanStatus(id, ls);
            if(res>0)
            {
                _logger.LogInformation("Loan status updated successfully for loan application Id: {id}", id);
                return Ok($"Loan status updated successfully for loan application Id: {id}");
            }
            else
            {
                _logger.LogError("Failed to update loan status for loan application Id: {id}", id);
                return BadRequest("Failed to update loan status");
            }
           
        }
        [HttpGet("LoanStatusTracking/{loanid}")]
        public ActionResult<IEnumerable<LoanStatusTracking>> GetLoanStatusTracking(Guid loanid)
        {
            _logger.LogInformation($"Geting LoanStatus");
            return _loanRepository.GetLoanStatusTrackings(loanid).ToList();
        }
        //  [Authorize(Roles = "Admin")]
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
            _logger.LogInformation("Initiating EMI calculation. Principal: {Principal}, Rate: {AnnualInterestRate}%, Tenure: {TenureInMonths} months",
         principal, annualInterestRate, tenureInMonths);

            var emi = _loanRepository.CalculateEmi(principal, annualInterestRate, tenureInMonths);

            _logger.LogInformation("EMI calculation completed successfully. Resulting EMI: {Emi}", emi);

            return Ok(emi);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("ViewLoanHistoryByUserName/{username}")]
        public ActionResult<IEnumerable<LoanApplication>> ViewLoanHistoryByUserName(string username)
        {
            _logger.LogInformation("Fetching loan history records for username: {Username}", username);

            var history = _loanRepository.ViewLoanHistoryByUserName(username).ToList();

            if (!history.Any())
            {
                _logger.LogWarning("No loan history records found matching username: {Username}", username);
                return NotFound($"No loan history found for user: {username}");
            }

            _logger.LogInformation("Successfully retrieved {Count} loan application records for username: {Username}",
                history.Count, username);

            return Ok(history);
        }
       // [Authorize(Roles = "Admin")]
        [HttpGet("LoanStatusTracking")]
        public ActionResult<IEnumerable<LoanStatusTracking>> LoanStatusTracking()
        {
            _logger.LogInformation("Retrieving all loan status tracking records from the database.");

            var trackingList = _loanRepository.LoanStatusTracking().ToList();
            if (!trackingList.Any())
            {
                _logger.LogWarning("The loan status tracking data store returned zero records.");
                return Ok(trackingList); // Returns empty array [ ] with HTTP 200
            }

            _logger.LogInformation("Successfully fetched {Count} status tracking logs.", trackingList.Count);

            return Ok(trackingList);
        }
    }
}
