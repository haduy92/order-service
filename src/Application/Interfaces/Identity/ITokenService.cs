using Application.Models;

namespace Application.Interfaces.Identity;

public interface ITokenService
{
    string GenerateAccessToken(string userId, string email);
    RefreshTokenDto GenerateRefreshToken();
}

