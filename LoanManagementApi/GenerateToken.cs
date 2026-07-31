using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWTAuthentication
{
    public interface IGenerateToken
    {
        public IActionResult Token(string username, string password);
    }
    public class GenerateToken: ControllerBase,IGenerateToken
    {
        private readonly IConfiguration _config;
       public GenerateToken(IConfiguration config)
        {
            _config = config;
        }
       public IActionResult Token(string username,string password)
        {
            UserDetails det = new UserDetails();
            var user = det.GetUserDetails().Where(x => x.Username == username && x.Password == password).FirstOrDefault();
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return Unauthorized();
            }
            var key = _config["SecretKey"]??string.Empty;
            //var key = "AmreshChadraSecretKeyJWTAmreshChadraSecretKeyJWT";
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var clames = new[]
            {
                new Claim(JwtRegisteredClaimNames.Name,username),
                new Claim(JwtRegisteredClaimNames.NameId,"123"),
                new Claim(ClaimTypes.Role,user?.Role)//for role based auth
            };
            var tokendata = new JwtSecurityToken(
                claims: clames,
                issuer: "localhost/mytokengenerationapp",
                audience: "localhost/myclientapp",
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256)
                );
            var token = new JwtSecurityTokenHandler().WriteToken(tokendata);
            return Ok(token);
        }
    }
    public class UserDetails
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
      
        public List<UserDetails> GetUserDetails()
        {
            List<UserDetails> usrdetails = new List<UserDetails>()
            {

                new UserDetails()
                {
                    Username="amresh",
                    Password="admin",
                    Role="admin"
                },
                new UserDetails()
                {
                    Username="amresh",
                    Password="hr",
                    Role="hr"
                }
            };
         return usrdetails;
        }  
    }
}
