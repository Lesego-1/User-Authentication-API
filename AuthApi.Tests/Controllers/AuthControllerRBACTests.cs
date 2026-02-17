using AuthApi.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace AuthApi.Tests.Controllers
{
    public class AuthControllerRBACTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public AuthControllerRBACTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        private async Task<string> RegisterAndLoginAsync(string username, string email, string password)
        {
            await _client.PostAsJsonAsync("/auth/register", new RegisterRequest
            {
                Username = username,
                Email = email,
                Password = password
            });

            var loginResponse = await _client.PostAsJsonAsync("/auth/login", new LoginRequest
            {
                Username = username,
                Password = password
            });

            loginResponse.EnsureSuccessStatusCode();

            var json = await loginResponse.Content.ReadAsStringAsync();
            var loginDto = JsonSerializer.Deserialize<LoginResponseDto>(json)!;

            return loginDto.Token;
        }

        [Fact]
        public async Task Users_ShouldReturn401_WhenNotAdmin()
        {
            var token = await RegisterAndLoginAsync("regularuser", "regular@example.com", "Password123!");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("/auth/users");
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Users_ShouldReturnAllUsers_WhenAdmin()
        {
            var token = await RegisterAndLoginAsync("admin", "admin@example.com", "AdminPass123!");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("/auth/users");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var users = JsonSerializer.Deserialize<List<RegisterResponseDto>>(json)!;

            Assert.NotEmpty(users);
        }

        [Fact]
        public async Task Profile_ShouldReturnUser_WhenAuthenticated()
        {
            var token = await RegisterAndLoginAsync("profileuser", "profile@example.com", "Password123!");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("/auth/profile");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var profile = JsonSerializer.Deserialize<RegisterResponseDto>(json)!;

            Assert.Equal("profileuser", profile.Username);
        }
    }
}
