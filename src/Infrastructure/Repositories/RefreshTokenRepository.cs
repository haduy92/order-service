using Application.Contracts.Persistence.Identities;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

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

    public async Task<RefreshToken?> GetAsync(string refreshToken)
    {
        return await _dbContext.RefreshTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);
    }

    public async Task UpdateAsync(RefreshToken entity)
    {
        _dbContext.RefreshTokens.Update(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task RevokeByUserIdAsync(string userId)
    {
        var entities = await _dbContext.RefreshTokens
            .Where(rt => rt.CreatorUserId == userId && rt.Revoked == null && rt.Expires < DateTime.UtcNow)
            .ToListAsync();

        if (entities?.Any() == true)
        {
            foreach (var entity in entities) entity.Revoked = DateTime.UtcNow;
        }

        await _dbContext.SaveChangesAsync();
    }

}

