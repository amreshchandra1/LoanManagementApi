using LoanManagementApi.Model;

namespace LoanManagementApi.Repository
{
    public interface IUser
    {
        public IEnumerable<UserRegistration> GetUserRegistation();
        public UserRegistration UserRegistation(UserRegistration userRegistration);
    }
}
