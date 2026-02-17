using AuthApi.Data;
using AuthApi.Models;
using AuthApi.Services;
using AuthApi.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Xunit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuthApi.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly AppDbContext _context;
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;

        public AuthServiceTests()
        {
            // 1️⃣ In-memory database for testing
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "AuthServiceTestsDb")
                .Options;
            _context = new AppDbContext(options);

            // 2️⃣ Mock configuration for TokenService
            var inMemorySettings = new Dictionary<string, string> {
                {"JwtSettings:Issuer", "TestIssuer"},
                {"JwtSettings:Audience", "TestAudience"},
                {"JwtSettings:SecretKey", "SuperSecretTestKey123!"}
            };
            IConfiguration config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            // 3️⃣ Initialize TokenService with dependencies
            _tokenService = new TokenService(config, _context);

            // 4️⃣ Initialize UserService with only AppDbContext
            _userService = new UserService(_context);

            // Seed test users
            SeedTestUsers();
        }

        private void SeedTestUsers()
        {
            var hasher = new PasswordHasher<User>();

            var user = new User
            {
                Username = "loginuser",
                Email = "login@example.com",
                Role = "User"
            };
            user.PasswordHash = hasher.HashPassword(user, "TestPassword123!");
            _context.Users.Add(user);

            var admin = new User
            {
                Username = "adminuser",
                Email = "admin@example.com",
                Role = "Admin"
            };
            admin.PasswordHash = hasher.HashPassword(admin, "AdminPass123!");
            _context.Users.Add(admin);

            _context.SaveChanges();
        }

        [Fact]
        public async Task AuthenticateAsync_ShouldReturnUser_WhenCredentialsAreCorrect()
        {
            var request = new LoginRequest
            {
                Username = "loginuser",
                Password = "TestPassword123!"
            };

            var user = await _userService.AuthenticateAsync(request);

            Assert.NotNull(user);
            Assert.Equal("loginuser", user.Username);
        }

        [Fact]
        public async Task AuthenticateAsync_ShouldReturnNull_WhenPasswordIsWrong()
        {
            var request = new LoginRequest
            {
                Username = "loginuser",
                Password = "WrongPassword"
            };

            var user = await _userService.AuthenticateAsync(request);

            Assert.Null(user);
        }
    }
}
