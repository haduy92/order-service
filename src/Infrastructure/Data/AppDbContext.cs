using Application.Contracts.Application;
using Domain.Entities;
using Infrastructure.Repositories.Configurations;
using Microsoft.EntityFrameworkCore;
using Shared.Extensions;

namespace Infrastructure.Data;

public class AppDbContext : DbContext
{
    private readonly ICurrentUser _currentUser;

    public AppDbContext(DbContextOptions<AppDbContext> options, ICurrentUser currentUser) : base(options)
    {
        _currentUser = currentUser;
    }

    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply entity configurations
        modelBuilder.ApplyConfiguration(new OrderConfiguration());
        modelBuilder.ApplyConfiguration(new OrderItemConfiguration());
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
        var userId = _currentUser.UserId.IsNullOrWhiteSpace() ? "system" : _currentUser.UserId;

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is Entity<int> entity)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entity.CreatorUserId = userId;
                        entity.CreationTime = DateTime.UtcNow;
                        break;
                    case EntityState.Modified:
                        entity.LastModifierUserId = userId;
                        entity.LastModificationTime = DateTime.UtcNow;
                        break;
                    default:
                        // No action needed for Deleted or Unchanged states
                        break;
                }
            }
        }
    }
}

