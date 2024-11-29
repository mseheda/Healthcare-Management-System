namespace Healthcare_Hospital_Management_System.Models
{
    public class JwtOptions
    {
        public required string Key { get; set; }
        public required string Issuer { get; set; }
        public required string Audience { get; set; }
        public required double ExpiresInMinutes { get; set; }
    }
}
