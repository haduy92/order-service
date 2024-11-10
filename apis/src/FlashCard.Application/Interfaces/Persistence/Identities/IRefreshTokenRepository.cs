using FlashCard.Domain.Entities;

namespace FlashCard.Application.Interfaces.Persistence.Identities;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetAsync(string refreshToken, string userId);
    Task<RefreshToken> InsertAsync(RefreshToken entity);
    Task UpdateAsync(RefreshToken entity);
}
