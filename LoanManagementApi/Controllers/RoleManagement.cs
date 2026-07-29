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
        public RoleManagement(ILogger<RoleManagement> logger,IRoleManagement roleManagementRepository)
        {
            _logger = logger;
            _roleManagementRepository = roleManagementRepository;

        }
        [HttpGet("AddRole")]
        public IActionResult AddRole(string roleName)
        {
            _roleManagementRepository.AddRole(roleName);
            _logger.LogInformation("Role {RoleName} added successfully", roleName);
            return Ok($"Role {roleName} added successfully");
        } 
    }
}
