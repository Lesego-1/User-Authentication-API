using AuthApi.Data;
using AuthApi.DTOs;
using AuthApi.Models;
using AuthApi.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Xunit;

namespace AuthApi.Tests.Services
{
    public class UserServiceTests
    {
        private async Task<AppDbContext> GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            var context = new AppDbContext(options);

            // Seed admin for testing
            if (await context.Users.CountAsync() == 0)
            {
                context.Users.Add(new User
                {
                    Username = "admin",
                    Email = "admin@example.com",
                    PasswordHash = "hashedpassword",
                    Role = "Admin"
                });
                await context.SaveChangesAsync();
            }

            return context;
        }

        [Fact]
        public async Task UserExistsAsync_ShouldReturnTrue_ForExistingUser()
        {
            var context = await GetInMemoryDbContext();
            var service = new UserService(context);

            var exists = await service.UserExistsAsync("admin", "admin@example.com");

            exists.Should().BeTrue();
        }

        [Fact]
        public async Task RegisterAsync_ShouldCreateNewUser()
        {
            var context = await GetInMemoryDbContext();
            var service = new UserService(context);

            var request = new RegisterRequest
            {
                Username = "testuser",
                Email = "test@example.com",
                Password = "Password123!"
            };

            var user = await service.RegisterAsync(request);

            user.Username.Should().Be("testuser");
            user.Email.Should().Be("test@example.com");
            user.Role.Should().Be("User");
        }
    }
}
