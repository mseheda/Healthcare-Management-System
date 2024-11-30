using Healthcare_Hospital_Management_System.Models;

namespace Healthcare_Hospital_Management_System.Services
{
    public class UserRepository : IUserRepository
    {
        readonly Dictionary<string, string> users = new Dictionary<string, string>
        {
            { "admin@gmail.com", "e564b4081d7a9ea4b00dada53bdae70c99b87b6fce869f0c3dd4d2bfa1e53e1c" },
            { "user@gmail.com", "48a53f0774c8ceff574a1fdcb0d470dbd382b3db273cff4344b6d39d5379c923" },
            { "dummyuser@gmail.com", "2348f998744212575d85959674f9607ab26f67708a917157472832386337c904" },
            { "testuser@gmail.com", "8b5b9db0c13db24256c829aa364aa90c6d2eba318b9232a4ab9313b954d3555f" },
            { "keyuser@gmail.com", "2bb80d537b1da3e38bd30361aa855686bde0eacd7162fef6a25fe97bf527a25b" },
            { "student@gmail.com", "7a3b3573cac24bf3922c7294de56c9f17013da092bfcdb33fc663de934e9546e" },
        };

        public User GetUserByEmail(string email)
        {
            users.TryGetValue(email, out string? passwordHash);

            return new User
            {
                Email = email,
                PasswordHash = passwordHash ?? throw new InvalidOperationException("Login or password are invalid")
            };
        }
    }
}
