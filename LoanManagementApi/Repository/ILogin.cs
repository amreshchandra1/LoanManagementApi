using Microsoft.AspNetCore.Mvc;

namespace LoanManagementApi.Repository
{
    public interface ILogin
    {
        public ActionResult GenerateToken(string usrname, string password);
        public string ReadJWT(string jwt);
    }
}
