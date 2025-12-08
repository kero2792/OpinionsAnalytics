using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpinionsAnalytics.Application.Repositories;

namespace OpinionsAnalytics.WksLoadDwh
{
    public class DwhLoadBackgroundService : BackgroundService
    {
        private readonly ILogger<DwhLoadBackgroundService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _interval;

        public DwhLoadBackgroundService(ILogger<DwhLoadBackgroundService> logger,
                                        IServiceProvider serviceProvider,
                                        IConfiguration configuration)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            var seconds = _configuration.GetValue<int?>("Dwh:LoadIntervalSeconds") ?? 3600;
            _interval = TimeSpan.FromSeconds(Math.Max(1, seconds));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("DWH loader started with interval {interval}", _interval);

            // Optional: run once immediately on startup
            await RunOnceAsync(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(_interval, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }

                await RunOnceAsync(stoppingToken);
            }

            _logger.LogInformation("DWH loader stopping");
        }

        private async Task RunOnceAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("DWH load triggered at {time}", DateTime.UtcNow);

                using var scope = _serviceProvider.CreateScope();
                var repo = scope.ServiceProvider.GetRequiredService<IDwhRepository>();

                var result = await repo.LoadFactsFromResenasAsync(cancellationToken);

                if (result.IsSucess)
                    _logger.LogInformation("DWH load finished: {message}", result.Message);
                else
                    _logger.LogWarning("DWH load finished with warnings/errors: {message}", result.Message);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                // shutdown requested
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error during DWH load");
            }
        }
    }
}