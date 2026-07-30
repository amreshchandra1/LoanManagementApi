using FluentValidation;
using LoanManagementApi.Model;
using LoanManagementApi.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Net;

namespace LoanManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoanController : ControllerBase
    {
        private readonly ILoan _loanRepository;
        private readonly ILogger<Loan> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHelper _helper;
        private readonly IAuditLog _auditRepository;
        private readonly ILogin _login;
        private readonly string username;
        private readonly IValidator<UserRegistration> _validator;
        public LoanController(ILogger<Loan> logger, ILoan loanRepository, IHttpContextAccessor httpContextAccessor,ILogin login,IHelper helper,IAuditLog auditLog, IValidator<UserRegistration> validator) 
        {
            _loanRepository = loanRepository;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _helper = helper;
            _login = login;
            _auditRepository = auditLog;
            username = _login.ReadJWT(_httpContextAccessor.HttpContext.Request.Headers.Authorization);
            _validator = validator;
            
        }
        [AllowAnonymous]
        [HttpPost("UserRegistation")]
        public async Task< ActionResult> UserRegistation(UserRegistration usrRegis)
        {
            var validationResult = await _validator.ValidateAsync(usrRegis);
            List<string> errorlst = new List<string>();
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                   // ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                    errorlst.Add(error.ErrorMessage);
                }
                _logger.LogInformation($"Some of validation fail in UserRegistation");
                return BadRequest(new {errors=errorlst } );
            }
           
            _logger.LogInformation("Creating User Registation");
            var res= _loanRepository.UserRegistation(usrRegis);
            if(res!=null)
            {
                _logger.LogInformation("User Registation created successfully for user: {UserName}", usrRegis.UserName);
                _auditRepository.LogAction(
                  "New User",
                  "New User",
                  $"User Registation created successfully for user: {usrRegis.UserName}"
                  );
                return Ok(res);
            }
            else
            {
                _logger.LogError("Failed to create User Registation for user: {UserName}", usrRegis.UserName);
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to create User Registation");
            }
        }
        //[Authorize(Roles = "Admin,Customer")]
        [HttpPost("CreateLoanApplication")]
        public ActionResult CreateLoanApplication(LoanApplication loanApplication)
        {
            _logger.LogInformation("Creating loan application for customer: {CustomerId}", loanApplication.CustomerId);
            loanApplication.UserRegistrationUserName = username;
            var res= _loanRepository.CreateLoanApplication(loanApplication);
             if(res!=null)
             {
                _logger.LogInformation("Loan application created successfully for customer: {CustomerId}", loanApplication.CustomerId);
                _auditRepository.LogAction(
                   string.IsNullOrEmpty(username) ? "user not login" : username,
                   "CreateLoanApplication",
                   $"Loan application created successfully for user {username}"
                   );
                return Ok(res);
             }
             else
             {
                _logger.LogError("Failed to create loan application for customer: {CustomerId}", loanApplication.CustomerId);
                return BadRequest("Failed to create loan application");
            }
        }
       // [Authorize(Roles ="Admin")]
        [HttpGet("UpdateLoanStatus/{id}/{ls}")]
        public ActionResult UpdateLoanStatus(Guid id, LoanStatus ls)
        {
            _logger.LogInformation($"Updating Loan Status for Loan Application {id}");
            int res= _loanRepository.UpdateLoanStatus(id, ls);
            if(res>0)
            {
                _logger.LogInformation($"Loan status updated successfully for loan application Id: {id} with status {ls}");
                _auditRepository.LogAction(
                  string.IsNullOrEmpty(username) ? "user not login" : username,
                  "CreateLoanApplication",
                  $"Loan application created successfully for user {username}"
                  );
                return Ok($"Loan status updated successfully for loan application Id: {id} with status {ls}");
            }
            else
            {
                _logger.LogError("Failed to update loan status for loan application Id: {id}", id);
                return BadRequest("Failed to update loan status");
            }
           
        }
        //[Authorize(Roles = "Admin,Customer")]
        [HttpGet("GetLoanStatusTrackingByLoanId/{loanid}")]
        public ActionResult<IEnumerable<LoanStatusTracking>> GetLoanStatusTrackingByLoanId(Guid loanid)
        {
            _logger.LogInformation($"Geting LoanStatus");
            var result = _loanRepository.GetLoanStatusTrackings(loanid).ToList();
            _auditRepository.LogAction(
                  string.IsNullOrEmpty(username) ? "user not login" : username,
                  "GetLoanStatusTracking",
                  $"Geting Loan Status Tracking for loan  id {loanid}"
                  );
            return Ok(result);
        }
      //  [Authorize(Roles = "Admin")]
        [HttpPost("ApproveReject")]
        public ActionResult ApproveReject(Guid id, LoanStatus ls)
        {
            _logger.LogInformation("ApproveReject loan application for Id: {id}", id);
            int result= _loanRepository.ApproveReject(id,ls);
            if (result > 0)
            {
                _auditRepository.LogAction(
                 string.IsNullOrEmpty(username)? "user not login": username,
                 "ApproveReject",
                 $"Loan Application ID: {id} updated to status: {ls}"
                 );
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

            var emi = _helper.CalculateEmi(principal, annualInterestRate, tenureInMonths);

            _logger.LogInformation("EMI calculation completed successfully. Resulting EMI: {Emi}", emi);

            return Ok(emi);
        }
        //[Authorize(Roles = "Admin,Customer")]
        [HttpGet("ViewLoanHistoryByUserName/{username}")]
        public ActionResult<IEnumerable<LoanApplication>> ViewLoanHistoryByUserName(string username)
        {
            _logger.LogInformation("Fetching loan history records for username: {Username}", username);

            var history = _loanRepository.ViewLoanHistoryByUserName(username).ToList();
            _auditRepository.LogAction(
                 string.IsNullOrEmpty(username) ? "user not login" : username,
                 "ViewLoanHistoryByUserName",
                 $"Geting Loan History for username {username}"
                 );
            if (!history.Any())
            {
                _logger.LogWarning("No loan history records found matching username: {Username}", username);
                return NotFound($"No loan history found for user: {username}");
            }

            _logger.LogInformation("Successfully retrieved {Count} loan application records for username: {Username}",
                history.Count, username);

            return Ok(history);
        }
      //  [Authorize(Roles = "Admin")]
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
