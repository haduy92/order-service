using EnglishClass.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EnglishClass.Infrastructure.Repositories;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Card> Cards { get; set; }
    public DbSet<User> Users { get; set; }
}
