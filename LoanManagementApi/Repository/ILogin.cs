using Microsoft.AspNetCore.Mvc;

namespace LoanManagementApi.Repository
{
    public interface ILogin
    {
        public string GenerateToken(string usrname, string password);
        public string ReadJWT(string jwt);
    }
}
