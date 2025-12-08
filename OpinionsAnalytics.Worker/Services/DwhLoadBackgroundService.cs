using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using OpinionsAnalytics.Application.Repositories;

namespace OpinionsAnalytics.Worker.Services
{
    public class DwhLoadBackgroundService : BackgroundService
    {
        private readonly ILogger<DwhLoadBackgroundService> _logger;
        private readonly IDwhRepository _dwhRepository;
        private readonly IConfiguration _configuration;
        private readonly TimeSpan _interval;

        public DwhLoadBackgroundService(ILogger<DwhLoadBackgroundService> logger,
                                        IDwhRepository dwhRepository,
                                        IConfiguration configuration)
        {
            _logger = logger;
            _dwhRepository = dwhRepository;
            _configuration = configuration;

            var seconds = _configuration.GetValue<int?>("Dwh:LoadIntervalSeconds") ?? 3600;
            _interval = TimeSpan.FromSeconds(Math.Max(1, seconds));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("DWH loader started with interval {interval}", _interval);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("DWH load triggered at {time}", DateTime.UtcNow);
                    var result = await _dwhRepository.LoadFactsFromResenasAsync(stoppingToken);
                    if (result.IsSucess)
                        _logger.LogInformation("DWH load finished: {message}", result.Message);
                    else
                        _logger.LogWarning("DWH load finished with warnings/errors: {message}", result.Message);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    // shutdown requested
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unhandled error during DWH load");
                }

                try
                {
                    await Task.Delay(_interval, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    // shutdown requested
                }
            }

            _logger.LogInformation("DWH loader stopping");
        }
    }
}