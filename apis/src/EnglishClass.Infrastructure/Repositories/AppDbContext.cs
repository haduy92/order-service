using EnglishClass.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EnglishClass.Infrastructure.Repositories;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {}

    public DbSet<Card> Cards { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}
