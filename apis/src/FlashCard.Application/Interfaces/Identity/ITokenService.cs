using FlashCard.Application.Models;

namespace FlashCard.Application.Interfaces.Identity;

public interface ITokenService
{
    string GenerateAccessToken(string userId, string email);
    RefreshTokenDto GenerateRefreshToken();
}
