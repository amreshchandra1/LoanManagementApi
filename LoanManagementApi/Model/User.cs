namespace LoanManagementApi.Model
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty; // BCrypt/Argon2 target in production
        public string Role { get; set; } = string.Empty; // Admin, LoanOfficer, Customer
    }
}
