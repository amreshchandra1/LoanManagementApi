using JWTAuthentication;
using LoanManagementApi.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LoanManagementApi.Repository
{
    public class Login : ControllerBase, ILogin
    {
        private readonly EFContext _context;
        private readonly IGenerateToken _generateToken;
        private readonly IConfiguration _config;

        public Login(EFContext context, IGenerateToken generateToken, IConfiguration config)
        {
            _context = context;
            _generateToken = generateToken;
            _config = config;
        }

        public string GenerateToken(string usrname, string password)
        {
            var user = _context.UserRegistration.Include(x => x.Roles).Where(x => x.UserName == usrname && x.Password==password).FirstOrDefault();

            if (user == null)
            {
                return "UnAuthorize";
            }
            
            var key = _config["SecretKey"] ?? string.Empty;
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Name, usrname),
                new Claim(JwtRegisteredClaimNames.NameId, user.UserName),
                new Claim(ClaimTypes.Role, user?.Roles?.RoleName)
            };
            var tokendata = new JwtSecurityToken(
                claims: claims,
                issuer: "mytokengenerationapp",
                audience: "myclientapp",
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256)
            );
            var token = new JwtSecurityTokenHandler().WriteToken(tokendata);
            return token;
        }
        public string ReadJWT(string jwt)
        {
            string? rawToken = jwt?.Replace("Bearer ","");

            var handler = new JwtSecurityTokenHandler();

            if (handler.CanReadToken(rawToken))
            {
                var jwtToken = handler.ReadJwtToken(rawToken);

                // 1. Read standard properties directly
                string issuer = jwtToken.Issuer;
                string audience = jwtToken.Audiences.FirstOrDefault();
                DateTime validTo = jwtToken.ValidTo;

                // 2. Extract specific claims by key names
                string? username = jwtToken.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
                string? userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "nameid")?.Value;
                string? role = jwtToken.Claims.FirstOrDefault(c => c.Type == "role" || c.Type == "http://microsoft.com")?.Value;

                Console.WriteLine($"User: {username}, ID: {userId}, Role: {role}");
                return userId;
            }
            return string.Empty;

           
        }
    }
}
