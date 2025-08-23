using Application.Models.Auth;

namespace Application.Contracts.Identity;

public interface ITokenService
{
    string GenerateAccessToken(string userId, string email);
    RefreshTokenDto GenerateRefreshToken();
}

