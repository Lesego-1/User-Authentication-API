using AuthApi.Models;
using Microsoft.AspNetCore.Identity;

namespace AuthApi.Data
{
    public static class DataSeeder
    {
        public static void SeedAdmin(AppDbContext context)
        {
            if (!context.Users.Any(u => u.Role == "Admin"))
            {
                var admin = new User
                {
                    Username = "admin",
                    Email = "admin@example.com",
                    Role = "Admin"
                };

                var passwordHasher = new PasswordHasher<User>();
                admin.PasswordHash = passwordHasher.HashPassword(admin, "AdminPassword123!");

                context.Users.Add(admin);
                context.SaveChanges();
            }
        }
    }
}
