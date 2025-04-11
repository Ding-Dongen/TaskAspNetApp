
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace TaskAspNet.Data.Context;

internal class UserDbContextFactory : IDesignTimeDbContextFactory<UserDbContext>
{
    public UserDbContext CreateDbContext(string[] args)
    {
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "TaskAspNet.Web");

        var config = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<UserDbContext>();
        var connectionString = config.GetConnectionString("IdentityConnection");

        optionsBuilder.UseSqlServer(connectionString);

        return new UserDbContext(optionsBuilder.Options);
    }
}
