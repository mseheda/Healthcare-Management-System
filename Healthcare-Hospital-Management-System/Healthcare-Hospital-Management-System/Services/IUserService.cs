using Healthcare_Hospital_Management_System.Models;

namespace Healthcare_Hospital_Management_System.Services
{
    public interface IUserService
    {
        User? GetUserByEmail(string email);

        bool VerifyHashedPassword(string plainPassword, string hashPassword);
    }
}
