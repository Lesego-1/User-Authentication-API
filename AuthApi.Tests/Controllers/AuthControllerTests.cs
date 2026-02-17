using AuthApi.DTOs;
using AuthApi.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace AuthApi.Tests.Controllers
{
    public class AuthControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public AuthControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Register_ShouldReturnOk_WhenUserIsNew()
        {
            var request = new RegisterRequest
            {
                Username = "newuser",
                Email = "newuser@example.com",
                Password = "Password123!"
            };

            var response = await _client.PostAsJsonAsync("/auth/register", request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var userResponse = JsonSerializer.Deserialize<RegisterResponseDto>(json)!;

            Assert.Equal("newuser", userResponse.Username);
        }

        [Fact]
        public async Task Login_ShouldReturnToken_WhenCredentialsAreCorrect()
        {
            // Seeded user must exist with hashed password in DB
            var request = new LoginRequest
            {
                Username = "loginuser",
                Password = "TestPassword123!"
            };

            var response = await _client.PostAsJsonAsync("/auth/login", request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var loginResponse = JsonSerializer.Deserialize<LoginResponseDto>(json)!;

            Assert.False(string.IsNullOrEmpty(loginResponse.Token));
            Assert.Equal("loginuser", loginResponse.Username);
        }
    }

    public class RegisterResponseDto
    {
        public string Username { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Role { get; set; } = default!;
    }

    public class LoginResponseDto
    {
        public string Username { get; set; } = default!;
        public string Token { get; set; } = default!;
        public string RefreshToken { get; set; } = default!;
        public string Role { get; set; } = default!;
    }
}
