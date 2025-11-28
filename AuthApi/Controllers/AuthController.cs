using AuthApi.Data;
using AuthApi.DTOs;
using AuthApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace AuthApi.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly AppDbContext _context;
        private readonly ILogger<AuthController> _logger; // Injected logger

        public AuthController(
            IUserService userService, 
            ITokenService tokenService, 
            AppDbContext context,
            ILogger<AuthController> logger) // Logger injected
        {
            _userService = userService;
            _tokenService = tokenService;
            _context = context;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _userService.UserExistsAsync(request.Username, request.Email))
            {
                _logger.LogWarning("Attempt to register with existing username/email: {Username} / {Email}", 
                    request.Username, request.Email);
                return BadRequest("Username or Email already exists.");
            }

            var user = await _userService.RegisterAsync(request);

            _logger.LogInformation("New user registered: {Username}", user.Username);

            return Ok(new
            {
                user.Username,
                user.Email,
                user.Role
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userService.AuthenticateAsync(request);

            if (user == null)
            {
                _logger.LogWarning("Failed login attempt for username: {Username}", request.Username);
                return Unauthorized("Invalid username or password.");
            }

            var token = _tokenService.GenerateToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken(user);

            _logger.LogInformation("User logged in: {Username}", user.Username);

            return Ok(new
            {
                user.Username,
                user.Email,
                user.Role,
                Token = token,
                RefreshToken = refreshToken.Token
            });
        }

        [HttpGet("users")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();

            _logger.LogInformation("Admin accessed all users: {AdminUsername}", 
                User.FindFirstValue(ClaimTypes.NameIdentifier));

            return Ok(users.Select(u => new
            {
                u.Username,
                u.Email,
                u.Role
            }));
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var username = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (username == null)
                return Unauthorized("Invalid token");

            var user = await _userService.GetUserByUsernameAsync(username);
            if (user == null)
                return NotFound("User not found");

            _logger.LogInformation("Profile accessed for user: {Username}", username);

            return Ok(new
            {
                user.Username,
                user.Email,
                user.Role
            });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var refreshToken = await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken);

            if (refreshToken == null || refreshToken.IsExpired)
            {
                _logger.LogWarning("Invalid or expired refresh token used");
                return Unauthorized("Invalid or expired refresh token");
            }

            var user = refreshToken.User;

            _context.RefreshTokens.Remove(refreshToken);
            await _context.SaveChangesAsync();

            var newJwt = _tokenService.GenerateToken(user);
            var newRefresh = _tokenService.GenerateRefreshToken(user);

            _logger.LogInformation("Refresh token issued for user: {Username}", user.Username);

            return Ok(new
            {
                Token = newJwt,
                RefreshToken = newRefresh.Token
            });
        }
    }
}
