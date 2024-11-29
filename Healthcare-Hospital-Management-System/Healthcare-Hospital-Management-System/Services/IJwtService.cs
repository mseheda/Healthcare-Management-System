using System.Security.Claims;

namespace Healthcare_Hospital_Management_System.Services
{
    public interface IJwtService
    {
        string GenerateToken(string username);
        ClaimsPrincipal ValidateToken(string token);
    }
}
