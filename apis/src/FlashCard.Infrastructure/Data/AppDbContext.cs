using FlashCard.Application.Interfaces.Application;
using FlashCard.Domain.Entities;
using FlashCard.Domain.Entities.Auditing;
using Microsoft.EntityFrameworkCore;

namespace FlashCard.Infrastructure.Data;

public class AppDbContext : DbContext
{
    private readonly ICurrentUser _currentUser;

    public AppDbContext(DbContextOptions<AppDbContext> options, ICurrentUser currentUser) : base(options)
    {
        _currentUser = currentUser;
    }

    public DbSet<Card> Cards { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Card>()
            .HasIndex(p => new { p.Title, p.Description })
            .HasMethod("GIN")
            .IsTsVectorExpressionIndex("english");

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
