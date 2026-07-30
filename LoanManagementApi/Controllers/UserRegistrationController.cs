using FluentValidation;
using LoanManagementApi.Model;
using LoanManagementApi.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LoanManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRegistrationController : ControllerBase
    {
        private readonly ILogger<UserRegistrationController> _logger;
        private readonly IValidator<UserRegistration> _validatorUserRegistration;
        private readonly ILoan _loanRepository;
        private readonly IAuditLog _auditRepository;
        public UserRegistrationController(ILogger<UserRegistrationController> logger, IValidator<UserRegistration> validatorUserRegistration,ILoan loanRepository, IAuditLog auditRepository)
        {
            _logger = logger;
            _validatorUserRegistration = validatorUserRegistration;
            _loanRepository = loanRepository;
            _auditRepository = auditRepository;
        }
        [AllowAnonymous]
        [HttpPost]
        [HttpPost("CreateUserRegistation")]
        public async Task<ActionResult> CreateUserRegistation(UserRegistration usrRegis)
        {
            var validationResult = await _validatorUserRegistration.ValidateAsync(usrRegis);
            List<string> errorlst = new List<string>();
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    // ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                    errorlst.Add(error.ErrorMessage);
                }
                _logger.LogInformation($"Some of validation fail in UserRegistation");
                return BadRequest(new { errors = errorlst });
            }

            _logger.LogInformation("Creating User Registation");
            var res = _loanRepository.UserRegistation(usrRegis);
            if (res != null)
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
    }
}
