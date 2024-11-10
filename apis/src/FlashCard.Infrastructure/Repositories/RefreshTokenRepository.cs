using FlashCard.Application.Interfaces.Persistence.Identities;
using FlashCard.Domain.Entities;
using FlashCard.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FlashCard.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AppIdentityDbContext _dbContext;

    public RefreshTokenRepository(AppIdentityDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RefreshToken> InsertAsync(RefreshToken entity)
    {
        _dbContext.RefreshTokens.Add(entity);
        await _dbContext.SaveChangesAsync();
        return entity;
    }

    public async Task<RefreshToken?> GetAsync(string refreshToken, string userId)
    {
        return await _dbContext.RefreshTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.CreatorUserId == userId);
    }

    public async Task UpdateAsync(RefreshToken entity)
    {
        _dbContext.RefreshTokens.Update(entity);
        await _dbContext.SaveChangesAsync();
    }
}
