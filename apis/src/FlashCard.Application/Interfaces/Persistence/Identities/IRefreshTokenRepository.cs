using FlashCard.Domain.Entities;

namespace FlashCard.Application.Interfaces.Persistence.Identities;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetAsync(string refreshToken);
    Task<RefreshToken> InsertAsync(RefreshToken entity);
    Task UpdateAsync(RefreshToken entity);
    Task RevokeByUserIdAsync(string userId);
}
