using System;

namespace AuthApi.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; } = null!;
        public DateTime Expires { get; set; }
        public DateTime Created { get; set; }
        public bool IsExpired => DateTime.UtcNow >= Expires;

        // Foreign key to User
        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
