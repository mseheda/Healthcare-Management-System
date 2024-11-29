namespace Healthcare_Hospital_Management_System.Models
{
    public class User
    {
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
    }
}
