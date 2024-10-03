using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace EnglishClass.Infrastructure.Repositories;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<AppDbContext>()
            .Build();
        var connStr = configuration.GetValue<string>("ConnectionStrings:CompliancePpsr");
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql(connStr);

        return new AppDbContext(optionsBuilder.Options);
    }
}
