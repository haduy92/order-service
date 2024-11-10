using FlashCard.Application.Interfaces.Application;
using FlashCard.Domain.Entities;
using FlashCard.Domain.Entities.Auditing;
using FlashCard.Infrastructure.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FlashCard.Infrastructure.Data;

public class AppIdentityDbContext : IdentityDbContext<ApplicationUser>
{
    private readonly ICurrentUser _currentUser;

    public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options, ICurrentUser currentUser) : base(options)
    {
        _currentUser = currentUser;
    }

    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }

    public override int SaveChanges()
    {
        SetAuditProperties();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SetAuditProperties();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void SetAuditProperties()
    {
        foreach (var entry in ChangeTracker.Entries())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    EntityAuditingHelper.SetCreationAuditProperties(entry, _currentUser.UserId!);
                    break;
                case EntityState.Modified:
                    EntityAuditingHelper.SetModificationAuditProperties(entry, _currentUser.UserId!);
                    break;
            }
        }
    }
}
