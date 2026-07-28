namespace LoanManagementApi.Model
{
    public class UserRegistration
    {
        public int Id {  get; set; }
        public string UserName { get; set; }    
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; }
        public string Password { get; set; }
        public Roles Roles { get; set; }
    }
}
