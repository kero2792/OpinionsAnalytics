namespace OpinionsAnalytics.WksLoadDwh
{
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using OpinionsAnalytics.Persistence.Context;
    using OpinionsAnalytics.Application.Repositories;
    using OpinionsAnalytics.Persistence.Repositories.Dwh;

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);
            var configuration = builder.Configuration;

            // Register DbContexts using connection strings from appsettings.json
            builder.Services.AddDbContext<ResenasContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("Resenas"))
                       .EnableSensitiveDataLogging()
                       .LogTo(System.Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information));

            builder.Services.AddDbContext<DwhRepositoryContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("Dwh"))
                       .EnableSensitiveDataLogging()
                       .LogTo(System.Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information));

            // Repository
            builder.Services.AddScoped<IDwhRepository, DwhRepository>();

            // Hosted service that runs the DWH load loop (service lives in this project)
            builder.Services.AddHostedService<DwhLoadBackgroundService>();

            var host = builder.Build();
            host.Run();
        }
    }
}