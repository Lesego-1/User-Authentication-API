using AuthApi.Models;

public interface ITokenService
{
    string GenerateToken(User user);
    RefreshToken GenerateRefreshToken(User user);  // Added
}
