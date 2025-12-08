using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OpinionsAnalytics.Persistence.Context;
using OpinionsAnalytics.Persistence.Repositories.Dwh;
using OpinionsAnalytics.Application.Repositories;
using OpinionsAnalytics.Worker.Services;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;

        // Connection strings en appsettings.json
        services.AddDbContext<DwhRepositoryContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("Dwh")));

        services.AddDbContext<ResenasContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("Resenas")));

        services.AddScoped<IDwhRepository, DwhRepository>();

        // Hosted service que ejecuta la carga periódica
        services.AddHostedService<DwhLoadBackgroundService>();
    });

var host = builder.Build();
await host.RunAsync();

// appsettings.json
{
  "ConnectionStrings": {
    "Dwh": "Server=...;Database=DwhDb;User Id=...;Password=...;",
    "Resenas": "Server=...;Database=SourceDb;User Id=...;Password=...;"
  },
  "Dwh": {
    "LoadIntervalSeconds": 3600
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}