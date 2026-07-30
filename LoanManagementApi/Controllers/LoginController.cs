using LoanManagementApi.Model;
using LoanManagementApi.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace LoanManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILogin _loginRepository;
        private readonly IAuditLog _auditRepository;
        public LoginController(ILogin login, IAuditLog auditLog)
        {
            _loginRepository = login;
            _auditRepository = auditLog;
        }
        [AllowAnonymous]
        [HttpPost("GenerateToken")]
        [HttpPost("SignIn")]
        public ActionResult SignIn(LoginRequest loginRequest)
        {
            var token= _loginRepository.GenerateToken(loginRequest.Username, loginRequest.Password);
            if(token== "UnAuthorize")
            {
                _auditRepository.LogAction(
                 "",
                 "SignIn",
                 $"Login Fail for {loginRequest.Username}"
                 );
                return Unauthorized("Invalid credentials");
            }
            _auditRepository.LogAction(
                  "",
                  "SignIn",
                  $"Login Sucessfull for {loginRequest.Username} and jwt token is generated"
                  );
            return Ok( token);
           // return _loginRepository.GenerateToken(usrname, password);
        }
        
    }
}
