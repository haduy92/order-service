using Domain.Entities;

namespace Application.Contracts.Persistence.Identities;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetAsync(string refreshToken);
    Task<RefreshToken> InsertAsync(RefreshToken entity);
    Task UpdateAsync(RefreshToken entity);
    Task RevokeByUserIdAsync(string userId);
}

