using Healthcare_Hospital_Management_System.Models;

namespace Healthcare_Hospital_Management_System.Services
{
    public interface IUserRepository
    {
        User GetUserByEmail(string email);
    }
}
