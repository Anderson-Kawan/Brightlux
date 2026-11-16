using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.AspNetCore.Identity;

namespace Brightlux.Data;

public class PostgresDbContextFactory : IDesignTimeDbContextFactory<PostgresDbContext>
{
    public PostgresDbContext CreateDbContext(string[] args)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDbContext<PostgresDbContext>(options => options.UseNpgsql(
            "Host=localhost;Database=brightlux;Username=postgres;Password=unused"));
        services.AddDefaultIdentity<IdentityUser>().AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<PostgresDbContext>();
        return services.BuildServiceProvider().GetRequiredService<PostgresDbContext>();
    }
}
