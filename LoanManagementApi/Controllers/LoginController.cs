using LoanManagementApi.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace LoanManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILogin _loginRepository;
        public LoginController(ILogin login)
        {
            _loginRepository = login;
        }
        [AllowAnonymous]
        [HttpPost("GenerateToken")]
        public ActionResult SignIn(string usrname, string password)
        {
            return _loginRepository.GenerateToken(usrname, password);
        }
        
    }
}
