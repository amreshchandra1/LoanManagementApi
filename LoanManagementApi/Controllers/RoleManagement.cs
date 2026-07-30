using FluentValidation;
using LoanManagementApi.Model;
using LoanManagementApi.Repository;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LoanManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleManagement : ControllerBase
    {
        private readonly ILogger<RoleManagement> _logger;
        private readonly IRoleManagement _roleManagementRepository;
        private readonly IValidator<Roles> _validator;
        public RoleManagement(ILogger<RoleManagement> logger,IRoleManagement roleManagementRepository, IValidator<Roles> validator)
        {
            _logger = logger;
            _roleManagementRepository = roleManagementRepository;
            _validator = validator;

        }
        [HttpGet("AddRole")]
        public async Task< IActionResult> AddRole(string roleName)
        {
            var validationResult = await _validator.ValidateAsync(new Roles() {RoleName=roleName });

            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return BadRequest(ModelState);
            }
            int result=_roleManagementRepository.AddRole(roleName);
            if (result > 0)
            {
                _logger.LogInformation($"Role {roleName} added successfully", roleName);
                return Ok($"Role {roleName} added successfully");
            }
            return BadRequest("No Record Updated");
            
        } 
    }
}
