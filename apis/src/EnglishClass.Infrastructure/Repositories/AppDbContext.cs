using EnglishClass.Domain.Entities;
using EnglishClass.Domain.Entities.Histories;
using Microsoft.EntityFrameworkCore;

namespace EnglishClass.Infrastructure.Repositories;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    { }

    public DbSet<Card> Cards { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<EntityChange> EntityChanges { get; set; }
    public DbSet<EntityChangeSet> EntityChangeSets { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }

    // TODO: override SaveChanges method to save entity change histories
}
